using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("p");
            // GameManager gm = FindObjectOfType<GameManager>();
            GameManager gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>(); // gross
            gm.atGoal = true;
            FindObjectOfType<GameManager>().TryToAdvance();
        }
    }
}
