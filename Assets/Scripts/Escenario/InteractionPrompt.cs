using UnityEngine;
using UnityEngine.Events;


public class InteractionPrompt : MonoBehaviour
{
    [Header("Jugador")]
    [Tooltip("Si queda vacío, busca un objeto con el tag 'Player'.")]
    [SerializeField] private Transform player;

    [Header("Interacción")]
    [SerializeField] private float radius = 3f;
    [SerializeField] private KeyCode key = KeyCode.E;

    [Tooltip("{0} se reemplaza por la tecla.")]
    [SerializeField] private string message = "Presiona {0}";

    [Header("Evento al presionar la tecla")]
    public UnityEvent onInteract;

    public bool PlayerNear { get; private set; }

    private void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        bool near = (player.position - transform.position).sqrMagnitude <= radius * radius;

        if (near && !PlayerNear)
        {
            if (PromptUI.Instance != null)
                PromptUI.Instance.Show(this, string.Format(message, key));
        }
        else if (!near && PlayerNear)
        {
            if (PromptUI.Instance != null)
                PromptUI.Instance.Hide(this);
        }

        PlayerNear = near;

        if (PlayerNear && Input.GetKeyDown(key))
            onInteract.Invoke();
    }

    private void OnDisable()
    {
        PlayerNear = false;
        if (PromptUI.Instance != null)
            PromptUI.Instance.Hide(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}