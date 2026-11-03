using UnityEngine;

public class EggSpawner : MonoBehaviour
{
    [Header("Prefaby jajek")]
    [SerializeField] private FallingEgg normalEggPrefab;
    [SerializeField] private FallingEgg goldenEggPrefab;
    [SerializeField] private FallingEgg rottenEggPrefab;

    [Header("Co ile sekund spada kolejne jajko")]
    [SerializeField] private float minSpawnDelay = 0.8f;
    [SerializeField] private float maxSpawnDelay = 1.4f;

    [Header("Predkosc spadania (jednostki na sekunde)")]
    [SerializeField] private float minFallSpeed = 1.8f;
    [SerializeField] private float maxFallSpeed = 3.0f;

    [Header("Stopniowy wzrost trudnosci") ]
    [SerializeField] private float speedBonusPer10Points = 0.2f;
    [SerializeField] private float maximumSpeedBonus = 1.5f;

    [Header("Obszar spadania dla ekranu 16:9, kamera Size 5")]
    [SerializeField] private float minX = -7.4f;
    [SerializeField] private float maxX = 7.4f;
    [SerializeField] private float spawnY = 5.6f;
    [SerializeField] private float groundY = -4.2f;

    [Header("Wagi losowania (nie musza sumowac sie do 100)")]
    [SerializeField] private int normalWeight = 70;
    [SerializeField] private int goldenWeight = 15;
    [SerializeField] private int rottenWeight = 15;

    private float timeToNextSpawn = 0.5f;

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver)
            return;

        timeToNextSpawn -= Time.deltaTime;
        if (timeToNextSpawn > 0f)
            return;

        SpawnEgg();
        timeToNextSpawn = Random.Range(minSpawnDelay, maxSpawnDelay);
    }

    private void SpawnEgg()
    {
        if (normalEggPrefab == null || goldenEggPrefab == null || rottenEggPrefab == null)
        {
            Debug.LogError("EggSpawner: przypisz WSZYSTKIE 3 prefaby jajek w Inspectorze.");
            enabled = false;
            return;
        }

        int normal = Mathf.Max(0, normalWeight);
        int golden = Mathf.Max(0, goldenWeight);
        int rotten = Mathf.Max(0, rottenWeight);
        int total = normal + golden + rotten;

        if (total <= 0)
        {
            Debug.LogError("EggSpawner: przynajmniej jedna waga musi byc wieksza od zera.");
            enabled = false;
            return;
        }

        int draw = Random.Range(0, total);
        FallingEgg selectedPrefab = draw < normal
            ? normalEggPrefab
            : draw < normal + golden
                ? goldenEggPrefab
                : rottenEggPrefab;

        Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX), spawnY, 0f);
        FallingEgg egg = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        float speedBonus = Mathf.Clamp(
            (Mathf.Max(0, GameManager.Instance.Score) / 10) * speedBonusPer10Points,
            0f, maximumSpeedBonus);

        egg.Initialize(Random.Range(minFallSpeed, maxFallSpeed) + speedBonus, groundY);
    }

    public void ResetTimer()
    {
        timeToNextSpawn = 0.5f;
        enabled = true;
    }
}
