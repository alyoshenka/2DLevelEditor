using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// add wall to outside
// add code through editor (extension methods?)
// null buttons on resize / move created level around

public class PlayerController : MonoBehaviour {
   
    Vector2 input;
    Vector2 mousePos;
    Rigidbody2D rb;
    GameObject[] bullets;
    Renderer rend;

    public float speed;
    public float bulletSpeed;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        bullets = new GameObject[20];
        for(int i = 0; i < bullets.Length; i++)
        {
            bullets[i] = Instantiate((GameObject)Resources.Load("Bullet"));
        }
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
        // bullets updae and recycle on their Update       

        // find bullet
        int i;
        for(i = 0; i < bullets.Length; i++)
        {
            if (! bullets[i].gameObject.activeSelf) { break; } // found allocated space
            if (i == bullets.Length - 1) { return; } // if no bullets available
        }

        // make new bullet
        bullets[i].SetActive(true);        
        bullets[i].transform.position = transform.position;
        Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
        dir = dir.normalized;
        bullets[i].GetComponent<Bullet>().dir = dir;
        bullets[i].transform.LookAt(transform.position + new Vector3(dir.x, dir.y, 0)); // ??
        bullets[i].transform.rotation = Quaternion.AngleAxis(0, bullets[i].transform.up);
    }
}
