using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerate : MonoBehaviour
{
    [Header("Positional Info")]
    public int number = 50;
    public int max = 20;
    public int min = 0;
    public int maxX = 0;
    public int maxY = 3;
    public int maxZ = 0;
    public float minAngle, maxAngle;
    [Space]
    public GameObject spawn;
    public GameObject exit;
    List<GameObject> sceneObjects = new List<GameObject>();
    public GameObject[] objectTypes = new GameObject[9];
    // Use this for initialization
    void Start()
    {
        GenerateObjects();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("X_1"))
        {
            SceneManager.LoadScene("MainMenu");
        }

    }

    public Vector3 GeneratePos()
    {

        int x, y, z;
        x = Random.Range(min, maxX);
        z = Random.Range(min, maxZ);
        y = Random.Range(min, maxY);
        return new Vector3(x, y, z);
    }

    void GenerateObjects()
    {
        Vector3 pos = new Vector3(10, 9, 0);
        Quaternion rotation = Quaternion.Euler(0, 90, 0);
        GameObject go = Instantiate(spawn, pos, rotation);
        sceneObjects.Add(go);
        pos = new Vector3(10, -5, 75);
        go = Instantiate(exit, pos, Quaternion.identity);
        sceneObjects.Add(go);

        for (int i = 0; i < number; i++)
        {
            int random = Random.Range(0, 9);
            float randX, randY, randZ;
            randX = Random.Range(minAngle, maxAngle);
            randY = Random.Range(minAngle, maxAngle);
            randZ = Random.Range(minAngle, maxAngle);

            Quaternion newRotation = Quaternion.Euler(randX, randY, randZ);

            go = Instantiate(objectTypes[random], GeneratePos(), newRotation);

            while (CheckCollision(go))
            {
                go.transform.position = GeneratePos();
            }

            sceneObjects.Add(go);
        }
    }

    bool CheckCollision(GameObject go)
    {
        var goColliders = go.GetComponentsInChildren<Collider>();

        for (var i = 0; i < sceneObjects.Count; i++)
        {
            var objColliders = sceneObjects[i].GetComponentsInChildren<Collider>();

            for (var j = 0; j < objColliders.Length; j++)
            {
                for (var k = 0; k < goColliders.Length; k++)
                {
                    if (objColliders[j].bounds.Intersects(goColliders[k].bounds))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
