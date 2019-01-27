using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : MonoBehaviour
{
    public AudioClip chomp1, chomp2, eatFruit;

    public Vector2 orientation;

    public float speed = 6.0f;

    public bool canMove = true;

    bool playChomp1 = false;
    new AudioSource audio;
    Animator anim;

    Vector2 direction = Vector2.zero;
    Vector2 nextDirection;

    Node currentNode, previousNode, targetNode;

    Node startingPos;

    GameBoard gameBoard;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        Node node = GetNodeAtPosition(transform.localPosition);
        anim = GetComponent<Animator>();
        gameBoard = GameObject.Find("Game").GetComponent<GameBoard>();


        if(node != null)
        {
            currentNode = node;
            startingPos = node;
        }

        direction = Vector2.left;
        orientation = Vector2.left;

        ChangePosition(direction);
        UpdateOrientaion();

        if (GameBoard.isPlayerOneUp)
        {
            SetDifficultyForLevel(GameBoard.playerOneLevel);
        }
        else
        {
            SetDifficultyForLevel(GameBoard.playerTwoLevel);
        }

    }

    public void Restart()
    {
        canMove = true;

        currentNode = startingPos;

        direction = Vector2.left;
        orientation = Vector2.left;
        nextDirection = Vector2.left;

        anim.enabled = true;
        ChangePosition(direction);
        UpdateOrientaion();
    }

    public void SetDifficultyForLevel(int level)
    {
        if(level == 1)
        {
            speed = 6;
        }
        else if (level == 2)
        {
            speed = 7;
        }
        else if (level == 3)
        {
            speed = 8;
        }
        else if (level == 4)
        {
            speed = 9;
        }
        else if (level > 4)
        {
            speed = 10;
        }
    }

    public void MoveToStartingPosition()
    {
        transform.position = startingPos.transform.position;
        UpdateOrientaion();
    }

    void Update()
    {
        if (canMove)
        {
            CheckInput();

            Move();

            UpdateOrientaion();

            UpdateAnimationState();

            ConsumePellet();
        }
    }

    void PlayChompSound()
    {
        if (playChomp1)
        {
            //play chomp2, set play chom1 to false
            audio.PlayOneShot(chomp2);
            playChomp1 = false;
        }
        else
        {
            //play chom1, set played chomp1 to true
            audio.PlayOneShot(chomp1);
            playChomp1 = true;
        }
    }

    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangePosition(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangePosition(Vector2.right);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangePosition(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangePosition(Vector2.down);
        }
    }

    void ChangePosition (Vector2 d)
    {
        if (d != direction)
            nextDirection = d;

        if(currentNode != null)
        {
            Node moveToNode = CanMove(d);

            if(moveToNode != null)
            {
                direction = d;
                targetNode = moveToNode;
                previousNode = currentNode;
                currentNode = null;
            }
        }
    }

    void UpdateAnimationState()
    {
        if (direction == Vector2.zero)
        {
            anim.SetBool("Idle", true);
        }
        else
        {
            anim.SetBool("Idle", false);
        }
    }

    void Move()
    {
        if(targetNode != currentNode && targetNode != null)
        {
            if (nextDirection == -direction)
            {
                direction *= -1;

                Node tempNode = targetNode;

                targetNode = previousNode;
                previousNode = tempNode;
            }

            if (OverShotTarget())
            {
                currentNode = targetNode;

                transform.localPosition = currentNode.transform.position;

                GameObject otherPortal = GetPortal(currentNode.transform.position);

                if (otherPortal != null)
                {
                    transform.localPosition = otherPortal.transform.position;

                    currentNode = otherPortal.GetComponent<Node>();
                }

                Node moveToNode = CanMove(nextDirection);

                if(moveToNode != null)
                    direction = nextDirection;

                if (moveToNode == null)
                    moveToNode = CanMove(direction);

                if (moveToNode != null)
                {
                    targetNode = moveToNode;
                    previousNode = currentNode;
                    currentNode = null;
                }
                else
                {
                    direction = Vector2.zero;
                }
            }
            else
            {
                transform.localPosition += (Vector3)direction * speed * Time.deltaTime;
            }
        }
    }

    void MoveToNode(Vector2 d)
    {
        Node moveToNode = CanMove(d);
        if(moveToNode != null)
        {
            transform.localPosition = moveToNode.transform.position;
            currentNode = moveToNode;
        }
    }

    public void UpdateOrientaion()
    {
        if(direction == Vector2.left)
        {
            orientation = Vector2.left;
            transform.localRotation = Quaternion.Euler(0, 0, 180);
        }
        else if(direction == Vector2.right)
        {
            orientation = Vector2.right;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction == Vector2.up)
        {
            orientation = Vector2.up;
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (direction == Vector2.down)
        {
            orientation = Vector2.down;
            transform.localRotation = Quaternion.Euler(0, 0, 270);
        }
    }

    void ConsumePellet()
    {
        GameObject o = GetTileAtPosition(transform.position);
        if (o != null)
        {
            Tile tile = o.GetComponent<Tile>();

            if (tile != null)
            {
                bool didConsume = false;
                if (GameBoard.isPlayerOneUp)
                {
                    if(!tile.didConsumePlayerOne && (tile.isPellet || tile.isSuperPellet))
                    {
                        o.GetComponent<SpriteRenderer>().enabled = false;
                        didConsume = true;
                        tile.didConsumePlayerOne = true;

                        if (tile.isSuperPellet)
                        {
                            GameBoard.playerOneScore += 50;
                            Ghost.frightenedGhosts = 4;
                        }
                        else
                            GameBoard.playerOneScore += 10;

                        gameBoard.playerOnePelletsConsumed++;
                    }
                }
                else
                {
                    if(!tile.didConsumePlayerTwo && (tile.isPellet || tile.isSuperPellet))
                    {
                        o.GetComponent<SpriteRenderer>().enabled = false;
                        didConsume = true;
                        tile.didConsumePlayerTwo = true;

                        if (tile.isSuperPellet)
                        {
                            GameBoard.playerTwoScore += 50;
                            Ghost.frightenedGhosts = 4;
                        }
                        else
                            GameBoard.playerTwoScore += 10;

                        gameBoard.playerTwoPelletsConsumed++;
                    }
                }

                if (didConsume)
                {
                    PlayChompSound();
                    if (tile.isSuperPellet)
                    {
                        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
                        foreach (GameObject go in ghosts)
                        {
                            go.GetComponent<Ghost>().StartFrightenedMode();
                        }
                    }
                    gameBoard.BonusItems();
                    gameBoard.UpdateUI();
                }
            }
        }
    }

    public void ConsumedBonusItem(int playerNum, Tile bonusItem)
    {
        if (playerNum == 1)
        {
            GameBoard.playerOneScore += bonusItem.pointValue;
        }
        else
        {
            GameBoard.playerTwoScore += bonusItem.pointValue;
        }
        audio.PlayOneShot(eatFruit);
        StartCoroutine(gameBoard.ProcessConsumedBonusItem(bonusItem.gameObject, bonusItem.pointValue));
    }

    Node CanMove(Vector2 d)
    {
        Node moveToNode = null;
        for (int i = 0; i < currentNode.neighborsList.Count; i++)
        {
            if(currentNode.validDirections[i] == d)
            {
                if (currentNode.GetComponent<Tile>().isEntranceToGhostHouse && d == Vector2.down)
                    return moveToNode;
                moveToNode = currentNode.neighborsList[i];
                break;
            }
        }
        return moveToNode;
    }

    GameObject GetTileAtPosition (Vector2 pos)
    {
        int tileX = Mathf.RoundToInt(pos.x);
        int tileY = Mathf.RoundToInt(pos.y);

        GameObject tile = GameBoard.board[tileX, tileY];

        if (tile != null)
            return tile;

        return null;
    }

    Node GetNodeAtPosition (Vector2 pos)
    {
        GameObject tile = GameBoard.board[(int)pos.x, (int)pos.y];

        if (tile != null)
        {
            return tile.GetComponent<Node>();
        }
        return null;
    }

    bool OverShotTarget()
    {
        float nodeToTarget = LengthFromNode(targetNode.transform.position);
        float nodeToSelf = LengthFromNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }

    float LengthFromNode (Vector2 targetPosition)
    {
        Vector2 vec = targetPosition - (Vector2)previousNode.transform.position;
        return vec.sqrMagnitude;
    }

    GameObject GetPortal (Vector2 pos)
    {
        GameObject tile = GameBoard.board[(int)pos.x, (int)pos.y];
        
        if (tile != null)
        {
            if (tile.GetComponent<Tile>() != null)
            {
                if (tile.GetComponent<Tile>().isPortal)
                {
                    GameObject otherPortal = tile.GetComponent<Tile>().portalReceiver;
                    return otherPortal;
                }
            }
        }
        return null;
    }
}
