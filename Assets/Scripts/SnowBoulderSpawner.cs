using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBoulderSpawner : MonoBehaviour
{
    [SerializeField] GameObject pfBoulder;
    private void Start()
    {
        StartCoroutine(SpawnBoulderRoutine());
    }

    IEnumerator SpawnBoulderRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            if (ShouldInstantiateBoulder())
            {
                GameObject boulderTransform = Instantiate(pfBoulder, transform.position, transform.rotation);
                boulderTransform.GetComponent<Rigidbody>().velocity = boulderTransform.transform.forward * GameManager.Instance.playerSpeed * 1.5f;
            }
        }
    }

    bool ShouldInstantiateBoulder() => Random.value > 0.9;
}
