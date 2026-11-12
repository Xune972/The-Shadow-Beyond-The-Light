using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject PanelPausa;
    private bool pausado = false;

    void Start()
    {
        PanelPausa.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausado) Continuar();
            else Pausar();
        }
    }

    public void Pausar()
    {
        PanelPausa.SetActive(true);
        Time.timeScale = 0f;
        pausado = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Continuar()
    {
        PanelPausa.SetActive(false);
        Time.timeScale = 1f;
        pausado = false;
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Salir()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Menu");
    }
}