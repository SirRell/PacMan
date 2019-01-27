using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBoard : MonoBehaviour
{
    private static readonly int boardWidth = 28;
    private static readonly int boardHeight = 36;

    bool didStartDeath = false;

    bool didStartConsumed = false;

    public int totalPellets = 0;
    public static int playerOneScore, playerTwoScore, highScore;
    public static int playerOneLevel = 1, playerTwoLevel = 1; 
    public static int livesPlayerOne = 3, livesPlayerTwo = 0; 
    public int playerOnePelletsConsumed = 0, playerTwoPelletsConsumed = 0;
    public static bool hasPlayedIntro = false;

    public static int ghostConsumedRunningScore = 200;

    public static bool isPlayerOneUp = true;

    public AudioClip consumedGhostAudioClip;

    public AudioClip backgroundAudioNormal, backgroundFrightened;
    public AudioClip backgroundAudioPacManDeath;

    public Text playerText, readyText, highScoreScore, playerOneUp, playerTwoUp, playerOneScoreText, playerTwoScoreText, levelText;
    public Text consumedGhostScoreText;
    public Image playerLives2, playerLives3;


    public static GameObject[,] board = new GameObject[boardWidth, boardHeight];

    public Image[] levelImages;

    GameObject pacMan;
    new AudioSource audio;
    Camera mainCamera;
    GameObject[] allGhosts;
    Object[] allPellets;

    bool didSpawnBonusItem1_player1, didSpawnBonusItem2_player1, didSpawnBonusItem1_player2, didSpawnBonusItem2_player2;


    public void StartDeath()
    {
        if (!didStartDeath)
        {
            didStartDeath = true;
            StopAllCoroutines();

            if (GameMenu.isOnePlayerGame)
            {
                playerOneUp.enabled = true;
            }
            else
            {
                playerOneUp.enabled = true;
                playerTwoUp.enabled = true;
            }

            GameObject bonusItem = GameObject.Find("bonusitem");

            if (bonusItem)
                Destroy(bonusItem.gameObject);

            foreach (GameObject ghost in allGhosts)
            {
                ghost.transform.GetComponent<Ghost>().canMove = false;
            }

            pacMan.GetComponent<PacMan>().canMove = false;
            pacMan.GetComponent<Animator>().enabled = false;
            audio.Stop();

            StartCoroutine(ProcessDeathAfter(2));
        }
    }

    IEnumerator ProcessDeathAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (GameObject ghost in allGhosts)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
        }

        StartCoroutine(ProcessDeathAnimation(2f));
    }

    IEnumerator ProcessDeathAnimation(float delay)
    {
        pacMan.transform.rotation = Quaternion.Euler(0, 0, 0);

        pacMan.GetComponent<Animator>().enabled = true;
        pacMan.GetComponent<Animator>().SetBool("Idle", false);
        pacMan.GetComponent<Animator>().SetBool("Dying", true);
        audio.clip = backgroundAudioPacManDeath;
        audio.PlayOneShot(backgroundAudioPacManDeath);

        yield return new WaitForSeconds(delay);

        StartCoroutine(ProcessRestart(2f));
    }

    IEnumerator ProcessRestart(float delay)
    {
        if (isPlayerOneUp) // Deduct lives, because you died..
            livesPlayerOne -= 1;
        else
            livesPlayerTwo -= 1;

        ghostConsumedRunningScore = 200;

        if (livesPlayerOne == 0 && livesPlayerTwo == 0) //If total Game over...
        {
            readyText.text = "GAME OVER";
            readyText.color = Color.red;
            readyText.enabled = true;
            pacMan.GetComponent<SpriteRenderer>().enabled = false;
            audio.Stop();

            StartCoroutine(ProcessGameOver(2));

        }
        else if (!GameMenu.isOnePlayerGame && (livesPlayerOne == 0 || livesPlayerTwo == 0)) //If game over for only 1 player
        {
            if (livesPlayerOne == 0)
            {
                playerText.text = "PLAYER 1";
            }
            else if (livesPlayerTwo == 0)
            {
                playerText.text = "PLAYER 2";
            }

            playerText.enabled = true;
            readyText.text = "GAME OVER";
            readyText.color = Color.red;
            readyText.enabled = true;

            pacMan.GetComponent<SpriteRenderer>().enabled = false;

            audio.Stop();

            yield return new WaitForSeconds(delay);

            if (!GameMenu.isOnePlayerGame)
                isPlayerOneUp = !isPlayerOneUp;

            if (isPlayerOneUp)
                StartCoroutine(BlinkText(playerOneUp));
            else
                StartCoroutine(BlinkText(playerTwoUp));

            RedrawBoard();

            if (isPlayerOneUp)
                playerText.text = "PLAYER 1";
            else
                playerText.text = "PLAYER 2";

            readyText.text = "GET READY!";
            readyText.color = new Color(107f / 255f, 1, 248f / 255f);

            yield return new WaitForSeconds(delay);

            StartCoroutine(ProcessRestartShowObjects(2));
        }
        else //Not a game over for anybody
        {
            pacMan.GetComponent<SpriteRenderer>().enabled = false;

            audio.Stop();

            if (!GameMenu.isOnePlayerGame)
                isPlayerOneUp = !isPlayerOneUp;

            if (isPlayerOneUp)
                StartCoroutine(BlinkText(playerOneUp));
            else
                StartCoroutine(BlinkText(playerTwoUp));

            if (!GameMenu.isOnePlayerGame)
            {
                if (isPlayerOneUp)
                    playerText.text = "PLAYER 1";
                else playerText.text = "PLAYER 2";
            }


            UpdateUI(); //Update player title
            UpdateLevelUI();
            playerText.enabled = true;
            readyText.enabled = true;

            RedrawBoard();
            yield return new WaitForSeconds(delay);

            StartCoroutine(ProcessRestartShowObjects(1));
        }

    }

    IEnumerator ProcessGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);
        hasPlayedIntro = false;
        SceneManager.LoadScene(0);
    }

    private void Start()
    {
        pacMan = GameObject.FindGameObjectWithTag("PacMan");
        audio = GetComponent<AudioSource>();
        allGhosts = GameObject.FindGameObjectsWithTag("Ghost");
        mainCamera = Camera.main;

        allPellets = FindObjectsOfType(typeof(GameObject));

        foreach (GameObject o in allPellets)
        {
            Vector2 pos = o.transform.position;

            if (o.CompareTag("Node") || o.CompareTag("Pellet"))
            {
                if (o.GetComponent<Tile>() != null)
                {
                    if (o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isSuperPellet)
                    {
                        totalPellets++;
                    }
                }
                board[(int)pos.x, (int)pos.y] = o;
            }
        }

        if (!hasPlayedIntro)
        {
            hasPlayedIntro = true;
            audio.Play();
        }
        StartCoroutine(StartGame());
        UpdateUI();
    }


    /*void Update()
    {
        BonusItems();
    }
    */

    public void BonusItems()
    {
        if (GameMenu.isOnePlayerGame)
        {
            SpawnBonusItemForPlayer(1);
        }
        else
        {
            if (isPlayerOneUp)
            {
                SpawnBonusItemForPlayer(1);
            }
            else
                SpawnBonusItemForPlayer(2);

        }
    }

    void SpawnBonusItemForPlayer(int playernum)
    {
        if(playernum == 1)
        {
            if(playerOnePelletsConsumed >= 70 && playerOnePelletsConsumed < 170)
            {
                if (!didSpawnBonusItem1_player1)
                {
                    didSpawnBonusItem1_player1 = true;
                    SpawnBonusItemForLevel(playerOneLevel);
                }
            }
            else if(playerOnePelletsConsumed >= 170)
            {
                if (!didSpawnBonusItem2_player1)
                {
                    didSpawnBonusItem2_player1 = true;
                    SpawnBonusItemForLevel(playerOneLevel);
                }
            }
        }
        else
        {
            if (playerTwoPelletsConsumed >= 70 && playerTwoPelletsConsumed < 170)
            {
                if (!didSpawnBonusItem1_player2)
                {
                    didSpawnBonusItem1_player2 = true;
                    SpawnBonusItemForLevel(playerTwoLevel);
                }
            }
            else if (playerTwoPelletsConsumed >= 170)
            {
                if (!didSpawnBonusItem2_player2)
                {
                    didSpawnBonusItem2_player2 = true;
                    SpawnBonusItemForLevel(playerTwoLevel);
                }
            }

        }
    }

    void SpawnBonusItemForLevel(int level)
    {
        GameObject bonusitem = null;
        switch (level)
        {
            case 1:
                bonusitem = Resources.Load("Prefabs/bonus_cherries", typeof(GameObject)) as GameObject;
                break;
            case 2:
                bonusitem = Resources.Load("Prefabs/bonus_strawberry", typeof(GameObject)) as GameObject;

                break;
            case 3:
                bonusitem = Resources.Load("Prefabs/bonus_peach", typeof(GameObject)) as GameObject;

                break;
            case 4:
                bonusitem = Resources.Load("Prefabs/bonus_peach", typeof(GameObject)) as GameObject;

                break;
            case 5:
                bonusitem = Resources.Load("Prefabs/bonus_apple", typeof(GameObject)) as GameObject;

                break;
            case 6:
                bonusitem = Resources.Load("Prefabs/bonus_apple", typeof(GameObject)) as GameObject;

                break;
            case 7:
                bonusitem = Resources.Load("Prefabs/bonus_grapes", typeof(GameObject)) as GameObject;

                break;
            case 8:
                bonusitem = Resources.Load("Prefabs/bonus_grapes", typeof(GameObject)) as GameObject;

                break;
            case 9:
                bonusitem = Resources.Load("Prefabs/bonus_galaxian", typeof(GameObject)) as GameObject;

                break;
            case 10:
                bonusitem = Resources.Load("Prefabs/bonus_galaxian", typeof(GameObject)) as GameObject;

                break;
            case 11:
                bonusitem = Resources.Load("Prefabs/bonus_bell", typeof(GameObject)) as GameObject;

                break;
            case 12:
                bonusitem = Resources.Load("Prefabs/bonus_bell", typeof(GameObject)) as GameObject;

                break;
            default:
                bonusitem = Resources.Load("Prefabs/bonus_key", typeof(GameObject)) as GameObject;

                break;
        }
        Instantiate(bonusitem);
    }

    public IEnumerator ProcessConsumedBonusItem(GameObject bonusItem, int scoreValue)
    {
        Vector2 pos = bonusItem.transform.position;
        Vector2 viewPortPoint = mainCamera.WorldToViewportPoint(pos);

        consumedGhostScoreText.GetComponent<RectTransform>().anchorMin = viewPortPoint;
        consumedGhostScoreText.GetComponent<RectTransform>().anchorMax = viewPortPoint;

        consumedGhostScoreText.enabled = true;
        Destroy(bonusItem.gameObject);
        UpdateUI();
        yield return new WaitForSeconds(0.75f);

        consumedGhostScoreText.enabled = false;
    }

    public void UpdateUI()
    {
        int currentLevel;

        playerOneScoreText.text = playerOneScore.ToString();
        playerTwoScoreText.text = playerTwoScore.ToString();

        if (isPlayerOneUp)
        {
            playerText.text = "PLAYER 1";
            levelText.text = playerOneLevel.ToString();
            currentLevel = playerOneLevel;

            if (totalPellets == playerOnePelletsConsumed)
            {
                PlayerWin();
            }
        }
        else
        {
            playerText.text = "PLAYER 2";
            levelText.text = playerTwoLevel.ToString();
            currentLevel = playerTwoLevel;

            if (totalPellets == playerTwoPelletsConsumed)
            {
                PlayerWin();
            }
        }

        if (isPlayerOneUp)
        {
            if(livesPlayerOne == 3)
            {
                playerLives3.enabled = true;
                playerLives2.enabled = true;
            }
            else if(livesPlayerOne == 2)
            {
                playerLives3.enabled = false;
                playerLives2.enabled = true;

            }
            else if(livesPlayerOne == 1)
            {
                playerLives3.enabled = false;
                playerLives2.enabled = false;
            }
        }
        else
        {
            if (livesPlayerTwo == 3)
            {
                playerLives3.enabled = true;
                playerLives2.enabled = true;
            }
            else if (livesPlayerTwo == 2)
            {
                playerLives3.enabled = false;
                playerLives2.enabled = true;

            }
            else if (livesPlayerTwo == 1)
            {
                playerLives3.enabled = false;
                playerLives2.enabled = false;
            }
        }
    }

    void UpdateLevelUI()
    {

        //Make all images disabled
        for (int i = 0; i < levelImages.Length; i++)
        {
            Image li = levelImages[i];
            li.enabled = false;
        }

        int currentLevel;
        if (isPlayerOneUp)
            currentLevel = playerOneLevel;
        else
            currentLevel = playerTwoLevel;

        for (int i = 1; i < levelImages.Length + 1; i++)
        {
            if (currentLevel >= i)
            {
                Image li = levelImages[i - 1];
                li.enabled = true;
            }
        }
    }

    void PlayerWin()
    {
        if (isPlayerOneUp)
        {
            playerOneLevel++;
        }
        else
        {
            playerTwoLevel++;
        }
        StartCoroutine(ProcessWin(2));
    }

    IEnumerator ProcessWin(float delay)
    {
        pacMan.GetComponent<PacMan>().canMove = false;
        pacMan.GetComponent<Animator>().enabled = false;

        audio.Stop();

        foreach(GameObject ghost in allGhosts)
        {
            ghost.GetComponent<Ghost>().canMove = false;
        }

        yield return new WaitForSeconds(delay);

        StartCoroutine(BlinkBoard(2));
    }

    IEnumerator BlinkBoard(float delay)
    {
        pacMan.GetComponent<SpriteRenderer>().enabled = false;

        foreach(GameObject ghost in allGhosts)
        {
            ghost.GetComponent<SpriteRenderer>().enabled = false;
        }

        //Blink board
        GameObject.Find("Maze").GetComponent<Animator>().SetBool("shouldBlink", true);
        yield return new WaitForSeconds(delay);
        //Restart game at next level
        GameObject.Find("Maze").GetComponent<Animator>().SetBool("shouldBlink", false);
        StartNextLevel();
    }

    void StartNextLevel()
    {
        StopAllCoroutines();

        if (isPlayerOneUp)
        {
            ResetPelletsForPlayer(1);
            playerOnePelletsConsumed = 0;
            didSpawnBonusItem1_player1 = false;
            didSpawnBonusItem2_player1 = false;
        }
        else
        {
            ResetPelletsForPlayer(2);
            playerTwoPelletsConsumed = 0;
            didSpawnBonusItem1_player2 = false;
            didSpawnBonusItem2_player2 = false;
        }


        StartCoroutine(ProcessStartNextLevel(1));
    }

    IEnumerator ProcessStartNextLevel(float delay)
    {
        playerText.enabled = true;
        readyText.enabled = true;
        UpdateUI();
        UpdateLevelUI();

        if (isPlayerOneUp)
            StartCoroutine(BlinkText(playerOneUp));
        else
            StartCoroutine(BlinkText(playerTwoUp));

        RedrawBoard();

        yield return new WaitForSeconds(delay);

        StartCoroutine(ProcessRestartShowObjects(1));
    }

    IEnumerator ProcessRestartShowObjects(float delay)
    {
        playerText.enabled = false;

        foreach (GameObject ghost in allGhosts)
        {
            ghost.GetComponent<Animator>().enabled = false;
            ghost.GetComponent<SpriteRenderer>().sprite = ghost.GetComponent<Ghost>().defaultSprite;
            ghost.GetComponent<SpriteRenderer>().enabled = true;
            ghost.GetComponent<Ghost>().MoveToStartingPosition();
        }

        pacMan.GetComponent<PacMan>().MoveToStartingPosition();
        pacMan.GetComponent<Animator>().SetBool("Dying", false);
        pacMan.GetComponent<SpriteRenderer>().sprite = playerLives2.sprite;
        pacMan.transform.rotation = Quaternion.Euler(0, 180, 0);
        pacMan.GetComponent<SpriteRenderer>().enabled = true;

        pacMan.GetComponent<Animator>().enabled = false;

        yield return new WaitForSeconds(delay);

        Restart();
    }

    IEnumerator StartGame()
    {
        if (GameMenu.isOnePlayerGame)
        {
            playerTwoUp.enabled = false;
            playerTwoScoreText.enabled = false;
        }
        else
        {
            playerTwoUp.enabled = true;
            playerTwoScoreText.enabled = true;
        }

        if (isPlayerOneUp)
        {
            StartCoroutine(BlinkText(playerOneUp));
        }
        else
        {
            StartCoroutine(BlinkText(playerTwoUp));
        }

        foreach (GameObject ghost in allGhosts)
        {
            ghost.GetComponent<Ghost>().canMove = false;
        }
        pacMan.GetComponent<Animator>().enabled = false;
        pacMan.GetComponent<PacMan>().canMove = false;

        UpdateLevelUI();

        yield return new WaitForSeconds(2.25f);


        playerText.enabled = false;
        readyText.enabled = false;

        StartCoroutine(StartGameAfter(2));
    }

    IEnumerator BlinkText(Text blinkText)
    {
        yield return new WaitForSeconds(.25f);
        blinkText.enabled = !blinkText.enabled;

        StartCoroutine(BlinkText(blinkText));
    }

    public void StartConsumed(Ghost consumedGhost)
    {
        if (!didStartConsumed)
        {
            didStartConsumed = true;

            // Pause all the ghosts
            foreach(GameObject ghost in allGhosts)
            {
                ghost.transform.GetComponent<Ghost>().canMove = false;
            }

            // Pause pacman
            pacMan.GetComponent<PacMan>().canMove = false;

            // Hide pacman
            pacMan.GetComponent<SpriteRenderer>().enabled = false;

            // Hide consumed ghost
            consumedGhost.transform.GetComponent<SpriteRenderer>().enabled = false;

            // Stop background musci
            audio.Stop();

            Vector2 pos = consumedGhost.transform.position;
            Vector2 viewPortPoint = mainCamera.WorldToViewportPoint(pos);

            consumedGhostScoreText.GetComponent<RectTransform>().anchorMin = viewPortPoint;
            consumedGhostScoreText.GetComponent<RectTransform>().anchorMax = viewPortPoint;

            consumedGhostScoreText.text = ghostConsumedRunningScore.ToString();

            consumedGhostScoreText.GetComponent<Text>().enabled = true;

            // Play the consumed sound
            audio.PlayOneShot(consumedGhostAudioClip);

            // Wait for the audio clip to finish
            StartCoroutine(ProcessConsumedAfter(0.75f, consumedGhost));
            UpdateUI();
        }
    }

    IEnumerator ProcessConsumedAfter(float delay, Ghost consumedGhost)
    {
        yield return new WaitForSeconds(delay);

        //hide the score
        consumedGhostScoreText.GetComponent<Text>().enabled = false;

        //show pacman
        pacMan.GetComponent<SpriteRenderer>().enabled = true;

        //show ghost
        consumedGhost.GetComponent<SpriteRenderer>().enabled = true;

        //resume all ghosts
        foreach(GameObject ghost in allGhosts)
        {
            ghost.transform.GetComponent<Ghost>().canMove = true;
        }

        //resume Pacman
        pacMan.GetComponent<PacMan>().canMove = true;

        //start background music
        audio.Play();

        didStartConsumed = false;
    }


    IEnumerator StartGameAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (GameObject ghost in allGhosts)
        {
            ghost.GetComponent<Ghost>().canMove = true;
        }
        pacMan.GetComponent<Animator>().enabled = true;
        pacMan.GetComponent<PacMan>().canMove = true;

        audio.clip = backgroundAudioNormal;
        audio.Play();
    }

    public void Restart()
    {
        int playerLevel = 0;

        if (isPlayerOneUp)
            playerLevel = playerOneLevel;
        else
            playerLevel = playerTwoLevel;

        pacMan.GetComponent<PacMan>().SetDifficultyForLevel(playerLevel);


        foreach (GameObject ghost in allGhosts)
        {
            ghost.GetComponent<Ghost>().SetDifficultyForLevel(playerLevel);
            ghost.transform.GetComponent<Ghost>().Restart();
        }

        readyText.enabled = false;
        playerText.enabled = false;

        pacMan.GetComponent<PacMan>().Restart();

        audio.clip = backgroundAudioNormal;
        audio.Play();

        didStartDeath = false;
    }

    void ResetPelletsForPlayer(int playerNum)
    {
        foreach (GameObject o in allPellets)
        {
            if (o.GetComponent<Tile>() != null)
            {
                if(o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isSuperPellet)
                {
                    if(playerNum == 1)
                    {
                        o.GetComponent<Tile>().didConsumePlayerOne = false;
                    }
                    else
                    {
                        o.GetComponent<Tile>().didConsumePlayerTwo = false;
                    }
                }
            }
        }

          
    }

    void RedrawBoard()
    {
        foreach(GameObject o in allPellets)
        {
            if (o.GetComponent<Tile>() != null)
            {
                if (o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isSuperPellet)
                {
                    if (isPlayerOneUp)
                    {
                        if (o.GetComponent<Tile>().didConsumePlayerOne)
                            o.GetComponent<SpriteRenderer>().enabled = false;
                        else
                            o.GetComponent<SpriteRenderer>().enabled = true;
                    }
                    else
                    {
                        if (o.GetComponent<Tile>().didConsumePlayerTwo)
                            o.GetComponent<SpriteRenderer>().enabled = false;
                        else
                            o.GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
            }
        }
    }

}
