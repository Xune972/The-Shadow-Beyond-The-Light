using System.Collections;
using UnityEngine;

public class PlataformasCorredizas : MonoBehaviour
{
    [Header("Plataformas")]
    [Tooltip("Si queda vacío, usa todos los hijos de este objeto en orden.")]
    [SerializeField] private Transform[] platforms;

    [Header("Movimiento")]
    [Tooltip("Desplazamiento local por defecto de cada plataforma. X positivo = derecha.")]
    [SerializeField] private Vector3 slideOffset = new Vector3(3f, 0f, 0f);
    [Tooltip("Opcional: un desplazamiento propio por plataforma (mismo orden que la lista). Si queda vacío, todas usan Slide Offset.")]
    [SerializeField] private Vector3[] customOffsets;
    [SerializeField] private float moveDuration = 1f;
    [Tooltip("Segundos de espera antes de que arranque la siguiente plataforma. Con 0 se mueven todas a la vez.")]
    [SerializeField] private float delayBetweenPlatforms = 1.5f;
    [SerializeField] private AnimationCurve easing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Vector3[] closedPositions;
    private Vector3[] openPositions;
    private bool isPlaying;
    private bool isOpen;

    public bool IsOpen => isOpen;
    public bool IsPlaying => isPlaying;

    private void Awake()
    {
        if (platforms == null || platforms.Length == 0)
        {
            platforms = new Transform[transform.childCount];
            for (int i = 0; i < platforms.Length; i++)
                platforms[i] = transform.GetChild(i);
        }

        closedPositions = new Vector3[platforms.Length];
        openPositions = new Vector3[platforms.Length];

        for (int i = 0; i < platforms.Length; i++)
        {
            closedPositions[i] = platforms[i].localPosition;
            openPositions[i] = closedPositions[i] + GetOffset(i);
        }
    }

    private Vector3 GetOffset(int index)
    {
        if (customOffsets != null && index < customOffsets.Length)
            return customOffsets[index];

        return slideOffset;
    }

    /// <summary>Mueve las plataformas a su posición final, una por una.</summary>
    public void Extend()
    {
        if (isPlaying || isOpen)
            return;

        StartCoroutine(SlideSequence(closedPositions, openPositions, true));
    }

    /// <summary>Devuelve las plataformas a su posición inicial.</summary>
    public void Retract()
    {
        if (isPlaying || !isOpen)
            return;

        StartCoroutine(SlideSequence(openPositions, closedPositions, false));
    }

    /// <summary>Extiende si está cerrada, retrae si está abierta.</summary>
    public void Toggle()
    {
        if (isOpen) Retract();
        else Extend();
    }

    private IEnumerator SlideSequence(Vector3[] from, Vector3[] to, bool opening)
    {
        isPlaying = true;

        for (int i = 0; i < platforms.Length; i++)
        {
            StartCoroutine(MovePlatform(platforms[i], from[i], to[i]));

            if (delayBetweenPlatforms > 0f)
                yield return new WaitForSeconds(delayBetweenPlatforms);
        }

        // Esperamos a que termine la última plataforma
        yield return new WaitForSeconds(Mathf.Max(0f, moveDuration - delayBetweenPlatforms));

        isOpen = opening;
        isPlaying = false;
    }

    private IEnumerator MovePlatform(Transform platform, Vector3 from, Vector3 to)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, moveDuration);
            platform.localPosition = Vector3.LerpUnclamped(from, to, easing.Evaluate(Mathf.Clamp01(t)));
            yield return null;
        }

        platform.localPosition = to;
    }

    private void OnDrawGizmosSelected()
    {
        Transform[] list = platforms;

        if (list == null || list.Length == 0)
        {
            list = new Transform[transform.childCount];
            for (int i = 0; i < list.Length; i++)
                list[i] = transform.GetChild(i);
        }

        Gizmos.color = Color.yellow;

        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == null) continue;
            Vector3 end = transform.TransformPoint(transform.InverseTransformPoint(list[i].position) + GetOffset(i));
            Gizmos.DrawLine(list[i].position, end);
            Gizmos.DrawWireSphere(end, 0.1f);
        }
    }
}