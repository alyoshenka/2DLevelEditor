using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// dependency properties for nodes

public class Bullet : MonoBehaviour {

    public Vector2 dir;

    float bulletSpeed;

	// Use this for initialization
	void Start () {
        gameObject.SetActive(false);
        bulletSpeed = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().bulletSpeed;
	}
	
	// Update is called once per frame
	void Update () {

        Vector2 delta = dir * bulletSpeed * Time.deltaTime;
        // move
        if (gameObject.activeSelf) { transform.position += new Vector3(delta.x, delta.y, 0); } // transform.forward * bulletSpeed * Time.deltaTime;transform.forward * bulletSpeed * Time.deltaTime;

        // recycle
        Vector2 scrnPnt = Camera.main.WorldToViewportPoint(transform.position);
        // if outside fov
        if ((scrnPnt.x < 0 || scrnPnt.x > 1) && (scrnPnt.y < 0 || scrnPnt.y > 1))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        if (other.CompareTag("Wall")) { gameObject.SetActive(false); }
    }
}
