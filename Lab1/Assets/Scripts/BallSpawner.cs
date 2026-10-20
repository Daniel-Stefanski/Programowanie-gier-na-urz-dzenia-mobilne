using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;       // Tutaj przeciągniemy prefab piłki
    public float spawnInterval = 1.0f;  // Czas w sekundach między kolejnymi piłkami
    public int maxBalls = 10;              // Maksymalna liczba piłek na scenie

    private List<GameObject> activeBalls = new List<GameObject>(); // Lista przechowująca aktywne piłki

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Uruchamiamy coroutine odpowiedzialną za ciągłe tworzenie piłek
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Czeka określoną liczbę sekund przed kolejnym spawnem
            yield return new WaitForSeconds(spawnInterval);

            // Najpierw oczyszczamy listę z piłek, które zostały zniszczone (np. wypadły poza planszę)
            activeBalls.RemoveAll(ball => ball == null);

            // Sprawdzamy, czy liczba aktywnych piłek jest mniejsza niż limit
            if (activeBalls.Count < maxBalls)
            {
                // Tworzy kopię piłki w pozycji spawner'a
                GameObject newBall = Instantiate(ballPrefab, transform.position, transform.rotation);
                
                // Dodajemy ją do naszej listy śledzonych piłek
                activeBalls.Add(newBall);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
