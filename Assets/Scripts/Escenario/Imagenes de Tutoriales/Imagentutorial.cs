using System.Collections;
using UnityEngine;

// Poné este script en un objeto de UI que tenga un CanvasGroup
// (por ejemplo, un Image dentro de tu Canvas con la explicación de la mecánica).
[RequireComponent(typeof(CanvasGroup))]
public class ImagenTutorial : MonoBehaviour
{
    [Header("Tiempos")]
    [Tooltip("Cuántos segundos se muestra la imagen antes de empezar a desvanecerse")]
    [SerializeField] private float duracionVisible = 4f;
    [Tooltip("Cuánto tarda en desvanecerse (fade out)")]
    [SerializeField] private float duracionFadeOut = 1f;
    [Tooltip("Cuánto tarda en aparecer al inicio (fade in). Poné 0 para que aparezca de golpe.")]
    [SerializeField] private float duracionFadeIn = 0.5f;

    [Header("Comportamiento")]
    [Tooltip("Si está activo, se muestra automáticamente al empezar el nivel")]
    [SerializeField] private bool mostrarAlInicio = true;
    [Tooltip("Si está activo, desactiva el GameObject por completo al terminar (ahorra recursos)")]
    [SerializeField] private bool desactivarAlTerminar = true;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    private void Start()
    {
        if (mostrarAlInicio)
        {
            Mostrar();
        }
    }

    // Llamá a este método desde otro script (por ejemplo, al entrar a una zona)
    // si preferís mostrarla en un momento específico en vez de al inicio.
    public void Mostrar()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(RutinaMostrar());
    }

    private IEnumerator RutinaMostrar()
    {
        // Fade in
        yield return StartCoroutine(Desvanecer(0f, 1f, duracionFadeIn));

        // Espera visible
        yield return new WaitForSeconds(duracionVisible);

        // Fade out
        yield return StartCoroutine(Desvanecer(1f, 0f, duracionFadeOut));

        if (desactivarAlTerminar)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator Desvanecer(float desde, float hasta, float duracion)
    {
        if (duracion <= 0f)
        {
            canvasGroup.alpha = hasta;
            yield break;
        }

        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracion)
        {
            tiempoTranscurrido += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(desde, hasta, tiempoTranscurrido / duracion);
            yield return null;
        }

        canvasGroup.alpha = hasta;
    }
}