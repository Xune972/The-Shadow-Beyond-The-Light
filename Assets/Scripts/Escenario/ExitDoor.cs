using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ExitDoor : MonoBehaviour
{
    [SerializeField] private ExitManager exitManager;

    [Header("Personaje permitido en la puerta")]
    [SerializeField] private CharacterMovement[] selectedCharacter;

    [Header("Sonido de la puerta")]
    [SerializeField] private AudioClip[] doorClips;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.95f, 1.05f);
    [SerializeField] private Vector2 volumeRange = new Vector2(0.85f, 1f);

    private AudioSource audioSource;
    private int lastIndex = -1;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HitBox"))
        {
            CharacterMovement character = other.GetComponentInParent<CharacterMovement>();
            if (character == null) return;
            if (!IsSelectedCharacter(character)) return;

            Debug.Log("El personaje llegó a la puerta", this);
            PlayDoorSound();
            exitManager.NotifyArrive(character);
        }
    }

    private void PlayDoorSound()
    {
        if (doorClips == null || doorClips.Length == 0) return;

        
        int i = Random.Range(0, doorClips.Length);
        if (doorClips.Length > 1 && i == lastIndex)
            i = (i + 1) % doorClips.Length;
        lastIndex = i;

        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.PlayOneShot(doorClips[i], Random.Range(volumeRange.x, volumeRange.y));
    }

    private bool IsSelectedCharacter(CharacterMovement character)
    {
        foreach (CharacterMovement c in selectedCharacter)
        {
            if (c == character) return true;
        }
        return false;
    }
}