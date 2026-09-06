using System.Runtime.CompilerServices;
using UnityEngine;

public class PuertaCorrediza : MonoBehaviour
{
    [Header("Configuración de la Puerta")]
    public Transform puerta; // Objeto 3D  de la puerta
    public Vector3 direccionMovimiento = new Vector3(2f, 0f, 0f); // Cuanto y hacia donde se movera ( distancia en X, Y o Z)
    public float velocidad = 3.0f;
    public KeyCode teclaInteraccion = KeyCode.E;

    private bool jugadorCerca = false;
    private bool estaAbierta = false;
    private Vector3 posicionInicial;
    private Vector3 posicionObjetivo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (puerta != null)
        {
            // Guarda la posición cerrada inicial
            posicionInicial = puerta.position;
            // Calcula la posición abierta sumando el desplazamiento definido
            posicionObjetivo = posicionInicial + direccionMovimiento;
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Detecta la tecla si el jugador esta dentro de la zona del boton
        if (jugadorCerca && Input.GetKeyDown(teclaInteraccion))
        {
            estaAbierta = !estaAbierta; // Alterna el estado (abrir/cerrar)
        }

        if (puerta != null)
        {
            // Determina la posición destino según el estado
            Vector3 destino = estaAbierta ? posicionObjetivo : posicionInicial;

            // Mueve la puerta de forma fluida hacia el destino
            puerta.position = Vector3.Lerp(puerta.position, destino, Time.deltaTime * velocidad);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }
}
