using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CollisionManager : MonoBehaviour {
    GameObject lastObject;
    public float RespawnTime = 3f;
    public int GameOverTime;
    private Vector3 respawnPoint;
    private Vector3 startPos;
    public Text Win;
    public AudioClip hurtSound, winSound, landSound;
    

    public bool drowning = false;

	// Use this for initialization
	void Start () {
        startPos = transform.position;

    }
	
	// Update is called once per frame
	void Update () {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player" || collision.transform.tag == "Bullet")
        {
            return;
        }
        if(collision.gameObject.tag == "Lava")
        {
            AudioSource.PlayClipAtPoint(hurtSound, transform.position);
            StartCoroutine(Die());
        }
        else if(collision.transform.parent.gameObject.tag == "Exit")
        {
            AudioSource.PlayClipAtPoint(landSound, transform.position);
            AudioSource.PlayClipAtPoint(winSound, transform.position);
            Win.text = "Winner!";
            Invoke("GameOver", GameOverTime);
        }
        else
        {
            lastObject = collision.gameObject;
        }
        

    }

    void RespawnPlayer()
    {
        transform.position = respawnPoint;
    }

    void GameOver()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public IEnumerator Die()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("Die");
        drowning = true;
        yield return new WaitForSeconds(3);

        RaycastHit hitInfo;
        if (Physics.Raycast(lastObject.transform.position + Vector3.up * 5f, Vector3.down, out hitInfo))
        {
            respawnPoint = hitInfo.point + Vector3.up * 2f;
            Invoke("RespawnPlayer", RespawnTime);
           
            //Restart animator
            anim.Rebind();
        }
        drowning = false;
    }
    
}
