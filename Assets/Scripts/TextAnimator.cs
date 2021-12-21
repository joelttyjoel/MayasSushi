using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAnimator : MonoBehaviour
{
    public List<string> strings;
    public Text text;
    public float animationTime;

    private float timer;
    private int index;

    private void Start()
    {
        index = 0;
        timer = animationTime;

        text.text = strings[index];
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if(timer < 0)
        {
            timer = animationTime;
            index++;
            if (index >= strings.Count) index = 0;

            text.text = strings[index];
        }
    }
}
