using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] TextMeshProUGUI score;

    // Update is called once per frame
    void Update()
    {
        score.text = playerTransform.position.x.ToString("0");
    }
}
