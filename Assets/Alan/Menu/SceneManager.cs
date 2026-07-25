using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private static SceneManager SceneManagerInstance;

    public int menuScene = 0;
    public int gameScene = 1;

    private void Awake()
    {
        if (SceneManagerInstance != null)
        {
            Destroy(gameObject);
            return;
        }

        SceneManagerInstance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeSceneToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(menuScene);
    }

    public void ChangeSceneToGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameScene);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
        {
            UnityEditor.EditorApplication.ExitPlaymode();
        }
#endif
    }
}