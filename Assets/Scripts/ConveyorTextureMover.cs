using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorTextureMover : MonoBehaviour
{
    public float timeBeforeSpawnNewPiece;
    public Vector3 spawnPoint;
    public Vector3 endPoint;
    public Vector3 movementVector;
    public float speed;
    public GameObject trackPrefab;

    private float distanceSpawnEnd;
    private List<GameObject> tracks;
    private float loopingTimer;

    void Start()
    {
        distanceSpawnEnd = Vector3.Distance(spawnPoint, endPoint);
        loopingTimer = timeBeforeSpawnNewPiece;

        tracks = new List<GameObject>();
        tracks.Add(transform.GetChild(0).gameObject);
        tracks.Add(transform.GetChild(1).gameObject);
        tracks.Add(transform.GetChild(2).gameObject);
    }

    void Update()
    {
        //check if should spawn another
        loopingTimer -= Time.deltaTime;
        if(loopingTimer < 0f)
        {
            tracks.Add(CreateNewTrack());
            loopingTimer = timeBeforeSpawnNewPiece;
        }

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
        Gizmos.DrawSphere(endPoint, 0.1f);
    }
}
