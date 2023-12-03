using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    private AudioSource menuSelectSound;
    private void Awake()
    {
        menuSelectSound = this.GetComponent<AudioSource>();
    }
    public void TextColorPointerEnter(TextMeshProUGUI textMesh)
    {
        textMesh.color = HexToColor("#FF5B00");
        menuSelectSound.Play();
    }


    public void TextColorPointerExit(TextMeshProUGUI textMesh)
    {
        textMesh.color = Color.white;
    }

    private Color HexToColor(string hex)
    {
        Color color = Color.white;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }
        else
        {
            Debug.LogError("Invalid hex color code: " + hex);
            return Color.white;
        }
    }

}
