using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// add wall to outside
// add code through editor (extension methods?)
// null buttons on resize / move created level around

public class Player : MonoBehaviour {
   
    Vector2 input;
    Vector2 mousePos;
    Rigidbody2D rb;

    public float speed;
    public float bulletSpeed;
    public bool canShoot;
    public int health;
    float hitReloadTimer;
    Text healthT;

	// Use this for initialization
	void Awake ()
    {
        DontDestroyOnLoad(gameObject);
        rb = GetComponent<Rigidbody2D>();
        canShoot = false;
        health = 10;
        hitReloadTimer = 0;
        healthT = GameObject.FindGameObjectWithTag("Health").GetComponent<Text>();
        healthT.text = "Health: " + health;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Move();
        // cursor.position = Input.mousePosition;
        Shoot();

        hitReloadTimer += Time.deltaTime;
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
        if(! Input.GetKeyDown(KeyCode.Mouse0) || ! canShoot) { return; } // if nothing    

        GameObject current = (GameObject)Resources.Load("Bullet");
        Vector2 dir = new Vector2();
        dir = (UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        Instantiate(current, transform.position, Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy" && hitReloadTimer > 0.5)
        {
            health--;
            if(health <= 0) { SceneManager.LoadScene("Lose"); }
            healthT.text = "Health: " + health;
            hitReloadTimer = 0;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy" && hitReloadTimer > 0.5)
        {
            health--;
            if (health <= 0) { SceneManager.LoadScene("Lose"); }
            healthT.text = "Health: " + health;
            hitReloadTimer = 0;
        }
    }
}
