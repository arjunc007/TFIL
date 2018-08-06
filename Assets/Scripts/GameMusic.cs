using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusic : MonoBehaviour {

    public AudioClip Music;

	// Use this for initialization
	void Start () {
        AudioSource.PlayClipAtPoint(Music, transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
