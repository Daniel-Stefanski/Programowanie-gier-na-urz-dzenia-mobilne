using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Zasady rozgrywki")]
    [SerializeField] private int startingLives = 3;
    [SerializeField] private int goldenEggsForExtraLife = 3;

    [Header("Efekt rozbicia")]
    [SerializeField] private Sprite brokenEggSprite;
    [SerializeField] private float brokenEggY = -3.9f;

    public bool IsGameOver { get; private set; }
    public int Score => score;

    private int score;
    private int lives;
    private int caughtNormal;
    private int caughtGolden;
    private int caughtRotten;
    private int missedGoodEggs;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("W scenie sa dwa obiekty GameManager. Zostaw tylko jeden.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ResetStatistics();
    }

    private void Update()
    {
        if (IsGameOver || !Input.GetMouseButtonDown(0))
            return;

        Camera camera = Camera.main;
        if (camera == null)
        {
            Debug.LogError("Brak kamery z tagiem MainCamera.");
            return;
        }

        Vector3 cursorWorld = camera.ScreenToWorldPoint(Input.mousePosition);

        // Transformy jajek przemieszczaja sie w Update; zsynchronizuj je
        // przed sprawdzeniem trafienia w Collider 2D.
        Physics2D.SyncTransforms();

        Collider2D hit = Physics2D.OverlapPoint(new Vector2(cursorWorld.x, cursorWorld.y));
        if (hit != null && hit.TryGetComponent(out FallingEgg egg))
            egg.Collect();
    }

    public void EggCaught(FallingEgg egg)
    {
        if (IsGameOver)
            return;

        switch (egg.Type)
        {
            case EggType.Zwykle:
                score += 1;
                caughtNormal++;
                break;

            case EggType.Zlote:
                score += 5;
                caughtGolden++;

                if (goldenEggsForExtraLife > 0 &&
                    caughtGolden % goldenEggsForExtraLife == 0 &&
                    lives < startingLives)
                {
                    lives++;
                    Debug.Log("Bonus: odzyskano 1 zycie za zlote jajka!");
                }
                break;

            case EggType.Zgnile:
                score -= 2;
                caughtRotten++;
                break;
        }
    }

    public void EggMissed(FallingEgg egg)
    {
        if (IsGameOver || egg.Type == EggType.Zgnile)
            return;

        missedGoodEggs++;
        lives = Mathf.Max(0, lives - 1);
        ShowBrokenEgg(egg.transform.position.x);

        if (lives == 0)
            FinishGame();
    }

    private void ShowBrokenEgg(float x)
    {
        if (brokenEggSprite == null)
            return;

        GameObject effect = new GameObject("Efekt_Rozbite_Jajko");
        effect.transform.position = new Vector3(x, brokenEggY, 0f);

        SpriteRenderer renderer = effect.AddComponent<SpriteRenderer>();
        renderer.sprite = brokenEggSprite;
        renderer.sortingOrder = 6;

        Destroy(effect, 0.8f);
    }

    private void FinishGame()
    {
        IsGameOver = true;

        foreach (FallingEgg egg in FindObjectsByType<FallingEgg>(FindObjectsSortMode.None))
            Destroy(egg.gameObject);
    }

    private void ResetStatistics()
    {
        score = 0;
        lives = Mathf.Max(1, startingLives);
        caughtNormal = 0;
        caughtGolden = 0;
        caughtRotten = 0;
        missedGoodEggs = 0;
        IsGameOver = false;
    }

    private void RestartGame()
    {
        foreach (FallingEgg egg in FindObjectsByType<FallingEgg>(FindObjectsSortMode.None))
            Destroy(egg.gameObject);

        ResetStatistics();

        EggSpawner spawner = FindFirstObjectByType<EggSpawner>();
        if (spawner != null)
            spawner.ResetTimer();
    }

    // Tymczasowe liczniki TESTOWE. W etapie 2 zastapimy je docelowym UI.
    private void OnGUI()
    {
        GUI.Box(new Rect(12, 12, 310, 118), "TEST - ETAP 1");
        GUI.Label(new Rect(26, 37, 275, 25), "Punkty: " + score);
        GUI.Label(new Rect(26, 60, 275, 25), "Zycia: " + lives + " / " + startingLives);
        GUI.Label(new Rect(26, 83, 275, 25),
            "Zlapane: " + (caughtNormal + caughtGolden + caughtRotten) +
            " | Pominiete: " + missedGoodEggs);

        if (!IsGameOver)
            return;

        float panelX = Screen.width / 2f - 180f;
        float panelY = Screen.height / 2f - 90f;

        GUI.Box(new Rect(panelX, panelY, 360f, 180f), "KONIEC GRY");
        GUI.Label(new Rect(panelX + 70f, panelY + 48f, 260f, 28f),
            "Wynik koncowy: " + score);

        if (GUI.Button(new Rect(panelX + 70f, panelY + 100f, 220f, 43f),
            "Zagraj ponownie"))
        {
            RestartGame();
        }
    }
}
