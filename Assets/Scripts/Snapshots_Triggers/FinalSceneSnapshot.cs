using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class FinalSceneSnapshot : MonoBehaviour
{
    [Header("Snapshots")]
    public AudioMixerSnapshot Final_Scene;

    [Header("Transición")]
    public float fadeTime = 2f;

    private IEnumerator Start()
    {
        if (Final_Scene == null)
        {
            Debug.LogWarning("FinalSceneSnapshot: falta asignar el snapshot Final", this);
            yield break;
        }

        Final_Scene.TransitionTo(fadeTime);
    }
}