using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton, ułatwiający dostep z poziomu piłki
    public int score = 0; // Wynik gracza

    void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Aktualny wynik: " + score);
    }
}
