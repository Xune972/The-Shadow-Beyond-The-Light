using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class BotonActivador : MonoBehaviour
{
    [Header("Detección")]
    [SerializeField] private string playerTag = "Player";

    [Header("Activación")]
    [Tooltip("Si está activo, hay que apretar la tecla estando cerca. Si no, se activa al pisarlo.")]
    [SerializeField] private bool requireInteractKey = false;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private bool onlyOnce = true;

    [Header("Cartel (opcional)")]
    [SerializeField] private GameObject interactPrompt;

    [Header("Eventos")]
    [SerializeField] private UnityEvent onPressed;
    [Tooltip("Solo se llama cuando el jugador sale del botón (útil para placas de presión).")]
    [SerializeField] private UnityEvent onReleased;

    private bool playerInside;
    private bool alreadyUsed;

    private void Reset()
    {
        // Deja el collider como Trigger al agregar el script
        GetComponent<Collider>().isTrigger = true;
    }

    private void Awake()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }
    private void Update()
    {
        if (requireInteractKey && playerInside && Input.GetKeyDown(interactKey))
            Press();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerInside = true;

        if (interactPrompt != null) interactPrompt.SetActive(true);

        if (!requireInteractKey)
            Press();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerInside = false;

        if (interactPrompt != null) interactPrompt.SetActive(false);

        onReleased?.Invoke();
    }

    private void Press()
    {
        if (onlyOnce && alreadyUsed)
            return;

        alreadyUsed = true;
        onPressed?.Invoke();
    }
}
