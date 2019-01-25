using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusItem : MonoBehaviour
{
    float randomLifeExpectancy;
    int playerNum;

    GameObject pacMan;

    private void Start()
    {
        name = "bonusitem";
        randomLifeExpectancy = Random.Range(9f, 10f);

        pacMan = GameObject.Find("PacMan");

        if (GameBoard.isPlayerOneUp)
            playerNum = 1;
        else
            playerNum = 2;        
    }

    void Update()
    {
        randomLifeExpectancy -= Time.deltaTime;

        if(randomLifeExpectancy <= 0f)
        {
            Destroy(gameObject);
        }

        Rect ghostRect = new Rect(transform.position, transform.GetComponent<SpriteRenderer>().sprite.bounds.size / 4);
        Rect pacManRect = new Rect(pacMan.transform.position, pacMan.transform.GetComponent<SpriteRenderer>().sprite.bounds.size / 4);

        if (ghostRect.Overlaps(pacManRect))
        {
            pacMan.GetComponent<PacMan>().ConsumedBonusItem(playerNum, GetComponent<Tile>());   
        }
        
    }
}
