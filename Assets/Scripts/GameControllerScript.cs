using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("A_1"))
        {
            SceneManager.LoadScene("SinglePlayer");
        }

        if (Input.GetButton("B_1"))
        {
            Application.Quit();
        }

        if (Input.GetButton("Y_1"))
        {
            SceneManager.LoadScene("Multiplayer");
        }
    }
}
