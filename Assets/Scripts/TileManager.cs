using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public ParticleSystem particlesystem;
    public Vector3Int positionGrid;
    public GameObject wasInstantiatedFrom;
    public int ingredientIndex;

    private bool isSelected;

    // Start is called before the first frame update
    void Start()
    {
        isSelected = false;
        
        particlesystem.Stop();
    }
    
    public void SetSelected(bool State)
    {
        isSelected = State;

        if (isSelected) particlesystem.Play();
        else particlesystem.Stop();
    }

    public void StartFallingAnimation(AnimationCurve InCurve, float InFallingTime, Vector3 InEndPosition)
    {
        StartCoroutine(FallingAnimation(InCurve, InFallingTime, InEndPosition));
    }

    private IEnumerator FallingAnimation(AnimationCurve InCurve, float InFallingTime, Vector3 InEndPosition)
    {
        float timeSinceStart = 0f;
        float percentageOfTravel = 0f;
        float distanceToEnd = Vector3.Distance(transform.position, InEndPosition);
        Vector3 movementVector = new Vector3(0f, -distanceToEnd, 0f);
        Vector3 startPosition = transform.position;

        while(timeSinceStart < InFallingTime)
        {
            timeSinceStart += Time.deltaTime;
            yield return new WaitForEndOfFrame();

            percentageOfTravel = timeSinceStart / InFallingTime;

            //move percentage of distance based on percentage of time
            //percentage of distance comes from animation, percentage of time comes from function
            transform.position = startPosition + (movementVector * InCurve.Evaluate(percentageOfTravel));
        }

        //in end is at final position
        transform.position = startPosition + movementVector;
    }
}
