using System.Collections;
using UnityEngine;

public class SkiRail : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float rotationDuration = 0.2f;
    private bool isOnRail = false;
    private float randomRotation;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && !isOnRail)
        {
            isOnRail = true;
            randomRotation = Random.Range(0f, 1f) < 0.5f ? 90f : -90f;
            StartCoroutine(RotatePlayer(randomRotation));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player") && isOnRail)
        {
            StartCoroutine(RotatePlayer(-randomRotation));
        }
    }

    private IEnumerator RotatePlayer(float targetAngle)
    {
        float elapsedRotationTime = 0f;
        Quaternion startRotation = player.rotation;
        Quaternion targetRotation = player.rotation * Quaternion.Euler(0f, targetAngle, 0f);

        while (elapsedRotationTime < rotationDuration)
        {
            player.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedRotationTime / rotationDuration);
            elapsedRotationTime += Time.deltaTime;
            yield return null;
        }

        player.rotation = targetRotation;
    }
}
