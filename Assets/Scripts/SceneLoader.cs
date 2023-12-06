using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    [SerializeField] Animator animator;

    private void Awake()
    {
        Instance = this;
    }


    public void LoadGameScene()
    {
        StartCoroutine(LoadScene(1));
    }

    IEnumerator LoadScene(int GameScene)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1);
    }

}
