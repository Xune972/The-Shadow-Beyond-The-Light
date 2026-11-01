
using UnityEngine;
using UnityEngine.Audio;

public class NoteSFX : MonoBehaviour
{
    [Header("Sonidos")]
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private AudioMixerGroup mixerGroup;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.95f, 1.05f);
    [SerializeField] private Vector2 volumeRange = new Vector2(0.85f, 1f);

    private AudioSource source;

    private void Awake()
    {
        GameObject child = new GameObject("NoteSFX_Audio");
        child.transform.SetParent(transform, false);

        source = child.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f; // 2D
        source.outputAudioMixerGroup = mixerGroup;
    }

    public void Play()
    {
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        source.pitch = Random.Range(pitchRange.x, pitchRange.y);
        source.PlayOneShot(clip, Random.Range(volumeRange.x, volumeRange.y));
    }
}