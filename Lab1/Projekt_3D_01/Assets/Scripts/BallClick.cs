using UnityEngine;

public class BallClick : MonoBehaviour
{
    // Ta metoda wbudowana w Unity wykrywa kliknięcie myszką lub dotknięcie na obiekcie z colliderem
    void OnMouseDown()
    {
        // Dodajemy 1 punkt do menedżera
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(1);
        }

        // Niszczymy piłkę po kliknięciu (opcjonalnie – jeśli chcesz, żeby znikała)
        Destroy(gameObject);
    }
}