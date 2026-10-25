using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;       
    public float spawnInterval = 1.0f;  
    public int maxBalls = 10;           

    // Obszar losowania nad planszą (szerokość i głębokość sufitu)
    public float spawnRangeX = 3.0f;
    public float spawnRangeZ = 3.0f;

    private List<GameObject> activeBalls = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            activeBalls.RemoveAll(ball => ball == null);

            if (activeBalls.Count < maxBalls)
            {
                // Losowanie pozycji w obrębie sufitu
                Vector3 randomPosition = transform.position + new Vector3(
                    Random.Range(-spawnRangeX, spawnRangeX),
                    0f,
                    Random.Range(-spawnRangeZ, spawnRangeZ)
                );

                GameObject newBall = Instantiate(ballPrefab, randomPosition, transform.rotation);
                activeBalls.Add(newBall);
            }
        }
    }
}