using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideyMenuController : MonoBehaviour
{
    public float moveSpeed;
    public Vector3 offsetVector;
    public List<GameObject> menuItems;
    public LayerMask slideyMenuMask;
    
    [System.NonSerialized]
    public float timeSinceSlide;
    [System.NonSerialized]
    public bool isSliding;
    [System.NonSerialized]
    public bool willSlide;

    private List<Vector3> positionsOnConveyor;
    private int currentIndex;
    private Vector3 startDragPos;
    private Vector3 endDragPos;

    private void Start()
    {
        positionsOnConveyor = new List<Vector3>();
        foreach (GameObject a in menuItems)
        {
            positionsOnConveyor.Add(a.transform.position);
        }

        currentIndex = (menuItems.Count - 1) / 2;

        timeSinceSlide = 0f;
    }

    private void MyMouseDown()
    {
        startDragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        isSliding = true;
        timeSinceSlide = 0f;
    }

    private void MyMouseUp()
    {
        endDragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dragVector = endDragPos - startDragPos;

        if (Vector3.Magnitude(dragVector) > 0.1f)
        {
            if (dragVector.x > 0f)
            {
                MoveConveyor(1);
            }
            else
            {
                MoveConveyor(-1);
            }

            willSlide = true;
        }
        else
        {
            willSlide = false;
        }

        isSliding = false;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (RaycastForSlideyArea()) MyMouseDown();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (RaycastForSlideyArea()) MyMouseUp();
        }

        Vector3 targetPosition = positionsOnConveyor[currentIndex] + offsetVector;

        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
        {
            transform.position = targetPosition;
        }

        if(!isSliding)
        {
            timeSinceSlide += Time.deltaTime;
        }
    }

    private bool RaycastForSlideyArea()
    {
        RaycastHit2D hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hit = Physics2D.Raycast(ray.origin, ray.direction, 1000.0f, slideyMenuMask);
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

    private void MoveConveyor(int movement)
    {
        currentIndex = currentIndex + movement;

        if (currentIndex < 0)
        {
            currentIndex = 0;
        }
        else if (currentIndex > menuItems.Count - 1)
        {
            currentIndex = menuItems.Count - 1;
        }
    }
}


