using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public LayerMask layerMaskBoard;
    public BoxCollider2D boardArea;
    public LayerMask layerMaskTiles;
    public LayerMask layerMaskCustomers;
    public bool canInteract;

    private List<GameObject> clickedObjects;
    private LineRenderer lineBetweenTiles;

    private static InteractionManager _instance;
    public static InteractionManager Instance { get { return _instance; } }

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

    // Start is called before the first frame update
    void Start()
    {
        lineBetweenTiles = GetComponent<LineRenderer>();

        clickedObjects = new List<GameObject>();
        canInteract = true;

        lineBetweenTiles.positionCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canInteract) return;

        //push
        if(Input.GetMouseButton(0))
        {
            if (clickedObjects.Count >= BoardManager.Instance.maxLengthSelected) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            //draw first line point on mouse pos
            if(lineBetweenTiles.positionCount > 0)
            {
                lineBetweenTiles.SetPosition(lineBetweenTiles.positionCount - 1, new Vector3(mousePos2D.x, mousePos2D.y, -1f));
            }

            //first check if inside board area
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 999999f, layerMaskBoard);
            //if hits border of board
            if (hit.collider == null)
            {
                StopClick();

                return;
            }

            //then check objects clicked
            hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 999999f, layerMaskTiles);
            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;

                bool isAlreadyClicked = false;
                foreach(GameObject a in clickedObjects)
                {
                    if(hitObject == a)
                    {
                        isAlreadyClicked = true;
                    }
                }

                if(!isAlreadyClicked)
                {
                    //check if is sidepiece
                    if (clickedObjects.Count > 0)
                    {
                        Vector3Int clickedPosition3 = clickedObjects[clickedObjects.Count - 1].gameObject.GetComponent<TileManager>().positionGrid;
                        Vector2Int clickedPosition = new Vector2Int(clickedPosition3.x, clickedPosition3.y);

                        //first check if is side piece, if all those fail, check diagonals, if all those fail, stop
                        //left
                        if (hitObject == BoardManager.Instance.ReturnTileObjectFromPosition(new Vector2Int(clickedPosition.x - 1, clickedPosition.y)))
                        {
                            //this is dumb but idk
                        }
                        //top
                        else if (hitObject == BoardManager.Instance.ReturnTileObjectFromPosition(new Vector2Int(clickedPosition.x, clickedPosition.y + 1)))
                        {

                        }
                        //right
                        else if (hitObject == BoardManager.Instance.ReturnTileObjectFromPosition(new Vector2Int(clickedPosition.x + 1, clickedPosition.y)))
                        {

                        }
                        //bot
                        else if (hitObject == BoardManager.Instance.ReturnTileObjectFromPosition(new Vector2Int(clickedPosition.x, clickedPosition.y - 1)))
                        {

                        }

                        else
                        {
                            StopClick();
                        }
                    }

                    clickedObjects.Add(hitObject);
                    hit.collider.GetComponent<TileManager>().SetSelected(true);

                    //remove nasty last one for mouse
                    if (lineBetweenTiles.positionCount > 0)
                    {
                        lineBetweenTiles.positionCount = lineBetweenTiles.positionCount - 1;
                    }

                    //draw line
                    lineBetweenTiles.positionCount = lineBetweenTiles.positionCount + 3;

                    lineBetweenTiles.SetPosition(lineBetweenTiles.positionCount - 1, new Vector3(hitObject.transform.position.x, hitObject.transform.position.y, -1f));
                    lineBetweenTiles.SetPosition(lineBetweenTiles.positionCount - 2, new Vector3(hitObject.transform.position.x, hitObject.transform.position.y, -1f));
                    lineBetweenTiles.SetPosition(lineBetweenTiles.positionCount - 3, new Vector3(hitObject.transform.position.x, hitObject.transform.position.y, -1f));

                    //add final point for mouse to mess aorund with
                    if(clickedObjects.Count < BoardManager.Instance.maxLengthSelected)
                    {
                        lineBetweenTiles.positionCount = lineBetweenTiles.positionCount + 1;
                        lineBetweenTiles.SetPosition(lineBetweenTiles.positionCount - 1, new Vector3(mousePos2D.x, mousePos2D.y, -1f));
                    }
                }
            }
        }

        //release
        if(Input.GetMouseButtonUp(0))
        {
            BoardManager.Instance.CheckSelection(clickedObjects);

            StopClick();
        }
    }
    
    public RaycastHit2D CheckIfHitObject(LayerMask LayerMaskIn)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        //then check objects clicked
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 999999f, LayerMaskIn);

        return hit;
    }

    private void StopClick()
    {
        foreach (GameObject a in clickedObjects)
        {
            a.GetComponent<TileManager>().SetSelected(false);
        }
        clickedObjects.Clear();

        lineBetweenTiles.positionCount = 0;
    }
}
