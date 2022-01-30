using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSizeFitter : MonoBehaviour
{
    public float orthSizePastMax;
    public float fixedWidth;
    public float aspectRatioW;
    public float aspectRatioH;

    private Camera thisCamera;
    private float currentAspectRatio;
    private float maxLimitAspectRatio;

    // Start is called before the first frame update
    void Start()
    {
        thisCamera = GetComponent<Camera>();

        maxLimitAspectRatio = aspectRatioW / aspectRatioH;

        currentAspectRatio = (float)Screen.width / (float)Screen.height;

        if (currentAspectRatio < maxLimitAspectRatio)
        {
            thisCamera.orthographicSize = fixedWidth / currentAspectRatio / 2;
        }
        else
        {
            thisCamera.orthographicSize = orthSizePastMax;
        }
    }

    //private void Update()
    //{
    //    currentAspectRatio = (float)Screen.width / (float)Screen.height;

    //    if (currentAspectRatio < maxLimitAspectRatio)
    //    {
    //        Debug.Log("fix");
    //        thisCamera.orthographicSize = fixedWidth / currentAspectRatio / 2;
    //    }
    //    else
    //    {
    //        thisCamera.orthographicSize = orthSizePastMax;
    //    }
    //}
}
