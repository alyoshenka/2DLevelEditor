using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// add wall to outside

public class PlayerController : MonoBehaviour {

    public float speed;

    Vector2 input;
    Rigidbody2D rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input = input.normalized;
        input *= speed * Time.deltaTime;
        rb.AddForce(input);
	}
}
