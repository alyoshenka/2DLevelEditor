using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// dependency properties for nodes

public class Bullet : MonoBehaviour {

    public Vector2 dir;
    float bulletSpeed;
    Camera cam;

	// Use this for initialization
	void Start () {
        bulletSpeed = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().bulletSpeed;
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += transform.up * bulletSpeed * Time.deltaTime;

        Vector2 scrnPnt = cam.WorldToViewportPoint(transform.position);
        // if outside fov
        if ((scrnPnt.x < 0 || scrnPnt.x > 1) && (scrnPnt.y < 0 || scrnPnt.y > 1)) { Destroy(gameObject); }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) { Destroy(other.gameObject); Destroy(gameObject); }
        if (other.CompareTag("Wall"))  { Destroy(gameObject); }
    }
}
