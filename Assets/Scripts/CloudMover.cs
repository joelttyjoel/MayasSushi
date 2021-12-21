using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMover : MonoBehaviour
{
    public float speed;
    public float speedRandomness;
    
    [System.NonSerialized]
    public Vector3 endPosition;

    // Start is called before the first frame update
    void Start()
    {
        speed += Random.Range(0f, speedRandomness);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;

        //will break if goiong other wya but ok
        if (transform.position.x > endPosition.x) Destroy(this.gameObject);
    }
}
