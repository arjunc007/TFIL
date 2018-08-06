using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookieMovement : MonoBehaviour {
    public float SpinSpeed;

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.rotation *= Quaternion.Euler(0f, SpinSpeed * Time.deltaTime, 0f);
	}
}
