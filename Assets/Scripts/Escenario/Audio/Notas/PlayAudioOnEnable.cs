using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayAudioOnEnable : MonoBehaviour
{
    [SerializeField] private AudioClip endAudio;
    [SerializeField] private float delay = 0f;
    [SerializeField] private bool stopOnDisable = true;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; // Sonido 2D
    }

    private void OnEnable()
    {
        if (endAudio == null) return;

        audioSource.clip = endAudio;

        if (delay > 0f)
            audioSource.PlayDelayed(delay);
        else
            audioSource.Play();
    }

    private void OnDisable()
    {
        if (stopOnDisable && audioSource != null)
            audioSource.Stop();
    }
}