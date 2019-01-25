using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public static bool isOnePlayerGame = true; // need to change back to true


    public Text playerText1;
    public Text playerText2;
    public Text playerSelector;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (!isOnePlayerGame)
            {
                isOnePlayerGame = true;
                playerSelector.transform.position = new Vector3(playerSelector.transform.position.x, playerText1.transform.position.y, playerSelector.transform.position.z);
            }
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (isOnePlayerGame)
            {
                isOnePlayerGame = false;
                playerSelector.transform.position = new Vector3(playerSelector.transform.position.x, playerText2.transform.position.y, playerSelector.transform.position.z);

            }
        }
        else if (Input.GetKeyUp(KeyCode.Return))
        {
            GameBoard.playerOneLevel = 1;
            GameBoard.playerOneScore = 0;
            GameBoard.livesPlayerOne = 3;

            GameBoard.playerTwoLevel = 1;
            GameBoard.playerTwoScore = 0;
            GameBoard.livesPlayerTwo = 3;

            if (isOnePlayerGame)
                GameBoard.livesPlayerTwo = 0;

            SceneManager.LoadScene(1);
        }
    }
}
