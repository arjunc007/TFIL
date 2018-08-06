using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SimpleCharacterControl : MonoBehaviour {

    [SerializeField] private float m_moveSpeed = 2;
    [SerializeField] private float m_turnSpeed = 200;
    [SerializeField] private float m_jumpForce = 4;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidBody;

    ////Throwing settings
    public GameObject stoneModel;
    public const int MAX_STONES = 10;
    private int m_numStones = 0;
    private List<GameObject> stones = new List<GameObject>();
    public float m_throwForce = 5.0f;

    //Input Controls
    public string jumpButton = "A_1";
    public string fireButton = "B_1";
    public string movementX = "LeftJoystickX_1";
    public string movementY = "LeftJoystickY_1";
    public string cameraX = "RightJoystickX_1";
    public string cameraY = "RightJoystickY_1";
    public Transform camera;
    public AudioClip jumpSound, landSound;
    
    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.33f;
    private readonly float m_backwardsWalkScale = 0.16f;
    private readonly float m_backwardRunScale = 0.66f;

    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;

    private bool m_isGrounded;
    private int m_maxJumps = 2;
    private int m_numJumps = 0;
    private List<Collider> m_collisions = new List<Collider>();

    private void Start()
    {
        for(int i = 0; i < MAX_STONES; i++)
        {
            stones.Add(Instantiate(stoneModel));
        }

        foreach(var stone in stones)
            stone.SetActive(false);

        m_numStones = MAX_STONES;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for(int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider)) {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if(validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        } else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

	void Update () {

        if (!GetComponent<CollisionManager>().drowning)
        {
            m_animator.SetBool("Grounded", m_isGrounded);


            //Move the player
            Move();

            JumpingAndLanding();

            //Shoot
            StartCoroutine(ThrowStones());

            m_wasGrounded = m_isGrounded;
        }

        if(transform.position.y < -10)
        {
            var colMan = GetComponent<CollisionManager>();
            AudioSource.PlayClipAtPoint(colMan.hurtSound, transform.position);
            StartCoroutine(colMan.Die());
        }
    }

    private void Move()
    {
        float h = 0f;// = Input.GetAxis("Horizontal");
        float v = 0f;// = Input.GetAxis("Vertical");

        //h = Input.GetAxis("Horizontal");
        //v = Input.GetAxis("Vertical");

        if (Input.GetAxis(movementX) + Input.GetAxis(movementY) != 0)
        {
            h = Input.GetAxis(movementX);
            v = -Input.GetAxis(movementY);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            v *= m_walkScale;
            h *= m_walkScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        Vector3 direction = camera.forward * m_currentV + camera.right * m_currentH;

        float directionLength = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized * directionLength;

        if (direction != Vector3.zero)
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);

            transform.rotation = Quaternion.LookRotation(m_currentDirection);
            transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;

            m_animator.SetFloat("MoveSpeed", direction.magnitude);
        }
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && (Input.GetKey(KeyCode.Space) || Input.GetButtonDown(jumpButton)))
        {
            if (m_isGrounded || m_numJumps < m_maxJumps)
            {
                m_jumpTimeStamp = Time.time;
                m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);

                m_animator.SetTrigger("Jump");
                m_numJumps++;
            }
        }

        if (!m_wasGrounded && m_isGrounded)
        {
            m_animator.SetTrigger("Land");
            m_numJumps = 0;
        }
    }

    private IEnumerator ThrowStones()
    {
        if (Input.GetButtonDown(fireButton) && m_numStones > 0)
        {
            foreach (var stone in stones)
            {
                if (!stone.activeSelf)
                {
                    m_animator.SetTrigger("Throw");

                    yield return new WaitForSeconds(0.8f);
                    //Get a random direction in front of player
                    Vector3 throwDir = transform.forward;   //Forward of player
                    throwDir = Quaternion.Euler(0, Random.Range(-7.5f, 7.5f), 0) * throwDir;    //Yaw
                    throwDir = Quaternion.AngleAxis(Random.Range(-3.75f, 11.25f), transform.right) * throwDir; //Pitch

                    stone.SetActive(true);
                    stone.GetComponent<Rigidbody>().AddForce(throwDir * m_throwForce);
                    stone.transform.position = transform.Find("ThrowPos").transform.position;
                    

                    m_numStones--;

                    
                    break;
                }
            }            
        }

        yield return null;
    }
}
