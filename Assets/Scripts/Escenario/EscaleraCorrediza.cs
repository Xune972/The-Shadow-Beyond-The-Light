using System.Collections;
using UnityEngine;

public class EscaleraCorrediza : MonoBehaviour
{
    [Header("Escalones")]
    [Tooltip("Si queda vacío, usa todos los hijos de este objeto en orden.")]
    [SerializeField] private Transform[] steps;

    [Header("Movimiento")]
    [Tooltip("Desplazamiento local de cada escalón. X positivo = hacia la derecha.")]
    [SerializeField] private Vector3 slideOffset = new Vector3(3f, 0f, 0f);
    [SerializeField] private float stepDuration = 1f;
    [Tooltip("Segundos de espera antes de que arranque el siguiente escalón.")]
    [SerializeField] private float delayBetweenSteps = 1.5f;
    [SerializeField] private AnimationCurve easing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Vector3[] closedPositions;
    private Vector3[] openPositions;
    private bool isPlaying;
    private bool isOpen;

    public bool IsOpen => isOpen;
    public bool IsPlaying => isPlaying;

    private void Awake()
    {
        if (steps == null || steps.Length == 0)
        {
            steps = new Transform[transform.childCount];
            for (int i = 0; i < steps.Length; i++)
                steps[i] = transform.GetChild(i);
        }

        closedPositions = new Vector3[steps.Length];
        openPositions = new Vector3[steps.Length];

        for (int i = 0; i < steps.Length; i++)
        {
            closedPositions[i] = steps[i].localPosition;
            openPositions[i] = closedPositions[i] + slideOffset;
        }
    }

    /// <summary>Desliza los escalones uno por uno hasta su posición final.</summary>
    public void Extend()
    {
        if (isPlaying || isOpen)
            return;

        StartCoroutine(SlideSequence(closedPositions, openPositions, true));
    }

    /// <summary>Vuelve los escalones a su posición inicial, también uno por uno.</summary>
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

        for (int i = 0; i < steps.Length; i++)
        {
            StartCoroutine(MoveStep(steps[i], from[i], to[i]));
            yield return new WaitForSeconds(delayBetweenSteps);
        }

        // Esperamos a que termine el último escalón
        yield return new WaitForSeconds(Mathf.Max(0f, stepDuration - delayBetweenSteps));

        isOpen = opening;
        isPlaying = false;
    }

    private IEnumerator MoveStep(Transform step, Vector3 from, Vector3 to)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, stepDuration);
            step.localPosition = Vector3.LerpUnclamped(from, to, easing.Evaluate(Mathf.Clamp01(t)));
            yield return null;
        }

        step.localPosition = to;
    }

    private void OnDrawGizmosSelected()
    {
        Transform[] list = steps;

        if (list == null || list.Length == 0)
        {
            list = new Transform[transform.childCount];
            for (int i = 0; i < list.Length; i++)
                list[i] = transform.GetChild(i);
        }

        Gizmos.color = Color.cyan;

        foreach (Transform s in list)
        {
            if (s == null) continue;
            Vector3 end = transform.TransformPoint(transform.InverseTransformPoint(s.position) + slideOffset);
            Gizmos.DrawLine(s.position, end);
            Gizmos.DrawWireSphere(end, 0.1f);
        }
    }
}