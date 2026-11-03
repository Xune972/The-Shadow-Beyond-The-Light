using UnityEngine;
using UnityEngine.Audio;

public class ChaseSnapshotTrigger : MonoBehaviour

{
    [Header("Snapshots")]
    public AudioMixerSnapshot Music;
    public AudioMixerSnapshot Chase;

    [Header("Tiempos de transición")]
    public float fadeInTime = 2f;

    private bool chaseActive = false;

    private void Start()
    {
        if (Music != null)
            Music.TransitionTo(0f);
    }
    public void ActivateChase()
    {
        if (chaseActive) return;

        chaseActive = true;

        if (Chase != null)
            Chase.TransitionTo(fadeInTime);
    }
    public void ResetToMusic()
    {
        chaseActive = false;

        if (Music != null)
            Music.TransitionTo(0f);
    }
}