using UnityEngine;

public class Botón : MonoBehaviour
{
    public WallsController controlador;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("ShadowPlayer"))
        {
            controlador.ActivarParedes();
        }
       
    }
}
