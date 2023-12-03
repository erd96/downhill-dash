using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBoulder : MonoBehaviour
{
    Transform playerPosition;

    private void Awake()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Obstacle1Z") || collision.transform.CompareTag("Osbtacle3Z"))
        {
            StartCoroutine(CheckPlayerPosition());
        }
    }

    IEnumerator CheckPlayerPosition()
    {
        while (true)
        {
            if (playerPosition.position.x > transform.position.x)
            {
                yield return new WaitForSeconds(1f);
                Destroy(this.gameObject);
                yield break; 
            }

            yield return null; 
        }
    }
}

