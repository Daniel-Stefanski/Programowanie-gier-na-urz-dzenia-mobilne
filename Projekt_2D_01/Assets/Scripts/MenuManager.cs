using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SzalonyKurnik_Gra");
    }

    public void QuitGame()
    {
        Debug.Log("Wyjscie z gry. Przycisk zakonczy aplikacje w buildzie.");
        Application.Quit();
    }
}
