using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class PisoCorredizo : MonoBehaviour
{
    [Header("Configuración del Piso")]
    public Transform piso; // Objeto 3D del piso que se va a mover
    public Vector3 direccionMovimiento = new Vector3(0f, 0f, 3f); // Distancia y dirección del recorrido
    public float velocidad = 2.0f;
    public KeyCode teclaInteraccion = KeyCode.E;

    [Header("Comportamiento")]
    [Tooltip("Si está activo, se puede volver a accionar el botón para que el piso regrese. Si está desactivado, una vez que se mueve queda fijo en destino.")]
    public bool puedeAlternar = true;

    [Header("Restricción de jugador")]
    [Tooltip("Tag que debe tener el jugador para poder accionar este piso (solo el personaje sombra)")]
    public string tagJugadorPermitido = "ShadowPlayer";

    [Header("Sonidos")]
    [SerializeField] private AudioClip openClip;
    [SerializeField] private AudioClip closeClip;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.95f, 1.05f);
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    private AudioSource audioSource;
    private bool jugadorCerca = false;
    private bool enPosicionFinal = false;
    private Vector3 posicionInicial;
    private Vector3 posicionObjetivo;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        if (piso != null)
        {
            posicionInicial = piso.position;
            posicionObjetivo = posicionInicial + direccionMovimiento;
        }
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(teclaInteraccion))
        {
            if (puedeAlternar || !enPosicionFinal)
            {
                enPosicionFinal = !enPosicionFinal;
                PlayFloorSound(enPosicionFinal);
            }
        }

        if (piso != null)
        {
            Vector3 destino = enPosicionFinal ? posicionObjetivo : posicionInicial;
            piso.position = Vector3.Lerp(piso.position, destino, Time.deltaTime * velocidad);
        }
    }
    private void PlayFloorSound(bool opening)
    {
        AudioClip clip = opening ? openClip : closeClip;
        if (clip == null) return;

        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.PlayOneShot(clip, volume);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag(tagJugadorPermitido))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag(tagJugadorPermitido))
        {
            jugadorCerca = false;
        }
    }
}
