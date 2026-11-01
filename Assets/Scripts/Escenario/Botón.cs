using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Botón : MonoBehaviour
{
    public WallsController controlador;

    [Header("Sonido del botón")]
    [SerializeField] private AudioClip buttonClip;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.97f, 1.03f);
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("ShadowPlayer"))
        {
            PlayButtonSound();
            controlador.ActivarParedes();
        }
    }

    private void PlayButtonSound()
    {
        if (buttonClip == null) return;

        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.PlayOneShot(buttonClip, volume);
    }
}
