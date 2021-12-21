using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnConveyorController : MonoBehaviour
{
    public bool faceLeft;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //is going Left
        if (faceLeft)
        {
            //starts as left so do nothing
        }
        else
        {
            //flip if going right
            spriteRenderer.flipX = true;
        }
    }
}
