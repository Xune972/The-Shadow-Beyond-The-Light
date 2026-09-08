using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
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


}
