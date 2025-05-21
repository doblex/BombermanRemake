using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField] UIDocument UI;

    [SerializeField] UIDocument pause;
    Toggle invincibilityToggle;
    Button quitButton;

    [SerializeField] UIDocument endGame;
    Button exitButton;
    Label endGameMessage;

    [SerializeField] Player player;
    List<Enemy> enemies = new List<Enemy>();

    bool isPausing = false;

    private void Awake()
    {
        Setup();
        UISetup();
    }

    private void Update()
    {
        Pause();
    }

    private void Setup()
    {
        enemies.AddRange(FindObjectsByType<Enemy>(FindObjectsSortMode.None));

        foreach (Enemy enemy in enemies)
        {
            enemy.GetComponent<HealthController>().onDeath += OnEnemyDead;
        }

        player.GetComponent<HealthController>().onDeath += OnPlayerDead;
    }

    private void UISetup()
    {
        UI.rootVisualElement.style.display = DisplayStyle.Flex;

        pause.rootVisualElement.style.display = DisplayStyle.None;
        invincibilityToggle = pause.rootVisualElement.Q<Toggle>("Invincible");
        invincibilityToggle.RegisterValueChangedCallback(evt =>
        {
            player.GetComponent<HealthController>().SetInvicible(evt.newValue);
        });
        quitButton = pause.rootVisualElement.Q<Button>("Quit");
        quitButton.clicked += Exit;

        endGame.rootVisualElement.style.display= DisplayStyle.None;
        endGameMessage = endGame.rootVisualElement.Q<Label>("Message");
        exitButton = endGame.rootVisualElement.Q<Button>("Quit");
        exitButton.clicked += Exit;
    }

    private void Exit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    private void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPausing = !isPausing;

            pause.rootVisualElement.style.display = isPausing ? DisplayStyle.Flex : DisplayStyle.None;
            UI.rootVisualElement.style.display = isPausing ? DisplayStyle.None : DisplayStyle.Flex;
            Time.timeScale = isPausing ? 0 : 1;
        }
    }

    private void OnPlayerDead()
    {
        GameEnded(false);
    }

    private void OnEnemyDead()
    {
        bool isGameEnded = true;

        foreach (Enemy enemy in enemies)
        {
            if (!enemy.isDead)
            {
                isGameEnded = false;
                break;
            }
        }

        if (isGameEnded)
        {
            GameEnded();
        }
    }


    private void GameEnded(bool win = true) 
    {
        Time.timeScale = 0;
        UI.rootVisualElement.style.display = DisplayStyle.None;
        endGame.rootVisualElement.style.display = DisplayStyle.Flex;
        endGameMessage.text = win ? "You Win!!!" : "You Lose";
    }
}
