using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SakeController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject sakeOnMouse;
    public Sprite sakeDone;
    public Sprite sakeWaiting;
    public float sakeTime;
    public BasicFillTimerController timerController;

    private float timer;
    private bool spriteSetToDone;
    private bool isUsingsake;
    private Image image;
    private GameObject sakeOnMouseObject;

    // Start is called before the first frame update
    void Start()
    {
        timer = sakeTime;
        spriteSetToDone = false;
        isUsingsake = false;

        image = GetComponent<Image>();

        sakeOnMouse.transform.position = new Vector3(-9999, -9999, 0);
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0f && !spriteSetToDone)
        {
            spriteSetToDone = true;

            image.sprite = sakeDone;
        }
        else if (timer > 0f && spriteSetToDone)
        {
            spriteSetToDone = false;

            image.sprite = sakeWaiting;
        }

        UpdateTimer();
    }

    private void UpdateTimer()
    {
        timerController.SetTimerPercentage(1 - (timer / sakeTime));
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (spriteSetToDone && InteractionManager.Instance.canInteract)
        {
            isUsingsake = true;
            InteractionManager.Instance.canInteract = false;
        }
        else isUsingsake = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isUsingsake) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        sakeOnMouse.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isUsingsake) return;
        isUsingsake = false;

        RaycastHit2D objectHitPerhaps = InteractionManager.Instance.CheckIfHitObject(InteractionManager.Instance.layerMaskCustomers);
        if (objectHitPerhaps.collider != null && objectHitPerhaps.collider.GetComponentInParent<CustomerController>() != null)
        {
            objectHitPerhaps.collider.GetComponentInParent<CustomerController>().RecieveDrink();

            timer = sakeTime;
        }

        sakeOnMouse.transform.position = new Vector3(-9999, -9999, 0);

        InteractionManager.Instance.canInteract = true;
    }

    //start hold piece
    //private void OnMouseDown()
    //{
    //    if (spriteSetToDone && InteractionManager.Instance.canInteract)
    //    {
    //        isUsingsake = true;
    //        InteractionManager.Instance.canInteract = false;
    //    }
    //    else isUsingsake = false;
    //}

    //private void OnMouseDrag()
    //{
    //    if (!isUsingsake) return;

    //    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    //    sakeOnMouse.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
    //}

    ////drop piece if possible
    //private void OnMouseUp()
    //{
    //    if (!isUsingsake) return;
    //    isUsingsake = false;

    //    RaycastHit2D objectHitPerhaps = InteractionManager.Instance.CheckIfHitObject(InteractionManager.Instance.layerMaskCustomers);
    //    if (objectHitPerhaps.collider != null)
    //    {
    //        objectHitPerhaps.collider.GetComponentInParent<CustomerController>().ResetLeaveTimer();

    //        timer = sakeTime;
    //    }

    //    sakeOnMouse.transform.position = new Vector3(-9999, -9999, 0);

    //    InteractionManager.Instance.canInteract = true;
    //}
}
