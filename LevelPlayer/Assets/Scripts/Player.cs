using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// add wall to outside
// add code through editor (extension methods?)
// null buttons on resize / move created level around

public class Player : MonoBehaviour {
   
    Vector2 input;
    Vector2 mousePos;
    Rigidbody2D rb;

    public float speed;
    public float bulletSpeed;

	// Use this for initialization
	void Awake ()
    {
        DontDestroyOnLoad(gameObject);
        rb = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        Move();
        // cursor.position = Input.mousePosition;
        Shoot();
	}

    // move player
    void Move()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input = input.normalized;
        input *= speed * Time.deltaTime;
        rb.AddForce(input);
    }

    // shoot towards mouse
    void Shoot()
    {
        if(! Input.GetKeyDown(KeyCode.Mouse0)) { return; } // if nothing    

        GameObject current = (GameObject)Resources.Load("Bullet");
        Vector2 dir = new Vector2();
        dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        Instantiate(current, transform.position, Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90));
    }
}
