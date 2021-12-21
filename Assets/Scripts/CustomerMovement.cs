using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerMovement : MonoBehaviour
{
    private bool isMovingCustomer;
    private CustomerController customerSelected;

    // Start is called before the first frame update
    void Start()
    {
        isMovingCustomer = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) ClickSomewhere();
    }

    private void ClickSomewhere()
    {
        if (!InteractionManager.Instance.canInteract) return;

        RaycastHit2D objectHitPerhaps = InteractionManager.Instance.CheckIfHitObject(InteractionManager.Instance.layerMaskCustomers);

        if (objectHitPerhaps.collider != null)
        {
            //has hit customer, now if moving customer already, if is same customer deselect, if other customer switch places
            if(isMovingCustomer)
            {
                isMovingCustomer = false;
                customerSelected.SetSelected(false);

                if (customerSelected != objectHitPerhaps.collider.GetComponentInParent<CustomerController>())
                {
                    MyGameManager.Instance.SwitchTwoCustomers(objectHitPerhaps.collider.transform.parent.gameObject, customerSelected.gameObject);
                }
            }
            else
            {
                isMovingCustomer = true;

                customerSelected = objectHitPerhaps.collider.GetComponentInParent<CustomerController>();
                customerSelected.SetSelected(true);
            }
        }
        //has clicked air and shuold deselect if is moving customer
        else
        {
            if(isMovingCustomer)
            {
                isMovingCustomer = false;
                customerSelected.SetSelected(false);
            }
        }
    }
}
