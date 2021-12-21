using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeBookController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public float canvasScale;
    public RectTransform scrollableArea;
    public Scrollbar scrollbar;
    public Vector3 scrollableAreaStart;
    public Vector3 scrollableAreaEnd;
    public float dragSpeed;
    public float velocityDecaySpeedPrecentage;
    public float vecocityMinCutValue;

    //private Vector3 startDragPos;
    //private Vector3 endDragPos;
    //private Vector3 startPosScollableArea;
    //private Vector3 startToEndVector;
    private Vector3 startPos;
    private Vector3 velocity;
    private Vector3 previousDragPos;

    private void Start()
    {
        //startToEndVector = scrollableAreaEnd - scrollableAreaStart;
        startPos = scrollableArea.transform.position / canvasScale;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //startDragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //startPosScollableArea = scrollableArea.transform.localPosition;
        
        previousDragPos = new Vector3(0f, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0f);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        //Vector3 newPos = startPosScollableArea + Vector3.Project((Camera.main.ScreenToWorldPoint(Input.mousePosition) - startDragPos) / canvasScale, Vector3.up);

        //if(newPos.y < scrollableAreaStart.y)
        //{
        //    newPos = new Vector3(newPos.x, scrollableAreaStart.y, newPos.z);
        //}
        //else if(newPos.y > scrollableAreaEnd.y)
        //{
        //    newPos = new Vector3(newPos.x, scrollableAreaEnd.y, newPos.z);
        //}

        //scrollableArea.transform.localPosition = newPos;
        Vector3 currentDragPos = new Vector3(0f, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0f);
        velocity += currentDragPos - previousDragPos;

        previousDragPos = new Vector3(0f, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //endDragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void Update()
    {
        Vector3 newPos = scrollableArea.transform.localPosition + (velocity * dragSpeed);
        //Debug.Log(velocity);

        if (newPos.y < scrollableAreaStart.y)
        {
            newPos = new Vector3(newPos.x, scrollableAreaStart.y, newPos.z);
        }
        else if (newPos.y > scrollableAreaEnd.y)
        {
            newPos = new Vector3(newPos.x, scrollableAreaEnd.y, newPos.z);
        }

        scrollableArea.transform.localPosition = newPos;

        //decay velocity
        if (velocity.magnitude > vecocityMinCutValue)
        {
            velocity = velocity.normalized * (velocityDecaySpeedPrecentage * velocity.magnitude);
        }
        else
        {
            velocity = Vector3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(new Vector3(scrollableAreaStart.x * canvasScale, scrollableAreaStart.y * canvasScale, 0f), 0.3f);
        Gizmos.DrawSphere(new Vector3(scrollableAreaEnd.x * canvasScale, scrollableAreaEnd.y * canvasScale, 0f), 0.3f);
    }
}
