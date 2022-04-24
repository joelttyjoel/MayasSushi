using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OrderStuff;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OrderStuff
{
    [System.Serializable]
    public class OrderItem
    {
        public Sprite itemSprite;
        public int itemLevel;

        public OrderItem(Sprite InItemSprite, int InItemLevel)
        {
            itemSprite = InItemSprite;
            itemLevel = InItemLevel;
        }
    }

    [System.Serializable]
    public class Customer
    {
        public float spawnTime;
        public List<int> itemsInOrderIndexInBoardManager;

        //public Customer(List<int> _itemsInOrderIndexInBoardManager)
        //{
        //    itemsInOrderIndexInBoardManager = _itemsInOrderIndexInBoardManager;
        //}
    }

    [System.Serializable]
    public class ListOfCustomerLists
    {
        public int scoreForLevel;
        public List<Customer> listOfCustomers;
    }
}

public class MyGameManager : MonoBehaviour
{
    //simply having this causes crash
    public AudioClip customerEnter;
    public AudioClip customerLeave;
    public GameObject customerPrefab;
    public List<GameObject> customerPlaces;
    public GameObject recipeBook;
    public GameObject pausMenu;
    public float timeOnRecipeBookOpen;
    public float timeBetweenCustomerListChecks;

    private int score;
    private bool recipeBookState;
    private bool pausMenuState;
    private List<Customer> customerList;
    private float timer;
    private Queue<Customer> customersInQue;
    private bool levelIsDone;
    private AudioSource audioSource;

    private static MyGameManager _instance;
    public static MyGameManager Instance { get { return _instance; } }

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

    void Start()
    {
        score = 0;

        recipeBookState = false;
        recipeBook.SetActive(recipeBookState);

        pausMenuState = false;
        pausMenu.SetActive(pausMenuState);

        customersInQue = new Queue<Customer>();

        //read current level settings
        customerList = new List<Customer>(LevelController.Instance.levelInformation.levels[LevelController.Instance.currentLevel].customers);

        //for first customer teehee
        AddCustomersToQue();
        AddCustromersFromQue();

        levelIsDone = false;

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //check if fill with another customer every 5 sec
        if (timer > timeBetweenCustomerListChecks)
        {
            timer = 0f;

            AddCustomersToQue();
            AddCustromersFromQue();

            //at end, check if end game
            CheckForEndLevel();
        }

        timer += Time.deltaTime;
    }

    private void AddCustomersToQue()
    {
        if (customerList.Count == 0) return;

        if (customerList[0].spawnTime < Time.timeSinceLevelLoad)
        {
            //Debug.Log(Time.timeSinceLevelLoad);

            customersInQue.Enqueue(customerList[0]);
            customerList.Remove(customerList[0]);
        }
    }

    private void AddCustromersFromQue()
    {
        if (customersInQue.Count > 0)
        {
            bool customerCreated = CreateCustomer(customersInQue.Peek());

            if(customerCreated)
            {
                audioSource.clip = customerEnter;
                audioSource.Play();
            }
        }
    }

    public void CustomerLeave()
    {
        audioSource.clip = customerLeave;
        audioSource.Play();
    }

    private void CheckForEndLevel()
    {
        if (customersInQue.Count == 0 && customerList.Count == 0 && levelIsDone == false)
        {
            for (int i = 0; i < 3; i++)
            {
                if (customerPlaces[i].transform.childCount != 0)
                {
                    return;
                }
            }

            Debug.Log("LEVEL ENDING");

            levelIsDone = true;

            //if got here, means noone has children
            LevelController.Instance.StartEndLevelSequence();
        }
    }

    private bool CreateCustomer(Customer InCustomer)
    {
        for (int i = 0; i < 3; i++)
        {
            if (customerPlaces[i].transform.childCount == 0)
            {
                GameObject temp = GameObject.Instantiate(customerPrefab);

                temp.transform.position = customerPlaces[i].transform.position + new Vector3(0, 0, -1);
                temp.transform.SetParent(customerPlaces[i].transform);

                temp.GetComponent<CustomerController>().CreateOrder(customersInQue.Dequeue().itemsInOrderIndexInBoardManager);

                return true;
            }
        }

        return false;
    }

    public void SwitchTwoCustomers(GameObject Customer1, GameObject Customer2)
    {
        GameObject CustomerPlace1 = Customer1.transform.parent.gameObject;
        GameObject CustomerPlace2 = Customer2.transform.parent.gameObject;
        CustomerController CustomerController1 = Customer1.GetComponent<CustomerController>();
        CustomerController CustomerController2 = Customer2.GetComponent<CustomerController>();

        Customer1.transform.position = CustomerPlace2.transform.position + new Vector3(0, 0, -1); ;
        Customer1.transform.SetParent(CustomerPlace2.transform);
        CustomerController1.startPosition = CustomerPlace2.transform.position + new Vector3(0, 0, -1);

        Customer2.transform.position = CustomerPlace1.transform.position + new Vector3(0, 0, -1);
        Customer2.transform.SetParent(CustomerPlace1.transform);
        CustomerController2.startPosition = CustomerPlace1.transform.position + new Vector3(0, 0, -1);
    }

    public void PlaceCustomerAtPlace(CustomerController Customer, GameObject CustomerPlaceTo)
    {
        if(CustomerPlaceTo.transform.childCount == 0)
        {
            Customer.transform.position = CustomerPlaceTo.transform.position + new Vector3(0, 0, -1);
            Customer.transform.SetParent(CustomerPlaceTo.transform);
            Customer.startPosition = CustomerPlaceTo.transform.position + new Vector3(0, 0, -1);
        }
    }

    public void ClearWholeBoard()
    {
        if (!InteractionManager.Instance.canInteract) return;

        BoardManager.Instance.ClearWholeBoard();
    }

    public void ClearWholeConveyor()
    {
        if (!InteractionManager.Instance.canInteract) return;

        ConveyorController.Instance.ClearWholeConveyor();
    }

    public void ToggleRecipeBook()
    {
        //always deselect all buttons
        EventSystem.current.SetSelectedGameObject(null);

        //always able to close recipe book but not always open
        if(!recipeBookState)
        {
            if (!InteractionManager.Instance.canInteract) return;
        }

        recipeBookState = !recipeBookState;

        recipeBook.SetActive(recipeBookState);

        InteractionManager.Instance.canInteract = !recipeBookState;

        //set time

        if (recipeBookState) SetTimescale(timeOnRecipeBookOpen);
        else SetTimescale(1f);

    }

    public void TogglePauseMenu()
    {
        //always deselect all buttons
        EventSystem.current.SetSelectedGameObject(null);

        if (!InteractionManager.Instance.canInteract) return;

        pausMenuState = !pausMenuState;

        pausMenu.SetActive(pausMenuState);

        InteractionManager.Instance.canInteract = !pausMenuState;

        //set time

        if (pausMenuState) SetTimescale(0f);
        else SetTimescale(1f);

    }

    public void ResetCurrentLevel()
    {
        LevelController.Instance.ResetCurrentLevel();
    }

    public void LoadOtherSceneByIndex(int index)
    {
        LevelController.Instance.LoadOtherSceneByIndex(index);
    }

    public int GetScore()
    {
        return score;
    }

    public void AddScore(int InScore)
    {
        score += InScore;
    }

    public void SetTimescale(float timescale)
    {
        Time.timeScale = timescale;
    }
}
