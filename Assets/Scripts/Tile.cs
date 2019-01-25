using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isPortal;

    public bool isPellet;
    public bool isSuperPellet;
    public bool didConsumePlayerOne, didConsumePlayerTwo;
    public bool isEntranceToGhostHouse;
    public bool isGhostHouse;

    public bool isBonusItem;
    public int pointValue;

    public GameObject portalReceiver;
}
