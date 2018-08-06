using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public Camera[] allCams;

    private int numCams = 0;

    private void Awake()
    {
        
        numCams = allCams.Length;

    }
 
	// Update is called once per frame
	void Update ()
    {

        Vector3 pos = Vector3.zero;

        foreach (Camera cam in allCams)
        {
            pos += cam.transform.position;
        }

        if (numCams != 0)
            pos /= numCams;

        transform.position = pos;
    }
}
