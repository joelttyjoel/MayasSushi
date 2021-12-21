using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    public class Piece
    {
        public GameObject gameObject;
        public int indexPoints;

        public Piece(GameObject gameObjectIn, int indexPointsIn)
        {
            gameObject = gameObjectIn;
            indexPoints = indexPointsIn;
        }
    }

    public Material particleMaterial;
    public Sprite[] plateSprites0to3 = new Sprite[4];
    public Sprite[] dragonSprites = new Sprite[5];
    public Sprite[] rainbowSprites = new Sprite[5];
    public GameObject pieceObjectTemplate;
    public float rangeForNextIndex;
    public float speed;
    public Vector3 scaleFront;
    public Vector3 scaleBack;
    public int layerFront;
    public int layerBack;
    public List<GameObject> placesToPlaceItems;
    public LayerMask movingItems;
    public float timeBeforeRemovePiecesAfterAnimationStart;
    public ParticleSystem particles1;
    public ParticleSystem particles2;
    [Header("Starts on index 0, loops from last to first")]
    public List<Vector3> conveyorPoints;

    private List<Piece> pieces;

    private static ConveyorController _instance;
    public static ConveyorController Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        pieces = new List<Piece>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Piece a in pieces)
        {
            //move towards next index in list, if closer than X, make next index
            //https://docs.unity3d.com/ScriptReference/Vector3.MoveTowards.html
            // Move our position a step closer to the target.
            float step = speed * Time.deltaTime; // calculate distance to move
            a.gameObject.transform.position = Vector3.MoveTowards(a.gameObject.transform.position, conveyorPoints[a.indexPoints], step);

            // check
            if (Vector3.Distance(a.gameObject.transform.position, conveyorPoints[a.indexPoints]) < rangeForNextIndex)
            {
                // Swap the position of the cylinder.
                if(a.indexPoints >= conveyorPoints.Count - 1)
                {
                    a.indexPoints = 0;
                }
                else
                {
                    a.indexPoints++;
                }

                if (a.indexPoints == 2)
                {
                    a.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                    a.gameObject.transform.localScale = scaleBack;
                    a.gameObject.GetComponent<SpriteRenderer>().sortingOrder = layerBack;
                }
                else if (a.indexPoints == 3)
                {
                    a.gameObject.GetComponent<SpriteRenderer>().flipX = true;
                    a.gameObject.transform.localScale = scaleFront;
                    a.gameObject.GetComponent<SpriteRenderer>().sortingOrder = layerFront;
                }
            }
        }
    }

    public void RemovePiece(GameObject pieceToRemove)
    {
        foreach(Piece a in pieces)
        {
            if(a.gameObject.GetInstanceID() == pieceToRemove.GetInstanceID())
            {
                Destroy(a.gameObject);
                pieces.Remove(a);
                break;
            }
        }
    }

    public bool AddPiece(BoardManager.PieceToCreate PieceToCreate)
    {
        //check if is special piece
        if (PieceToCreate.pieceLevel0to3 > 1) return AddSpecialPiece(PieceToCreate);

        //check if can place, if can return true, else false
        foreach(GameObject a in placesToPlaceItems)
        {
            if (!a.GetComponent<BoxCollider2D>().IsTouchingLayers(movingItems))
            {
                GameObject temp = GameObject.Instantiate(pieceObjectTemplate);
                temp.GetComponent<SpriteRenderer>().sprite = PieceToCreate.sprite;
                temp.transform.position = a.transform.position;
                temp.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = plateSprites0to3[PieceToCreate.pieceLevel0to3];
                temp.transform.localScale = scaleFront;
                Piece tempPiece = new Piece(temp, 0);
                pieces.Add(tempPiece);

                //play correct particlesystem
                particleMaterial.SetTexture("_MainTex", PieceToCreate.sprite.texture);
                a.transform.GetChild(0).GetComponent<ParticleSystem>().Play();

                return true;
            }
        }
        return false;
    }

    private bool AddSpecialPiece(BoardManager.PieceToCreate PieceToCreate)
    {
        //for now all these have 5 pieces
        bool canPlace = true;
        foreach (GameObject a in placesToPlaceItems)
        {
            if (a.GetComponent<BoxCollider2D>().IsTouchingLayers(movingItems))
            {
                canPlace = false;
                return false;
            }
        }

        if(canPlace)
        {
            //decide array of sprites to use
            Sprite[] spritesToUse = new Sprite[5];

            if (PieceToCreate.sprite == dragonSprites[3]) spritesToUse = dragonSprites;
            else spritesToUse = rainbowSprites;

            int index = 0;
            foreach (GameObject a in placesToPlaceItems)
            {
                GameObject temp = GameObject.Instantiate(pieceObjectTemplate);
                temp.GetComponent<SpriteRenderer>().sprite = spritesToUse[index];
                temp.transform.position = a.transform.position;
                temp.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = plateSprites0to3[PieceToCreate.pieceLevel0to3];
                Piece tempPiece = new Piece(temp, 0);
                pieces.Add(tempPiece);

                //play correct particlesystem
                particleMaterial.SetTexture("_MainTex", PieceToCreate.sprite.texture);
                a.transform.GetChild(0).GetComponent<ParticleSystem>().Play();

                index++;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public void ClearWholeConveyor()
    {
        StartCoroutine(ClearWholeBoardAnimation());
    }

    IEnumerator ClearWholeBoardAnimation()
    {
        particles1.Play();
        particles2.Play();
        yield return new WaitForSeconds(timeBeforeRemovePiecesAfterAnimationStart);

        particles1.Stop();
        particles2.Stop();

        yield return new WaitForSeconds(0.6f);

        while (pieces.Count > 0)
        {
            Destroy(pieces[0].gameObject);
            pieces.RemoveAt(0);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(conveyorPoints[0], 0.2f);

        Gizmos.color = Color.red;
        for (int i = 1; i < conveyorPoints.Count; i++)
        {
            Gizmos.DrawSphere(conveyorPoints[i], 0.1f);
            Gizmos.DrawLine(conveyorPoints[i], conveyorPoints[i - 1]);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(conveyorPoints[conveyorPoints.Count - 1], 0.2f);
    }
}
