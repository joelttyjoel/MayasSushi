using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectRandomSpriteOnStart : MonoBehaviour
{
    public bool alsoChangeMask;
    public List<Sprite> spritesToChooseFrom;

    // Start is called before the first frame update
    void Start()
    {
        int randomNum = Random.Range(0, spritesToChooseFrom.Count);

        GetComponent<SpriteRenderer>().sprite = spritesToChooseFrom[randomNum];

        if(alsoChangeMask)
        {
            GetComponent<SpriteMask>().sprite = spritesToChooseFrom[randomNum];
        }
    }
}
