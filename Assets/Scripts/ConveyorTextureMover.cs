using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorTextureMover : MonoBehaviour
{
    [Header("ADD ONE CHILD IN MIDDLE TO START")]
    public Vector3 spawnPoint;
    public Vector3 middlePoint;
    public Vector3 endPoint;
    public Vector3 movementVector;
    public float speed;
    public GameObject trackPrefab;
    public float middlePointRange;

    private float distanceSpawnEnd;
    private List<GameObject> tracks;
    private bool hasHitMiddle = false;

    void Start()
    {
        distanceSpawnEnd = Vector3.Distance(spawnPoint, endPoint);

        tracks = new List<GameObject>();
        tracks.Add(transform.GetChild(0).gameObject);
    }

    void Update()
    {
        Vector3 movementOffset = Time.deltaTime * speed * movementVector;

        //move all pieces
        foreach (GameObject a in tracks)
        {
            a.transform.position += movementOffset;
        }

        //if first item passes end, delete
        if (Vector3.Distance(tracks[0].transform.position, spawnPoint) > distanceSpawnEnd)
        {
            GameObject.Destroy(tracks[0]);
            tracks.RemoveAt(0);
        }

        //if one item passes middle, create new piece
        foreach (GameObject a in tracks)
        {
            if (!hasHitMiddle && Vector3.Distance(a.transform.position, middlePoint) < middlePointRange)
            {
                tracks.Add(CreateNewTrack());
                hasHitMiddle = true;
                StartCoroutine(MiddlePointCooldown());
                break;
            }
        }
    }

    private IEnumerator MiddlePointCooldown()
    {
        hasHitMiddle = true;
        yield return new WaitForSeconds(1f);
        hasHitMiddle = false;
    }

    private GameObject CreateNewTrack()
    {
        GameObject a = Instantiate(trackPrefab);
        a.transform.SetParent(this.transform);
        a.transform.position = spawnPoint;
        a.transform.localScale = new Vector3(1f, 1f, 1f);

        return a;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(spawnPoint, 0.1f);
        Gizmos.DrawSphere(middlePoint, 0.1f);
        Gizmos.DrawSphere(endPoint, 0.1f);
    }
}
