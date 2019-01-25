using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float moveSpeed = 5.9f;
    public float frightenedMoveSpeed = 2.9f;
    public float consumedMoveSpeed = 15f;
    public float normalMoveSpeed = 5.9f;

    public bool canMove = true;

    float previousMoveSpeed;

    Animator anim;
    GameObject game;
    AudioSource backgroudAudio;

    public int blinkyReleaseTimer = 3, pinkyReleaseTimer = 5, inkyReleaseTimer = 14, clydeReleaseTimer = 21;
    public float ghostReleaseTimer = 0f;

    public int frightenedModeDuration = 10;
    public int startBlinkingAt = 7;

    public bool isInGhostHouse = false;

    public Node startingPosition;
    public Node homeNode;
    public Node ghostHouse;

    public int scatterModeTimer1 = 7;
    public int chaseModeTimer1 = 20;
    public int scatterModeTimer2 = 7;
    public int chaseModeTimer2 = 20;
    public int scatterModeTimer3 = 5;
    public int chaseModeTimer3 = 20;
    public int scatterModeTimer4 = 5;

    int modeChangeIteration = 1;
    float modeChangeTimer = 0;

    float frightenedModeTimer = 0;

    public static int frightenedGhosts;

    public Sprite defaultSprite;

    public enum Mode
    {
        Chase,
        Scatter,
        Frightened,
        Consumed
    }

  

    Mode currentMode = Mode.Scatter;
    Mode previousMode;

    public enum GhostType
    {
        Red,
        Pink,
        Blue,
        Orange
    }

    public GhostType ghostType = GhostType.Red;

    private GameObject pacMan;

    private Node currentNode, targetNode, previousNode;
    private Vector2 direction, nextDirection;

    void Start()
    {
        if (GameBoard.isPlayerOneUp)
        {
            SetDifficultyForLevel(GameBoard.playerOneLevel);
        }
        else
        {
            SetDifficultyForLevel(GameBoard.playerTwoLevel);
        }


        anim = GetComponent<Animator>();
        game = GameObject.Find("Game");
        backgroudAudio = game.GetComponent<AudioSource>();
        pacMan = GameObject.FindGameObjectWithTag("PacMan");

        Node node = GetNodeAtPosition(transform.localPosition);
        if (node != null)
        {
            startingPosition = node;
            currentNode = node;
        }

        if (isInGhostHouse)
        {
            direction = Vector2.up;
            targetNode = currentNode.neighborsList[0];
        }
        else
        {
            direction = Vector2.left;
            targetNode = ChooseNextNode();
        }

        previousNode = currentNode;

    }

    public void SetDifficultyForLevel(int level)
    {

        if(level == 1)
        {
            scatterModeTimer1 = 7;
            scatterModeTimer2 = 7;
            scatterModeTimer3 = 5;
            scatterModeTimer4 = 5;

            chaseModeTimer1 = 20;
            chaseModeTimer2 = 20;
            chaseModeTimer3 = 20;

            frightenedModeDuration = 10;
            startBlinkingAt = 7;

            pinkyReleaseTimer = 5;
            inkyReleaseTimer = 14;
            clydeReleaseTimer = 21;
            blinkyReleaseTimer = 3;


            moveSpeed = 5.9f;
            normalMoveSpeed = 5.9f;
            frightenedMoveSpeed = 2.9f;
            consumedMoveSpeed = 15f;
        }
        else if (level == 2)
        {
            scatterModeTimer1 = 7;
            scatterModeTimer2 = 7;
            scatterModeTimer3 = 5;
            scatterModeTimer4 = 1;

            chaseModeTimer1 = 20;
            chaseModeTimer2 = 20;
            chaseModeTimer3 = 1033;

            frightenedModeDuration = 9;
            startBlinkingAt = 6;

            pinkyReleaseTimer = 4;
            inkyReleaseTimer = 12;
            clydeReleaseTimer = 18;
            blinkyReleaseTimer = 3;


            moveSpeed = 6.9f;
            normalMoveSpeed = 6.9f;
            frightenedMoveSpeed = 3.9f;
            consumedMoveSpeed = 18f;
        }
        else if (level == 3)
        {
            scatterModeTimer1 = 7;
            scatterModeTimer2 = 7;
            scatterModeTimer3 = 5;
            scatterModeTimer4 = 1;

            chaseModeTimer1 = 20;
            chaseModeTimer2 = 20;
            chaseModeTimer3 = 1033;

            frightenedModeDuration = 8;
            startBlinkingAt = 5;

            pinkyReleaseTimer = 3;
            inkyReleaseTimer = 10;
            clydeReleaseTimer = 15;
            blinkyReleaseTimer = 2;


            moveSpeed = 7.9f;
            normalMoveSpeed = 7.9f;
            frightenedMoveSpeed = 4.9f;
            consumedMoveSpeed = 20f;
        }
        else if (level == 4)
        {
            scatterModeTimer1 = 7;
            scatterModeTimer2 = 7;
            scatterModeTimer3 = 5;
            scatterModeTimer4 = 1;

            chaseModeTimer1 = 20;
            chaseModeTimer2 = 20;
            chaseModeTimer3 = 1033;

            frightenedModeDuration = 7;
            startBlinkingAt = 4;

            pinkyReleaseTimer = 2;
            inkyReleaseTimer = 8;
            clydeReleaseTimer = 13;
            blinkyReleaseTimer = 2;


            moveSpeed = 8.9f;
            normalMoveSpeed = 8.9f;
            frightenedMoveSpeed = 5.9f;
            consumedMoveSpeed = 22f;
        }
        else if (level > 4)
        {
            scatterModeTimer1 = 5;
            scatterModeTimer2 = 5;
            scatterModeTimer3 = 5;
            scatterModeTimer4 = 1;

            chaseModeTimer1 = 20;
            chaseModeTimer2 = 20;
            chaseModeTimer3 = 1037;

            frightenedModeDuration = 6;
            startBlinkingAt = 3;

            pinkyReleaseTimer = 2;
            inkyReleaseTimer = 6;
            clydeReleaseTimer = 10;
            blinkyReleaseTimer = 1;


            moveSpeed = 9.9f;
            normalMoveSpeed = 9.9f;
            frightenedMoveSpeed = 6.9f;
            consumedMoveSpeed = 24f;
        }

    }

    public void Restart()
    {
        canMove = true;

        currentMode = Mode.Scatter;

        moveSpeed = normalMoveSpeed;
        previousMode = 0;


        ghostReleaseTimer = 0;
        modeChangeIteration = 1;
        modeChangeTimer = 0;
        anim.SetBool("Frightened", false);
        anim.SetBool("Blinking", false);
        anim.SetBool("Eaten", false);
        anim.enabled = true;

        if (ghostType != GhostType.Red)
        {
            isInGhostHouse = true;
        }

        currentNode = startingPosition;

        if (isInGhostHouse)
        {
            direction = Vector2.up;
            targetNode = currentNode.neighborsList[0];
        }
        else
        {
            direction = Vector2.left;
            targetNode = ChooseNextNode();
        }
        previousNode = currentNode;

        UpdateAnimation();

    }

    public void MoveToStartingPosition()
    {
        transform.position = startingPosition.transform.position;
    }

    void Update()
    {
        if (canMove)
        {

            ModeUpdate();

            Move();

            ReleaseGhosts();

            CheckCollision();

            CheckIsInGhostHouse();
        }
    }

    void CheckIsInGhostHouse()
    {
        if (currentMode == Mode.Consumed)
        {
            GameObject tile = GetTileAtPosition(transform.position);
            if (tile != null)
            {
                if (tile.transform.GetComponent<Tile>() != null)
                {
                    if (tile.GetComponent<Tile>().isGhostHouse)
                    {
                        moveSpeed = normalMoveSpeed;

                        Node node = GetNodeAtPosition(transform.position);
                        if (node != null)
                        {
                            currentNode = node;
                            direction = Vector2.up;
                            targetNode = currentNode.neighborsList[0];

                            previousNode = currentNode;

                            currentMode = Mode.Chase;


                            anim.SetBool("Eaten", false);

                            ghostReleaseTimer = 0;
                            isInGhostHouse = true;
                            UpdateAnimation();
                        }
                    }
                }
            }
        }
    }

    void CheckCollision()
    {
        Rect ghostRect = new Rect(transform.position, transform.GetComponent<SpriteRenderer>().sprite.bounds.size / 4);
        Rect pacManRect = new Rect(pacMan.transform.position, pacMan.transform.GetComponent<SpriteRenderer>().sprite.bounds.size / 4);

        if(currentMode != Mode.Consumed)
        {
            if (ghostRect.Overlaps(pacManRect))
            {
                if(currentMode == Mode.Frightened)
                {
                    Consumed();
                }
                else
                {
                    game.GetComponent<GameBoard>().StartDeath();
                }
            }
        }
    }

    void Consumed()
    {
        if (GameMenu.isOnePlayerGame)
        {
            GameBoard.playerOneScore += GameBoard.ghostConsumedRunningScore;
        }
        else
        {
            if (GameBoard.isPlayerOneUp)
            {
                GameBoard.playerOneScore += GameBoard.ghostConsumedRunningScore; ;
            }
            else
            {
                GameBoard.playerTwoScore += GameBoard.ghostConsumedRunningScore; ;

            }
        }

        currentMode = Mode.Consumed;
        previousMoveSpeed = moveSpeed;
        moveSpeed = consumedMoveSpeed;
        anim.SetBool("Frightened", false);
        anim.SetBool("Blinking", false);
        UpdateAnimation();

        game.GetComponent<GameBoard>().StartConsumed(this);

        GameBoard.ghostConsumedRunningScore *= 2;
        frightenedGhosts--;

        if (frightenedGhosts == 0)
            EndFrightenedMode();
    }

    void ModeUpdate()
    {
        if(currentMode != Mode.Frightened)
        {
            modeChangeTimer += Time.deltaTime;

            if (modeChangeIteration == 1)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer1)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer1)
                {
                    modeChangeIteration = 2;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }
            else if (modeChangeIteration == 2)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer2)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer2)
                {
                    modeChangeIteration = 3;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }
            else if (modeChangeIteration == 3)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer3)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer3)
                {
                    modeChangeIteration = 4;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }
            else if (modeChangeIteration == 4)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer4)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

            }
        }
        else if (currentMode == Mode.Frightened)
        {
            anim.SetBool("Frightened", true );
            frightenedModeTimer += Time.deltaTime;

            if(frightenedModeTimer >= startBlinkingAt)
            {
                anim.SetBool("Blinking", true);
            }

            if (frightenedModeTimer >= frightenedModeDuration)
            {
                EndFrightenedMode();
                ChangeMode(previousMode);
            }
        }
    }

    void EndFrightenedMode()
    {
        backgroudAudio.clip = game.GetComponent<GameBoard>().backgroundAudioNormal;
        backgroudAudio.Play();
        frightenedGhosts = 0;

        anim.SetBool("Frightened", false);
        anim.SetBool("Blinking", false);
        frightenedModeTimer = 0;

        GameBoard.ghostConsumedRunningScore = 200;
    }

    void Move()
    {
        if(targetNode != currentNode && targetNode != null && !isInGhostHouse)
        {
            if (OverShotTarget())
            {
                currentNode = targetNode;
                transform.localPosition = currentNode.transform.position;

                GameObject otherPortal = GetPortal(currentNode.transform.position);
                if(otherPortal != null)
                {
                    transform.localPosition = otherPortal.transform.position;
                    currentNode = otherPortal.GetComponent<Node>();
                }

                targetNode = ChooseNextNode();
                previousNode = currentNode;
                currentNode = null;

                UpdateAnimation();
            }
            else
            {
                transform.localPosition += (Vector3)direction * moveSpeed * Time.deltaTime;
            }
        }

        
    }

    void UpdateAnimation()
    {
        if (currentMode != Mode.Frightened)
        {
            if (currentMode == Mode.Consumed)
            {
                anim.SetBool("Eaten", true);
            }

            anim.SetInteger("xDir", (int)direction.x);
            anim.SetInteger("yDir", (int)direction.y);

        }
        else
        {
            anim.SetInteger("yDir", 0);
            anim.SetInteger("xDir", 0);
        }
    }

    void ChangeMode (Mode m)
    {

        if(currentMode == Mode.Frightened)
        {
            moveSpeed = previousMoveSpeed;
        }

        if(m == Mode.Frightened)
        {
            previousMoveSpeed = moveSpeed;
            moveSpeed = frightenedMoveSpeed;
        }

        if(currentMode != m)
        {
            previousMode = currentMode;
            currentMode = m;

        }

        UpdateAnimation();
    }

    public void StartFrightenedMode()
    {
        if(currentMode != Mode.Consumed)
        {
            frightenedModeTimer = 0;
            anim.SetBool("Blinking", false);
            ChangeMode(Mode.Frightened);
            backgroudAudio.clip = game.GetComponent<GameBoard>().backgroundFrightened;
            backgroudAudio.Play();
        }
    }

    Vector2 GetRedGhostTargetTile()
    {
        Vector2 pacManPosition = pacMan.transform.localPosition;
        Vector2 targetTile = new Vector2(Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y));
        return targetTile;
    }

    Vector2 GetPinkGhostTargetTile()
    {
        //Four tiles ahead of Pac-Man
        //Taking account position and Orientation
        Vector2 pacManPosition = pacMan.transform.localPosition;
        Vector2 pacManOrientation = pacMan.GetComponent<PacMan>().orientation;

        int pacManPositionX = Mathf.RoundToInt(pacManPosition.x);
        int pacManPositionY = Mathf.RoundToInt(pacManPosition.y);

        Vector2 pacManTile = new Vector2(pacManPositionX, pacManPositionY);
        Vector2 targetTile = pacManTile + (4 * pacManOrientation);

        return targetTile;
    }

    Vector2 GetBlueGhostTargetTile()
    {
        // Select the position two tiles in front of Pac-Man
        // Draw a vector from Blinky to that position
        // Double the length of the vector
        Vector2 pacManPosition = pacMan.transform.localPosition;
        Vector2 paManOrientation = pacMan.GetComponent<PacMan>().orientation;

        int pacManPositionX = Mathf.RoundToInt(pacManPosition.x);
        int pacManPositionY = Mathf.RoundToInt(pacManPosition.y);

        Vector2 pacManTile = new Vector2(pacManPositionX, pacManPositionY);

        Vector2 targetTile = pacManTile + (2 * pacManPosition);

        //Temporary Blinky Position
        Vector2 tempBlinkyPosition = GameObject.Find("Blinky").transform.localPosition;
        int blinkyPositionX = Mathf.RoundToInt(tempBlinkyPosition.x);
        int blinkyPositionY = Mathf.RoundToInt(tempBlinkyPosition.y);

        tempBlinkyPosition = new Vector2(blinkyPositionX, blinkyPositionY);

        float distance = GetDistance(tempBlinkyPosition, targetTile);
        distance *= 2;

        targetTile = new Vector2(tempBlinkyPosition.x + distance, tempBlinkyPosition.y + distance);

        return targetTile;

    }

    Vector2 GetOrangeGhostTargetTile()
    {
        //Calculate the distance from Pac-Man
        // If the distance is greater than eight tiles targeting is the same as blinky
        // If the distance is less than eight tiles, then target is his home node, so same as scatter mode

        Vector2 pacManPosition = pacMan.transform.localPosition;

        float distance = GetDistance(transform.localPosition, pacManPosition);
        Vector2 targetTile = Vector2.zero;

        if(distance >= 8)
        {
            targetTile = new Vector2(Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y));
        }
        else if (distance < 8)
        {
            targetTile = homeNode.transform.position;
        }

        return targetTile;
    }

    Vector2 GetTargetTile()
    {
        Vector2 targetTile = Vector2.zero;

        if(ghostType == GhostType.Red)
        {
            targetTile = GetRedGhostTargetTile();
        }
        else if (ghostType == GhostType.Pink)
        {
            targetTile = GetPinkGhostTargetTile();
        }
        else if (ghostType == GhostType.Blue)
        {
            targetTile = GetBlueGhostTargetTile();
        }
        else if (ghostType == GhostType.Orange)
        {
            targetTile = GetOrangeGhostTargetTile();
        }
        return targetTile;
    }

    Vector2 GetRandomTile()
    {
        int x = Random.Range(0, 28);
        int y = Random.Range(0, 36);

        return new Vector2(x, y);
    }

    GameObject GetTileAtPosition (Vector2 pos)
    {
        int tileX = Mathf.RoundToInt(pos.x);
        int tileY = Mathf.RoundToInt(pos.y);

        GameObject tile = GameBoard.board[tileX, tileY];
        if (tile != null)
        {
            return tile;
        }
        return null;
    }

    void ReleasePinkGhost()
    {
        if (ghostType == GhostType.Pink && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }

    void ReleaseBlueGhost()
    {
        if (ghostType == GhostType.Orange && isInGhostHouse)
            isInGhostHouse = false;
    }

    void ReleaseOrangeGhost()
    {
        if (ghostType == GhostType.Blue && isInGhostHouse)
            isInGhostHouse = false;
    }

    void ReleaseRedGhost()
    {
        if (ghostType == GhostType.Red && isInGhostHouse)
            isInGhostHouse = false;
    }

    void ReleaseGhosts()
    {
        ghostReleaseTimer += Time.deltaTime;

        if (ghostReleaseTimer > pinkyReleaseTimer)
            ReleasePinkGhost();

        if (ghostReleaseTimer > inkyReleaseTimer)
            ReleaseBlueGhost();

        if (ghostReleaseTimer > clydeReleaseTimer)
            ReleaseOrangeGhost();

        if (ghostReleaseTimer > blinkyReleaseTimer)
            ReleaseRedGhost();
    }

    Node ChooseNextNode()
    {
        Vector2 targetTile = Vector2.zero;

        if (currentMode == Mode.Chase)
        {
            targetTile = GetTargetTile();
        }
        else if (currentMode == Mode.Scatter)
        {
            targetTile = homeNode.transform.position;
        }
        else if(currentMode == Mode.Frightened)
        {
            targetTile = GetRandomTile();
        }
        else if(currentMode == Mode.Consumed)
        {
            targetTile = ghostHouse.transform.position;
        }

        Node moveToNode = null;

        Node[] foundNodes = new Node[4];
        Vector2[] foundNodesDirection = new Vector2[4];

        int nodeCounter = 0;

        for (int i = 0; i < currentNode.neighborsList.Count; i++)
        {
            if(currentNode.validDirections [i] != -direction)
            {
                if(currentMode != Mode.Consumed)
                {
                    GameObject tile = GetTileAtPosition(currentNode.transform.position);
                    if(tile.transform.GetComponent<Tile>().isEntranceToGhostHouse == true)
                    {
                        if(currentNode.validDirections[i] != Vector2.down)
                        {
                            foundNodes[nodeCounter] = currentNode.neighborsList[i];
                            foundNodesDirection[nodeCounter] = currentNode.validDirections[i];
                            nodeCounter++;
                        }
                    }
                    else
                    {
                        foundNodes[nodeCounter] = currentNode.neighborsList[i];
                        foundNodesDirection[nodeCounter] = currentNode.validDirections[i];
                        nodeCounter++;
                    }
                }
                else
                {
                    foundNodes[nodeCounter] = currentNode.neighborsList[i];
                    foundNodesDirection[nodeCounter] = currentNode.validDirections[i];
                    nodeCounter++;                   
                }
            }            
        }

        if (foundNodes.Length == 1)
        {
            moveToNode = foundNodes[0];
            direction = foundNodesDirection[0];
        }
        if (foundNodes.Length > 1)
        {
            float leastDistance = 100000f;

            for (int i = 0; i < foundNodes.Length; i++)
            {
                if(foundNodesDirection[i] != Vector2.zero)
                {
                    float distance = GetDistance(foundNodes[i].transform.position, targetTile);

                    if (distance < leastDistance)
                    {
                        leastDistance = distance;
                        moveToNode = foundNodes[i];
                        direction = foundNodesDirection[i];
                    }
                }
            }
        }
        return moveToNode;
    }

    Node GetNodeAtPosition (Vector2 pos)
    {
        GameObject tile = GameBoard.board[(int)pos.x, (int)pos.y];
        if (tile != null)
        {
            if(tile.GetComponent<Node>() != null)
            {
                return tile.GetComponent<Node>();
            }
        }
        return null;
    }

    GameObject GetPortal (Vector2 pos)
    {
        GameObject tile = GameBoard.board [(int)pos.x, (int)pos.y];
        if(tile != null)
        {
            if (tile.GetComponent<Tile>().isPortal)
            {
                GameObject otherPortal = tile.GetComponent<Tile>().portalReceiver;
                return otherPortal;
            }
        }
        return null;
    }

    float LengthFromNode(Vector2 targetPosition)
    {
        Vector2 vec = targetPosition - (Vector2)previousNode.transform.position;
        return vec.sqrMagnitude;
    }

    bool OverShotTarget()
    {
        float nodeToTarget = LengthFromNode(targetNode.transform.position);
        float nodeToSelf = LengthFromNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }

    float GetDistance (Vector2 posA, Vector2 posB)
    {
        float dx = posA.x - posB.x;
        float dy = posA.y - posB.y;

        float distance = Mathf.Sqrt(dx * dx + dy * dy);
        return distance;
    }
}
