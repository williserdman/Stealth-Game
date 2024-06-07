using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject gameLoseUI;
    [SerializeField] GameObject gameWinUI;


    bool isGameOver;

    // Start is called before the first frame update
    void Start()
    {
        Guard.OnGuardSpottedPlayer += OnGameLoss;
        FindObjectOfType<Player>().endOfLevel += OnGameWin;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void GeneralGameOver(GameObject UI)
    {
        UI.SetActive(true);
        isGameOver = true;
        Guard.OnGuardSpottedPlayer -= OnGameLoss;
        FindObjectOfType<Player>().endOfLevel -= OnGameWin;
    }

    void OnGameWin()
    {
        GeneralGameOver(gameWinUI);
    }

    void OnGameLoss()
    {
        GeneralGameOver(gameLoseUI);
    }
}
