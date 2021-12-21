using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public List<GameObject> clouds;
    public Vector3 spawnPosition;
    public Vector3 endPosition;
    public float spawnRangeY;
    public float timeBetweenSpawns;
    public float randomnessBetweenSpawns;
    public int maxStartClouds;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CloudSpawnerLoop());

        //maybe start random clouds at start hm
        int startingClouds = Random.Range(0, maxStartClouds);
        for(int i = 0; i < startingClouds; i++)
        {
            //idk not making this universal
            int cloudIndex = Random.Range(0, clouds.Count);

            GameObject cloud = Instantiate(clouds[cloudIndex]);
            cloud.transform.SetParent(this.transform);
            cloud.transform.position = spawnPosition;
            cloud.transform.position += new Vector3(-Random.Range(0f, 2f * spawnPosition.x), Random.Range(-spawnRangeY, spawnRangeY), 0f);

            CloudMover cloudMover = cloud.GetComponent<CloudMover>();
            cloudMover.endPosition = endPosition;
        }
    }

    private IEnumerator CloudSpawnerLoop()
    {
        while(true)
        {
            SpawnCloud();

            yield return new WaitForSeconds(timeBetweenSpawns + Random.Range(0f, randomnessBetweenSpawns));
        }
    }

    private void SpawnCloud()
    {
        int cloudIndex = Random.Range(0, clouds.Count);

        GameObject cloud = Instantiate(clouds[cloudIndex]);
        cloud.transform.SetParent(this.transform);
        cloud.transform.position = spawnPosition;
        cloud.transform.position += new Vector3(0, Random.Range(-spawnRangeY, spawnRangeY), 0f);

        CloudMover cloudMover = cloud.GetComponent<CloudMover>();
        cloudMover.endPosition = endPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(spawnPosition, 0.3f);
        Gizmos.DrawSphere(endPosition, 0.3f);

        Gizmos.DrawLine(spawnPosition, new Vector3(spawnPosition.x, spawnPosition.y + spawnRangeY, 0f));
        Gizmos.DrawLine(spawnPosition, new Vector3(spawnPosition.x, spawnPosition.y - spawnRangeY, 0f));
    }
}
