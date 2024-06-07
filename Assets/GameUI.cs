using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [SerializeField] GameObject gameLoseScreen;
    [SerializeField] GameObject gameWinScreen;
    bool isGameOver;

    // Start is called before the first frame update
    void Start()
    {
        Guard.OnGuardSpottedPlayer += ShowGameLose;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space)) SceneManager.LoadScene(0);
        }
    }

    void ShowGameWin()
    {
        EndGame(gameWinScreen);
        
    }

    void ShowGameLose()
    {
        EndGame(gameLoseScreen);
    }

    void EndGame(GameObject UI)
    {
        UI.SetActive(true);
        isGameOver = true;
        Guard.OnGuardSpottedPlayer -= ShowGameLose;
    }
}
