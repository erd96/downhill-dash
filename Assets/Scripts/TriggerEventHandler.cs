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
