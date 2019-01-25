using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraseCollider : MonoBehaviour
{

    void Start()
    {
        Invoke("Remove", 1);
    }
    void Remove()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<BoxCollider2D>().enabled = false;

        }
    }
}
