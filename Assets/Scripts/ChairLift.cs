using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairLift : MonoBehaviour
{
    [SerializeField] float activationDistanceX = 1f;
    [SerializeField] GameObject[] ChairLiftPrefabs;
    [SerializeField] PhysicMaterial Friction;

    private Transform ChairliftTransform;
    private Rigidbody ChairliftRB;
    private Animator ChairliftAnimator;
    private Transform Player;

    private bool willFall;
    private bool coroutineRunning = false;
    

    private void Awake()
    {
        willFall = Random.value > 0.95f;
        if (willFall)
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
            ChairliftTransform = ChairLiftPrefabs[Random.Range(0, ChairLiftPrefabs.Length)].transform;
            ChairliftRB = ChairliftTransform.GetComponent<Rigidbody>();
            ChairliftAnimator = ChairliftTransform.GetComponent<Animator>();
        }
    }


    void Update()
    {
        if (willFall)
        {
            float distanceX = Vector3.Distance(new Vector3(transform.position.x, 0, 0), new Vector3(Player.position.x, 0, 0));
            if (distanceX < activationDistanceX && !coroutineRunning)
            {
                StartCoroutine(ChairLiftFall());
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Slope"))
        {
            ChairliftTransform.GetComponent<MeshCollider>().material = Friction;
        }
    }

    IEnumerator ChairLiftFall()
    {
        coroutineRunning = true;
        ChairliftAnimator.SetBool("ChairFall", true);

        while (Vector3.Distance(new Vector3(transform.position.x, 0, 0), new Vector3(Player.position.x, 0, 0)) > 7f)
        {
            yield return null;
        }

        ChairliftAnimator.SetBool("ChairFall", false);
        ChairliftRB.isKinematic = false;
    }
}
