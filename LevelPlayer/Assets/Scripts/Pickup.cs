using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

    GameManager gm;

	// Use this for initialization
	void Start () {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            // Destroy(gameObject);
            // add to player stats

            // this would be a cool place for dynamic script

            // check for win cond
            // if(GameObject.FindGameObjectsWithTag("Pickup").Length == 0) { GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().TryToAdvance(); }
            gm.TryToAdvance();
        }
    }
}
