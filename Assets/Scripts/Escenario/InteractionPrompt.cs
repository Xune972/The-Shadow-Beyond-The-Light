using UnityEngine;


public class InteractionPrompt : MonoBehaviour
{
    [Header("Jugador")]
    [Tooltip("Si queda vacío, busca un objeto con el tag 'Player'.")]
    [SerializeField] private Transform player;

    [Header("Cartel")]
    [SerializeField] private float radius = 3f;

    [Tooltip("Tecla que se muestra en el mensaje (solo es texto, no detecta la tecla).")]
    [SerializeField] private KeyCode key = KeyCode.E;

    [Tooltip("{0} se reemplaza por la tecla.")]
    [SerializeField] private string message = "Presiona {0}";


    public bool PlayerNear { get; private set; }

    private void Start()
    {
        FindPlayer();
    }

    private void FindPlayer()
    {
        if (player != null) return;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void Update()
    {
        
        if (player == null)
        {
            FindPlayer();
            if (player == null) return;
        }

        bool near = (player.position - transform.position).sqrMagnitude <= radius * radius;
        if (near == PlayerNear) return;

        PlayerNear = near;

        if (PromptUI.Instance == null) return;

        if (near)
            PromptUI.Instance.Show(this, message.Replace("{0}", key.ToString()));
        else
            PromptUI.Instance.Hide(this);
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
