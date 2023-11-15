using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEventHandler : MonoBehaviour
{
    private HillMeshInstantiator meshInstantiator;
    private bool isTriggerHandled = false;

    // Start is called before the first frame update
    void Start()
    {
        meshInstantiator = GameObject.Find("HMInstantiator").GetComponent<HillMeshInstantiator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggerHandled)
        {
            if (gameObject.name == "meshTriggerInstantiate")
            {
                GameManager.Instance.playerSpeed+=0.5f;
                GameManager.Instance.maxSlope += 0.05f;
                meshInstantiator.OnMeshTriggerInstantiate();
            }
            else if (gameObject.name == "meshTriggerDestroy")
            {
                meshInstantiator.OnMeshTriggerDestroy();
            }
            else if (gameObject.name == "leftBoundsTrigger" || gameObject.name == "rightBoundsTrigger")
            {
                meshInstantiator.OnBoundsTrigger();
            }

            isTriggerHandled = true;
            StartCoroutine(ResetTriggerHandling());
        }
    }

    private IEnumerator ResetTriggerHandling()
    {
        yield return new WaitUntil(() => !isTriggerHandled);
        isTriggerHandled = false;
    }
}
