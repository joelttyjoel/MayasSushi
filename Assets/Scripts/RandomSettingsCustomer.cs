using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSettingsCustomer : MonoBehaviour
{
    public float breathSpeedMin;
    public float breathSpeedMax;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().SetFloat("SpeedMultiplier", Random.Range(breathSpeedMin, breathSpeedMax));
    }
}
