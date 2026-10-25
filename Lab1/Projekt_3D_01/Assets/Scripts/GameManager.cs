using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameMode { None, TimeAttack, TargetAttack }
    public GameMode currentMode = GameMode.None;

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameHUDPanel;

    [Header("UI Texts")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI infoText; // Informacje o czasie lub pozostałych piłkach

    [Header("Game Settings")]
    public int score = 0;
    public float gameDuration = 30f; // Dla trybu 1 (czas)
    public int targetScore = 10;     // Dla trybu 2 (liczba zbitych piłek)

    private float timer = 0f;
    private bool isPlaying = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Włączamy menu główne na starcie
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (gameHUDPanel != null) gameHUDPanel.SetActive(false);
        Time.timeScale = 0f; // Zatrzymujemy grę do momentu wyboru trybu
    }

    void Update()
    {
        if (!isPlaying) return;

        if (currentMode == GameMode.TimeAttack)
        {
            timer -= Time.deltaTime;
            if (infoText != null) infoText.text = "Czas: " + Mathf.Max(0, timer).ToString("F1") + "s | Wynik: " + score;

            if (timer <= 0)
            {
                EndGame();
            }
        }
        else if (currentMode == GameMode.TargetAttack)
        {
            timer += Time.deltaTime;
            if (infoText != null) infoText.text = "Czas: " + timer.ToString("F1") + "s | Trafienia: " + score + "/" + targetScore;

            if (score >= targetScore)
            {
                EndGame();
            }
        }
    }

    // Metody wywoływane przez przyciski w Menu
    public void StartTimeAttackMode()
    {
        currentMode = GameMode.TimeAttack;
        timer = gameDuration;
        BeginGame();
    }

    public void StartTargetAttackMode()
    {
        currentMode = GameMode.TargetAttack;
        timer = 0f;
        BeginGame();
    }

    void BeginGame()
    {
        isPlaying = true;
        Time.timeScale = 1f; // Odliczanie czasu w grze
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameHUDPanel != null) gameHUDPanel.SetActive(true);
    }

    public void AddScore(int points)
    {
        if (!isPlaying) return;
        score += points;
        if (scoreText != null) scoreText.text = "Wynik: " + score;
    }

    void EndGame()
    {
        isPlaying = false;
        Time.timeScale = 0f; // Zatrzymanie gry
        if (infoText != null)
        {
            if (currentMode == GameMode.TimeAttack)
                infoText.text = "Koniec gry! Twój wynik: " + score + " punktów.";
            else
                infoText.text = "Gratulacje! Czas ukończenia: " + timer.ToString("F2") + " sekund.";
        }
    }
}