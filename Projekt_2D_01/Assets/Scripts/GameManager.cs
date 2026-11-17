using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Zasady rozgrywki - z Etapu 1")]
    [SerializeField] private int startingLives = 3;
    [SerializeField] private int goldenEggsForExtraLife = 3;

    [Header("Efekt rozbicia - z Etapu 1")]
    [SerializeField] private Sprite brokenEggSprite;
    [SerializeField] private float brokenEggY = -3.9f;

    [Header("HUD - Etap 2")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text goldenProgressText;
    [SerializeField] private Image[] heartImages = new Image[3];
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;
    [SerializeField] private GameObject pauseButton;

    [Header("Game Over - Etap 2")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text finalScoreText;

    [Header("Pauza - Etap 2")]
    [SerializeField] private GameObject pausePanel;

    public bool IsGameOver { get; private set; }
    public int Score => score;

    private bool isPaused;
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
        Time.timeScale = 1f;
        ResetStatistics();
    }

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.SetActive(true);

        RefreshUI();
    }

    private void Update()
    {
        if (IsGameOver || isPaused || !Input.GetMouseButtonDown(0))
            return;

        // Klikniecia UI nie powinny zbierac jajek znajdujacych sie pod przyciskami.
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Camera camera = Camera.main;
        if (camera == null)
        {
            Debug.LogError("Brak kamery z tagiem MainCamera.");
            return;
        }

        Vector3 cursorWorld = camera.ScreenToWorldPoint(Input.mousePosition);
        Physics2D.SyncTransforms();

        Collider2D hit = Physics2D.OverlapPoint(
            new Vector2(cursorWorld.x, cursorWorld.y));

        if (hit != null && hit.TryGetComponent(out FallingEgg egg))
            egg.Collect();
    }

    public void EggCaught(FallingEgg egg)
    {
        if (IsGameOver || isPaused)
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

        RefreshUI();
    }

    public void EggMissed(FallingEgg egg)
    {
        if (IsGameOver || isPaused || egg.Type == EggType.Zgnile)
            return;

        missedGoodEggs++;
        lives = Mathf.Max(0, lives - 1);
        ShowBrokenEgg(egg.transform.position.x);
        RefreshUI();

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
        isPaused = false;
        Time.timeScale = 1f;

        foreach (FallingEgg egg in
            FindObjectsByType<FallingEgg>(FindObjectsSortMode.None))
        {
            Destroy(egg.gameObject);
        }

        if (finalScoreText != null)
            finalScoreText.text = "Twój wynik: " + score;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
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
        isPaused = false;
    }

    private void RefreshUI()
    {
        if (scoreText != null)
            scoreText.text = "Wynik: " + score;

        if (goldenProgressText != null)
        {
            int progress = goldenEggsForExtraLife > 0
                ? caughtGolden % goldenEggsForExtraLife
                : 0;
            goldenProgressText.text = goldenEggsForExtraLife > 0
                ? "Złote do życia: " + progress + "/" + goldenEggsForExtraLife
                : "Bonus życia wyłączony";
        }

        if (heartImages == null)
            return;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null)
                continue;

            heartImages[i].sprite = i < lives
                ? fullHeartSprite
                : emptyHeartSprite;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        foreach (FallingEgg egg in
            FindObjectsByType<FallingEgg>(FindObjectsSortMode.None))
        {
            Destroy(egg.gameObject);
        }

        ResetStatistics();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.SetActive(true);

        EggSpawner spawner = FindFirstObjectByType<EggSpawner>();
        if (spawner != null)
            spawner.ResetTimer();

        RefreshUI();
    }

    public void PauseGame()
    {
        if (IsGameOver || isPaused)
            return;

        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SzalonyKurnik_Menu");
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
