using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;

    public GameObject creditsMenu;

    public void OpenCreditsMenu ()
    {
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }

    public void OpenMainMenuPanel()
    {
        mainMenu.SetActive(true);
        creditsMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

   public void PlayGame()
    {
        SceneManager.LoadScene("Level 1");
    }
}
