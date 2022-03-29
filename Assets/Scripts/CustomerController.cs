using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OrderStuff;
using UnityEngine.UI;

public class CustomerController : MonoBehaviour
{
    [System.Serializable]
    public class EmotionsOfType
    {
        public List<Sprite> neutralSprites;
        public List<Sprite> happySprites;
        public List<Sprite> sadSprites;
        public List<Sprite> angrySprites;
    }
    [System.Serializable]
    public class HairColor
    {
        public List<Sprite> hairSprites;
        public List<Sprite> beardSprites;
    }

    [Header("Face stuff")]
    public SpriteRenderer browSprite;
    public EmotionsOfType allBrowSprites;
    public SpriteRenderer eyeSprite;
    public EmotionsOfType allEyeSprites;
    public List<Sprite> allEyeSpritesBlinked;
    public SpriteRenderer mouthSprite;
    public EmotionsOfType allMouthSprites;
    [Header("Head and hands")]
    public SpriteRenderer headSprite;
    public SpriteRenderer leftHandSprite;
    public SpriteRenderer rightHandSprite;
    public List<Sprite> headSprites;
    public List<Sprite> handSprites;
    [Header("Hair stuff")]
    public SpriteRenderer hairSprite;
    public SpriteRenderer beardSprite;
    public List<HairColor> hairColors;
    public float beardPercentage;
    public Sprite emptySprite;
    [Header("Drink stuff")]
    public Animation drinkAnimation;
    public Animation pouringAnimation;
    public float showDrinkTime;
    [Header("----")]
    public float timeBetweenBlinks;
    public float blinkRandomnessPlusMinus;
    public float timeForBlink;
    public ParticleSystem angerParticles;
    public Animation leftHand;
    public Animation rightHand;
    public Collider2D grabCollider;
    public float timeBeforeRemovePiece;
    public List<SpriteRenderer> listPlaces;
    public List<SpriteRenderer> listBubbles;
    public List<Sprite> spritesForBubbles0to3;
    public float timeForAnger;
    public float timeAddedWhenGetPiece;
    public float averageLeaveTime;
    public float randomRange;
    public Vector3 selectedOffset;

    [System.NonSerialized]
    public List<OrderItem> itemsInOrder;
    [System.NonSerialized]
    public Vector3 startPosition;
    [System.NonSerialized]
    public Vector3 leftHandStartPosition;
    [System.NonSerialized]
    public Vector3 rightHandStartPosition;

    private List<Sprite> theseBrowSprites;
    private List<Sprite> theseEyeSprites;
    private Sprite thisBlinkedEyeSprite;
    private List<Sprite> theseMouthSprites;
    private bool angerParticlesPlaying;
    private float leaveTimer;
    private float startLeaveTimer;
    private bool isSelected;
    private int currentEmotion;
    private int defaultEmotion;
    private bool canPickStuffUp;

    public void CreateOrder(List<int> InItemIndexes)
    {
        itemsInOrder = new List<OrderItem>();

        for (int i = 0; i < InItemIndexes.Count; i++)
        {
            BoardManager.PieceToCreate piece = BoardManager.Instance.ReturnPieceFromIndex(InItemIndexes[i]);

            itemsInOrder.Add(new OrderItem(piece.sprite, piece.pieceLevel0to3));
        }

        UpdateOrderListVisual();
    }

    public void UpdateOrderListVisual()
    {
        //first clear
        for (int i = 0; i < listPlaces.Count; i++)
        {
            listPlaces[i].sprite = null;

            listBubbles[i].sprite = null;
        }

        for (int i = 0; i < itemsInOrder.Count; i++)
        {
            listPlaces[i].sprite = itemsInOrder[i].itemSprite;

            listBubbles[i].sprite = spritesForBubbles0to3[itemsInOrder[i].itemLevel];
        }
    }

    public void RecieveDrink()
    {
        drinkAnimation.Play();
        pouringAnimation.Play();

        leaveTimer = startLeaveTimer;
    }

    public void SetSelected(bool isSelectedState)
    {
        isSelected = isSelectedState;

        if (isSelectedState)
        {
            transform.position = startPosition + selectedOffset;

            StartCoroutine(SelectedAnimation());
        }
        else
        {
            transform.position = startPosition;
        }
    }

    public void ResetHandPositions()
    {
        leftHand.gameObject.transform.localPosition = leftHandStartPosition;
        rightHand.gameObject.transform.localPosition = rightHandStartPosition;
    }

    public void SetEmotionForTime(int emotion, float time)
    {
        StartCoroutine(EmotionAnimation(emotion, time));
    }

    //neutral, glad, ledsen, arg
    public void SetEmotion(int emotion)
    {
        currentEmotion = emotion;

        browSprite.sprite = theseBrowSprites[currentEmotion];
        eyeSprite.sprite = theseEyeSprites[currentEmotion];
        mouthSprite.sprite = theseMouthSprites[currentEmotion];
    }

    private void Start()
    {
        //init lists
        theseBrowSprites = new List<Sprite>();
        theseEyeSprites = new List<Sprite>();
        theseMouthSprites = new List<Sprite>();

        leaveTimer = Random.Range(averageLeaveTime - randomRange, averageLeaveTime + randomRange);
        startLeaveTimer = leaveTimer;

        angerParticlesPlaying = false;

        startPosition = transform.position;

        isSelected = false;

        canPickStuffUp = false;
        StartCoroutine(EnterAnimation());

        leftHandStartPosition = leftHand.gameObject.transform.localPosition;
        rightHandStartPosition = rightHand.gameObject.transform.localPosition;

        //emotion start
        SelectEmotionSpritesOnStart();
        SetEmotion(0);
        defaultEmotion = 0;
        StartCoroutine(BlinkLoop());
        
        SetHeadAndHandsOnStart();

        SetHairAndBeardOnStart();
    }

    private void Update()
    {
        //leave stuff
        leaveTimer -= Time.deltaTime;

        if (leaveTimer < timeForAnger && !angerParticlesPlaying)
        {
            angerParticlesPlaying = true;
            angerParticles.Play();
            SetEmotion(3);
            defaultEmotion = 3;
        }
        else if(leaveTimer > timeForAnger && angerParticlesPlaying)
        {
            angerParticlesPlaying = false;
            angerParticles.Stop();
            SetEmotion(0);
            defaultEmotion = 0;
        }
        if(leaveTimer < 0)
        {
            StartCoroutine(LeaveAnimation());
        }
    }

    private void SetHairAndBeardOnStart()
    {
        int colorSelect = Random.Range(0, hairColors.Count - 1);

        int hairSelect = Random.Range(0, hairColors[colorSelect].hairSprites.Count - 1);

        int beardSelect = Random.Range(0, hairColors[colorSelect].beardSprites.Count - 1);

        hairSprite.sprite = hairColors[colorSelect].hairSprites[hairSelect];

        if (Random.Range(0f, 1f) < beardPercentage)
        {
            beardSprite.sprite = hairColors[colorSelect].beardSprites[beardSelect];
        }
        else
        {
            beardSprite.sprite = emptySprite;
        }
    }

    private void SetHeadAndHandsOnStart()
    {
        int skintone = Random.Range(0, 4);
        headSprite.sprite = headSprites[skintone];
        leftHandSprite.sprite = handSprites[skintone];
        rightHandSprite.sprite = handSprites[skintone];
    }

    private void SelectEmotionSpritesOnStart()
    {
        //not breaking this out into function because want to modify like with eyes
        //brows
        int browRandom = Random.Range(0, allBrowSprites.neutralSprites.Count - 1);
        theseBrowSprites.Add(allBrowSprites.neutralSprites[browRandom]);
        theseBrowSprites.Add(allBrowSprites.happySprites[browRandom]);
        theseBrowSprites.Add(allBrowSprites.sadSprites[browRandom]);
        theseBrowSprites.Add(allBrowSprites.angrySprites[browRandom]);
        //eyes
        int eyeRandom = Random.Range(0, allEyeSprites.neutralSprites.Count - 1);
        thisBlinkedEyeSprite = allEyeSpritesBlinked[eyeRandom];
        theseEyeSprites.Add(allEyeSprites.neutralSprites[eyeRandom]);
        theseEyeSprites.Add(allEyeSprites.happySprites[eyeRandom]);
        theseEyeSprites.Add(allEyeSprites.sadSprites[eyeRandom]);
        theseEyeSprites.Add(allEyeSprites.angrySprites[eyeRandom]);
        //mouth
        int mouthRandom = Random.Range(0, allMouthSprites.neutralSprites.Count - 1);
        theseMouthSprites.Add(allMouthSprites.neutralSprites[browRandom]);
        theseMouthSprites.Add(allMouthSprites.happySprites[browRandom]);
        theseMouthSprites.Add(allMouthSprites.sadSprites[browRandom]);
        theseMouthSprites.Add(allMouthSprites.angrySprites[browRandom]);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.IsTouching(grabCollider)) return;

        if (!canPickStuffUp) return;

        Sprite collidedPieceSprite = collision.gameObject.GetComponent<SpriteRenderer>().sprite;
        //Debug.Log("enterd" + gameObject.name);

        foreach (OrderItem a in itemsInOrder)
        {
            if (collidedPieceSprite == a.itemSprite)
            {
                //add score
                MyGameManager.Instance.AddScore((a.itemLevel + 1) * LevelController.Instance.levelInformation.scoreMultiplierxLevel);

                itemsInOrder.Remove(a);

                UpdateOrderListVisual();

                leaveTimer += timeAddedWhenGetPiece;

                //start animation to remove sprite etc
                StartCoroutine(GrabAnimation(collision.gameObject));

                break;
            }
        }

        if (itemsInOrder.Count == 0) OrderComplete();
    }

    private void OrderComplete()
    {
        SetEmotion(1);

        leaveTimer = startLeaveTimer;

        StartCoroutine(LeaveAnimation());
    }

    private IEnumerator BlinkLoop()
    {
        while (true)
        {
            eyeSprite.sprite = theseEyeSprites[currentEmotion];

            yield return new WaitForSeconds(timeBetweenBlinks + Random.Range(-blinkRandomnessPlusMinus, blinkRandomnessPlusMinus));

            eyeSprite.sprite = thisBlinkedEyeSprite;

            yield return new WaitForSeconds(timeForBlink);
        }
    }

    private IEnumerator EmotionAnimation(int emotion, float time)
    {
        int beforeEmotion = currentEmotion;
        SetEmotion(emotion);

        yield return new WaitForSeconds(time);

        currentEmotion = beforeEmotion;
        SetEmotion(currentEmotion);
    }

    private IEnumerator SelectedAnimation()
    {
        leftHand.clip = GetClipByIndex(1, leftHand);
        rightHand.clip = GetClipByIndex(1, rightHand);

        //play
        leftHand.Play();
        rightHand.Play();
        SetEmotion(2);

        while (isSelected)
        {
            yield return new WaitForEndOfFrame();
        }

        //goes here when 
        leftHand.Stop();
        rightHand.Stop();
        SetEmotion(defaultEmotion);

        ResetHandPositions();
    }

    private IEnumerator GrabAnimation(GameObject piece)
    {
        leftHand.clip = GetClipByIndex(0, leftHand);
        rightHand.clip = GetClipByIndex(0, rightHand);

        leftHand.Play();
        rightHand.Play();

        yield return new WaitForSeconds(timeBeforeRemovePiece);

        piece.GetComponent<SpriteRenderer>().sprite = null;
    }

    private AnimationClip GetClipByIndex(int index, Animation animation)
    {
        int i = 0;
        foreach (AnimationState animationState in animation)
        {
            if (i == index)
                return animationState.clip;
            i++;
        }
        return null;
    }

    private IEnumerator LeaveAnimation()
    {
        yield return new WaitForSeconds(2f);

        StopAllCoroutines();

        Destroy(this.gameObject);
    }

    private IEnumerator EnterAnimation()
    {
        canPickStuffUp = false;

        //blind animation because cool
        yield return new WaitForSeconds(0.25f);
        SetEmotion(1);
        eyeSprite.sprite = thisBlinkedEyeSprite;
        yield return new WaitForSeconds(timeForBlink);
        eyeSprite.sprite = theseEyeSprites[currentEmotion];
        yield return new WaitForSeconds(0.1f);
        eyeSprite.sprite = thisBlinkedEyeSprite;
        yield return new WaitForSeconds(timeForBlink);
        eyeSprite.sprite = theseEyeSprites[currentEmotion];

        yield return new WaitForSeconds(0.9f);
        SetEmotion(0);

        canPickStuffUp = true;
    }
}
