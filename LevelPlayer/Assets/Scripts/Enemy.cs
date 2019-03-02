using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// more use choice here?
    // at least move towards vs shoot at

public class Enemy : MonoBehaviour {

    public float speed;
    Transform player;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
	}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        // take damage
    //        gameObject.SetActive(false);
    //    }
    //}
}
