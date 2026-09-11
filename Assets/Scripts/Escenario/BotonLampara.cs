using UnityEngine;

public class BotonLampara : MonoBehaviour
{
    [Header("Lamparas que controla este boton")]
    public LamparaSombra[] lamparas;

    [Header("Interaccion")]
    [Tooltip("Tecla para accionar el boton cuando el jugador esta cerca")]
    public KeyCode teclaInteraccion = KeyCode.E;

    [Tooltip("Tag que debe tener el jugador")]
    public string tagJugador = "Player";

    private bool jugadorCerca = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagJugador))
        {
            jugadorCerca = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagJugador))
        {
            jugadorCerca = false;
        }
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(teclaInteraccion))
        {
            AccionarBoton();
        }
    }

    public void AccionarBoton()
    {
        foreach (LamparaSombra lampara in lamparas)
        {
            if (lampara != null)
            {
                lampara.Alternar();
            }
        }
    }
}