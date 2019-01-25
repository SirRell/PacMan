using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neighborsList = new List<Node>();
    public List<Vector2> validDirections = new List<Vector2>();

    void Start()
    {

        RaycastHit2D upHit = Physics2D.Raycast(transform.position + Vector3.up * .5f, Vector2.up);
        if(upHit != false)
        {
        if (upHit.transform.CompareTag("Node"))
            {
                neighborsList.Add(upHit.transform.gameObject.GetComponent<Node>());
                validDirections.Add(Vector2.up);
            }
        }

        RaycastHit2D downHit = Physics2D.Raycast(transform.position + Vector3.down * .5f, Vector2.down);
        if(downHit != false)
        {
            if (downHit.transform.CompareTag("Node"))
            {
                neighborsList.Add(downHit.transform.gameObject.GetComponent<Node>());
                validDirections.Add(Vector2.down);
            }

        }

        RaycastHit2D leftHit = Physics2D.Raycast(transform.position + Vector3.left * .5f, Vector2.left);
        if(leftHit != false)
        {
            if (leftHit.transform.CompareTag("Node"))
            {
                neighborsList.Add(leftHit.transform.gameObject.GetComponent<Node>());
                validDirections.Add(Vector2.left);
            }

        }

        RaycastHit2D rightHit = Physics2D.Raycast(transform.position + Vector3.right * .5f, Vector2.right);
        if(rightHit != false)
        {
            if (rightHit.transform.CompareTag("Node"))
            {
                neighborsList.Add(rightHit.transform.gameObject.GetComponent<Node>());
                validDirections.Add(Vector2.right);
            }
        }
       
    }
}
