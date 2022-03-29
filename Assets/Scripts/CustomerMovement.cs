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

        //first check for edge case where selected is null but is moving, happens if customer leaves while mid air
        if(isMovingCustomer && customerSelected == null)
        {
            isMovingCustomer = false;
        }

        RaycastHit2D objectHitPerhaps = InteractionManager.Instance.CheckIfHitObject(InteractionManager.Instance.layerMaskCustomers);
        //has clicked air and shuold deselect if is moving customer
        if (objectHitPerhaps.collider == null)
        {
            if (isMovingCustomer)
            {
                isMovingCustomer = false;
                customerSelected.SetSelected(false);
            }

            return;
        }

        //edge case where selected is null but is moving, happens if customer leaves while mid air
        if (isMovingCustomer && customerSelected == null)
        {
            isMovingCustomer = false;
        }

        //check if clicked empty space with no customer
        if (objectHitPerhaps.collider.GetComponentInParent<CustomerController>() == null)
        {
            if (isMovingCustomer)
            {
                isMovingCustomer = false;
                customerSelected.SetSelected(false);
                
                MyGameManager.Instance.PlaceCustomerAtPlace(customerSelected, objectHitPerhaps.collider.gameObject);
            }
        }
        else
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
    }
}
