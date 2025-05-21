using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    UIDocument UIDocument;
    VisualElement root;

    Button playButton;
    Button quitButton;

    private void Awake()
    {
        UIDocument = GetComponent<UIDocument>();
        root = UIDocument.rootVisualElement;

        playButton = root.Q<Button>("Play");
        playButton.clicked += PlayButton_Clicked;
        quitButton = root.Q<Button>("Quit");
        quitButton.clicked += QuitButton_Clicked;
    }

    private void QuitButton_Clicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    private void PlayButton_Clicked()
    {
        SceneManager.LoadScene("Level");
    }
}
