using UnityEngine;
using UnityEngine.Audio;

public class LevelSnapshot : MonoBehaviour
{
    [Header("Snapshot de los niveles")]
    public AudioMixerSnapshot levelSnapshot;

    [Header("Transición")]
    public float fadeTime = 2f;
}