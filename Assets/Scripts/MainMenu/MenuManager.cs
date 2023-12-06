using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    //[SerializeField] GameObject LevelLoader;
    private AudioSource menuSelectSound;
    [SerializeField] GameObject LevelLoader;
    [SerializeField] GameObject ParticleEffect;
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

    public void StartGame()
    {
        ParticleEffect.SetActive(false);
        LevelLoader.SetActive(true);
        SceneLoader.Instance.LoadGameScene();
    }
}
