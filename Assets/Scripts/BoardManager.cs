using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [System.Serializable]
    public struct PieceToCreate
    {
        public int pieceLevel0to3;
        public Sprite sprite;
        public List<GameObject> ingredients;

        public PieceToCreate(int inLevel, Sprite InSprite, List<GameObject> InIngredients)
        {
            pieceLevel0to3 = inLevel;
            sprite = InSprite;
            ingredients = InIngredients;
        }
    }

    public struct PieceOnBoard
    {
        public Vector3Int positionGrid;
        public GameObject gameObject;

        public PieceOnBoard(Vector3Int InPositionGrid, GameObject InGameObject)
        {
            positionGrid = InPositionGrid;
            gameObject = InGameObject;
        }
    }

    public float fallingTimePerTile;
    public AnimationCurve fallingAnimation;
    public GameObject gridObject;
    public int sizeX;
    public int sizeY;
    public int minLengthSelected;
    public int maxLengthSelected;
    public List<PieceToCreate> piecesToCreate;
    public List<GameObject> ingredients;
    public GameObject poofAnimation;
    public Animation knifeAnimation;
    public float timeBeforeRemovePiecesAfterKnifeAnimation;

    private List<int> ingredientsPercentages;
    private Grid grid;
    private PieceOnBoard[,] piecesOnBoard;
    private List<int> ingredientsPercentageList;
    private float distanceYPerTile;

    private static BoardManager _instance;
    public static BoardManager Instance { get { return _instance; } }

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
        LoadSettingsStart();

        grid = gridObject.GetComponent<Grid>();
        piecesOnBoard = new PieceOnBoard[sizeX, sizeY];

        //this is a 100 slot list used to select random item from list
        ingredientsPercentageList = new List<int>();
        for (int i = 0; i < ingredientsPercentages.Count; i++)
        {
            for (int k = 0; k < ingredientsPercentages[i]; k++)
            {
                ingredientsPercentageList.Add(i);
            }
        }

        //SetNewPieceToCreate();

        FillBoardFullStart();

        distanceYPerTile = Vector3.Distance(grid.GetCellCenterWorld(new Vector3Int(0, 0, 0)), grid.GetCellCenterWorld(new Vector3Int(0, 1, 0)));
    }
    
    private void LoadSettingsStart()
    {
        ingredientsPercentages = LevelController.Instance.levelInformation.levels[LevelController.Instance.currentLevel].boardPercentages;
    }

    private void FillBoardFullStart()
    {
        for (int i = 0; i < sizeX; i++)
        {
            for (int k = 0; k < sizeY; k++)
            {
                //ngl this part kinda smart
                int randomIndex = Random.Range(0, 100);
                GameObject temp = Instantiate(ingredients[ingredientsPercentageList[randomIndex]]);
                temp.transform.SetParent(transform);

                Vector3Int gridPos = new Vector3Int(i, k, 0);

                temp.GetComponent<TileManager>().positionGrid = gridPos;

                temp.GetComponent<TileManager>().wasInstantiatedFrom = ingredients[ingredientsPercentageList[randomIndex]];

                temp.transform.position = grid.GetCellCenterWorld(gridPos);

                piecesOnBoard[i, k] = new PieceOnBoard(gridPos, temp);
            }
        }
    }

    private void FillBoard(List<GameObject> clickedObjects)
    {
        List<TileManager> clickedTiles = new List<TileManager>();
        foreach (GameObject a in clickedObjects)
        {
            clickedTiles.Add(a.GetComponent<TileManager>());
        }

        //move pieces down, move towards point using animation curve
        MakePiecesFallToLowest(clickedTiles);

        //now fill up empty slots created ontop
        for (int k = 0; k < sizeX; k++)
        {
            //instead of list of all things etc to know how to invert positions, keep track of largest null count per column
            int largestNullCount = 0;

            for (int i = 0; i < sizeY; i++)
            {
                //if space is empty after all has moved, add new piece there
                if (piecesOnBoard[k, i].gameObject == null)
                {
                    //ngl this part kinda smart
                    int randomIndex = Random.Range(0, 99);
                    GameObject temp = Instantiate(ingredients[ingredientsPercentageList[randomIndex]]);
                    temp.transform.SetParent(transform);

                    //set end state stuff before animation

                    Vector3Int gridPos = new Vector3Int(k, i, 0);

                    temp.GetComponent<TileManager>().positionGrid = gridPos;

                    temp.GetComponent<TileManager>().wasInstantiatedFrom = ingredients[ingredientsPercentageList[randomIndex]];

                    piecesOnBoard[k, i] = new PieceOnBoard(gridPos, temp);

                    //set position above and start animation top fall down to start state

                    //check how many null there are in current lane, as each lane fills up each will find less nulls making it go less high
                    int emptyCount = 0;
                    for (int j = sizeY - 1; j > -1; j--)
                    {
                        if (piecesOnBoard[k, j].gameObject == null)
                        {
                            emptyCount++;
                        }
                    }

                    if (emptyCount > largestNullCount) largestNullCount = emptyCount;

                    //Debug.Log(largestNullCount);
                    //Debug.Log(emptyCount);

                    //since current pieces null has already been filled, final to be filled in each collumn will find 0 empty count, making it start with no extra offset to first piece
                    temp.transform.position = grid.GetCellCenterWorld(new Vector3Int(k, sizeY, 0)) +
                        new Vector3(0, distanceYPerTile * (largestNullCount - emptyCount), 0);


                    //new Vector3(grid.GetCellCenterWorld(gridPos).x
                    //, grid.GetCellCenterWorld(new Vector3Int(k, sizeY, 0)).y + (distanceYPerTile * arrayOfMovesForEachTile[k, i])
                    //    , 0);

                    temp.GetComponent<TileManager>().StartFallingAnimation(fallingAnimation, fallingTimePerTile, grid.GetCellCenterWorld(gridPos));
                }
            }
        }
    }

    public void FillBoardOnlyRice(List<GameObject> clickedObjects)
    {
        foreach (GameObject a in clickedObjects)
        {
            //only rice
            GameObject temp = Instantiate(ingredients[0]);
            temp.transform.SetParent(transform);

            Vector3Int gridPos = new Vector3Int(a.GetComponent<TileManager>().positionGrid.x, a.GetComponent<TileManager>().positionGrid.y, 0);

            temp.GetComponent<TileManager>().positionGrid = gridPos;

            temp.GetComponent<TileManager>().wasInstantiatedFrom = ingredients[ingredientsPercentageList[0]];

            piecesOnBoard[gridPos.x, gridPos.y] = new PieceOnBoard(gridPos, temp);

            temp.transform.position = grid.GetCellCenterWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
        }
    }

    private void MakePiecesFallToLowest(List<TileManager> clickedTiles)
    {
        //go through all tiles, check how many tiles under it are empty, move tile down that many tiles
        int[,] arrayOfMovesForEachTile = new int[sizeX, sizeY];

        //start on one because will never need lowest piece to fall
        for (int k = 1; k < sizeY; k++)
        {
            for (int i = 0; i < sizeX; i++)
            {
                //if no piece, will move 0 pieces
                if (piecesOnBoard[i, k].gameObject == null) continue;

                int emptyCount = 0;
                //go through all colums of pieces on board under current piece, if empty add empty count by one
                //lets say k = 2, check row 1 and 0 if have empty slots
                for (int j = k - 1; j > -1; j--)
                {
                    if (piecesOnBoard[i, j].gameObject == null)
                    {
                        //Debug.Log(i + ", " + j);
                        emptyCount++;
                    }
                }

                arrayOfMovesForEachTile[i, k] = emptyCount;
            }
        }

        for (int k = 0; k < sizeY; k++)
        {
            for (int i = 0; i < sizeX; i++)
            {
                //if tile is empty or tile has no moves, skip this and dont move
                if (piecesOnBoard[i, k].gameObject == null || arrayOfMovesForEachTile[i, k] == 0) continue;
                //for each tile, move to world position of self grid pos - emptycount under self
                //piecesOnBoard[i, k].gameObject.transform.position = grid.GetCellCenterWorld(piecesOnBoard[i, k].positionGrid - new Vector3Int(0, arrayOfMovesForEachTile[i, k], 0));

                //Incase time needs to be relative to falling distance
                float timeForFall = fallingTimePerTile;

                //start move animation for each tile
                piecesOnBoard[i, k].gameObject.GetComponent<TileManager>().StartFallingAnimation(fallingAnimation, timeForFall, grid.GetCellCenterWorld(piecesOnBoard[i, k].positionGrid - new Vector3Int(0, arrayOfMovesForEachTile[i, k], 0)));

                //if tile is empty or tile has no moves, skip this and dont move
                if (piecesOnBoard[i, k].gameObject == null) continue;
                //move objects position
                piecesOnBoard[i, k].gameObject.GetComponent<TileManager>().positionGrid = new Vector3Int(i, k - arrayOfMovesForEachTile[i, k], 0);
                //move in local grid array
                piecesOnBoard[i, k].positionGrid = new Vector3Int(i, k - arrayOfMovesForEachTile[i, k], 0);
                piecesOnBoard[i, k - arrayOfMovesForEachTile[i, k]] = piecesOnBoard[i, k];
                piecesOnBoard[i, k] = new PieceOnBoard(Vector3Int.zero, null);
            }
        }

        //for (int i = 0; i < sizeX; i++)
        //{
        //    string line = "";
        //    for (int k = 0; k < sizeY; k++)
        //    {
        //        if (piecesOnBoard[i, k].gameObject == null)
        //        {
        //            line += " null ";
        //        }
        //        else
        //        {
        //            line += piecesOnBoard[i, k].gameObject.name;
        //        }
        //        line += piecesOnBoard[i, k].positionGrid;
        //        line += " ";
        //    }
        //    Debug.Log(line);
        //    Debug.Log("\n");
        //}
    }

    public void ClearBoardOfSelected(List<GameObject> clickedObjects)
    {
        foreach (GameObject a in clickedObjects)
        {
            //Debug.Log("Destroy" + a.name + " at pos:" + a.GetComponent<TileManager>().positionGrid);

            //spawn poof
            GameObject.Instantiate(poofAnimation, grid.GetCellCenterWorld(a.GetComponent<TileManager>().positionGrid), Quaternion.identity, transform);

            piecesOnBoard[a.GetComponent<TileManager>().positionGrid.x, a.GetComponent<TileManager>().positionGrid.y] = new PieceOnBoard(Vector3Int.zero, null);

            Destroy(a);
        }
    }

    public bool CheckSelection(List<GameObject> clickedObjects)
    {
        if (clickedObjects.Count < minLengthSelected) return false;

        List<GameObject> copyClickedObjects = new List<GameObject>();

        //go through every possible recipy
        //for ever ingredient in the recipe, check if its same as ingredient in clicked objects, if at the end bot
        int index = 0;
        foreach (PieceToCreate c in piecesToCreate)
        {
            //if they are not same length, quick skip
            if (c.ingredients.Count != clickedObjects.Count) continue;

            copyClickedObjects.Clear();
            foreach (GameObject a in clickedObjects)
            {
                copyClickedObjects.Add(a);
            }

            int correctCount = 0;

            for (int i = 0; i < c.ingredients.Count; i++)
            {
                foreach (GameObject b in copyClickedObjects)
                {
                    if (c.ingredients[i] == b.GetComponent<TileManager>().wasInstantiatedFrom)
                    {
                        correctCount++;
                        copyClickedObjects.Remove(b);
                        break;
                    }
                }
            }

            //Debug.Log(correctCount);

            //Debug.Log(correctCount);
            //Debug.Log(c.ingredients.Count);

            if (correctCount == c.ingredients.Count)
            {
                if (!ConveyorController.Instance.AddPiece(c))
                {
                    GameObject.Find("Flowchart").GetComponent<Flowchart>().ExecuteBlock("ConveyorFull");
                    return false;
                }

                ClearBoardOfSelected(clickedObjects);

                FillBoard(clickedObjects);
                return true;
            }

            index++;
        }

        return false;
    }

    public PieceToCreate ReturnPieceFromIndex(int InIndex)
    {
        return piecesToCreate[InIndex];
    }

    public GameObject ReturnTileObjectFromPosition(Vector2Int position)
    {
        if(position.x > sizeX - 1 || position.x < 0)
        {
            return null;
        }
        else if (position.y > sizeY - 1 || position.y < 0)
        {
            return null;
        }

        return piecesOnBoard[position.x, position.y].gameObject;
    }

    public void ClearWholeBoard()
    {
        StartCoroutine(ClearWholeBoardAnimation());
    }

    private IEnumerator ClearWholeBoardAnimation()
    {
        knifeAnimation.Play();

        yield return new WaitForSeconds(timeBeforeRemovePiecesAfterKnifeAnimation);

        List<GameObject> allPieces = new List<GameObject>();

        for (int i = 0; i < sizeX; i++)
        {
            for (int k = 0; k < sizeY; k++)
            {
                allPieces.Add(piecesOnBoard[i, k].gameObject);
            }
        }

        ClearBoardOfSelected(allPieces);

        FillBoard(allPieces);
    }
}
