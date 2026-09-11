using UnityEngine;

[RequireComponent(typeof(Light))]
public class LamparaSombra : MonoBehaviour
{
    [Header("Estado")]
    public bool encendida = false;

    private Light luz;

    void Awake()
    {
        luz = GetComponent<Light>();
        AplicarEstado();
    }

    // Conectá estos métodos al OnClick() de tus botones de UI (o InputAction si usás Input System)

    public void Encender()
    {
        encendida = true;
        AplicarEstado();
    }

    public void Apagar()
    {
        encendida = false;
        AplicarEstado();
    }

    public void Alternar()
    {
        encendida = !encendida;
        AplicarEstado();
    }

    private void AplicarEstado()
    {
        if (luz != null)
            luz.enabled = encendida;
    }
}
