using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveEmptyPlates : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<SpriteRenderer>().sprite == null)
        {
            ConveyorController.Instance.RemovePiece(collision.gameObject);
        }
    }
}
