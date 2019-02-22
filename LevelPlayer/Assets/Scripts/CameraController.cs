using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    Transform player;

	// Use this for initialization
	void Start ()
    {
        try
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            transform.position = new Vector3(player.position.x, player.position.y, -10);
        }
        catch { }
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
