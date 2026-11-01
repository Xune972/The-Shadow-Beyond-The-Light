using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NoteInteractable : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip noteAudio;
    [SerializeField] private bool stopAudioOnClose = true;
    [SerializeField] private bool playOnlyOnce = false;

    [Header("UI (opcional)")]
    [SerializeField] private GameObject interactPrompt; // Ej: "Presioná E"
    [SerializeField] private GameObject notePanel;      // Panel con el texto de la nota

    [Header("Interacción")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string playerTag = "Shadow"; // Tag del personaje de sombra
    [SerializeField] private bool freezePlayerWhileOpen = true;


    private AudioSource audioSource;
    private CharacterMovement player;
    private bool playerInRange;
    private bool isOpen;
    private bool alreadyPlayed;
    private NoteSFX noteSfx;

    private void Awake()
    {
        noteSfx = GetComponent<NoteSFX>();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; // Sonido 2D

        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (notePanel != null) notePanel.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange && !isOpen)
            return;

        // El cartel solo se muestra si la sombra es el personaje activo
        if (!isOpen && interactPrompt != null && player != null)
            interactPrompt.SetActive(player.ControlsEnabled);

        if (Input.GetKeyDown(interactKey))
        {
            if (isOpen)
            {
                CloseNote();
            }
            else if (player != null && player.ControlsEnabled)
            {
                OpenNote();
            }
        }
        else if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseNote();
        }
    }

    private void OpenNote()
    {
        isOpen = true;

        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (notePanel != null) notePanel.SetActive(true);
        if (noteSfx != null) noteSfx.Play();

        if (noteAudio != null && !(playOnlyOnce && alreadyPlayed))
        {
            audioSource.clip = noteAudio;
            audioSource.Play();
            alreadyPlayed = true;
        }

        if (freezePlayerWhileOpen && player != null)
            player.SetControlsEnabled(false);
    }

    private void CloseNote()
    {
        isOpen = false;

        if (notePanel != null) notePanel.SetActive(false);

        if (stopAudioOnClose)
            audioSource.Stop();

        if (freezePlayerWhileOpen && player != null)
            player.SetControlsEnabled(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInRange = true;
        player = other.GetComponentInParent<CharacterMovement>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInRange = false;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (isOpen)
            CloseNote();
    }

    private void OnDisable()
    {
        // Evita que el personaje quede congelado si la nota se desactiva abierta
        if (isOpen && freezePlayerWhileOpen && player != null)
            player.SetControlsEnabled(true);
    }
}