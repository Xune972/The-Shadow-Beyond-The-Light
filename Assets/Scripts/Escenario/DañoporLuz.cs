using UnityEngine;
using System.Collections.Generic;

// Poné este script en el GameObject de tu personaje sombra.
// IMPORTANTE: el material del personaje tiene que soportar transparencia
// (Rendering Mode = Transparent o Fade en Standard Shader, o un shader URP/HDRP transparente).
public class DanoPorLuz : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private float vidaMaxima = 100f;
    [SerializeField] private float vidaActual;

    [Header("Daño y curación")]
    [Tooltip("Cuánta vida pierde por segundo mientras está bajo una lámpara encendida")]
    [SerializeField] private float dañoPorSegundo = 25f;
    [Tooltip("Cuánta vida recupera por segundo mientras está en la sombra (ninguna lámpara lo ilumina)")]
    [SerializeField] private float curacionPorSegundo = 15f;

    [Header("Desvanecimiento")]
    [Tooltip("Velocidad del desvanecido/aparecido, en cambio de alpha por segundo. Más alto = más rápido.")]
    [SerializeField] private float velocidadDesvanecimiento = 1.5f;
    [Tooltip("El/los Renderer del personaje que se desvanecen. Si se deja vacío, se buscan solos en los hijos.")]
    [SerializeField] private Renderer[] renderers;
    [Tooltip("Colliders que se desactivan mientras está completamente desvanecido (opcional)")]
    [SerializeField] private Collider[] colliders;

    [Header("Obstáculos (opcional)")]
    [Tooltip("Si tenés paredes que deban bloquear la luz, poné esa layer acá. Si no, dejalo en Nothing.")]
    [SerializeField] private LayerMask capaObstaculos;

    public float VidaActual => vidaActual;
    public bool EstaIluminado { get; private set; }
    public bool EstaDesvanecido { get; private set; }

    private List<LamparaSombra> lamparas = new List<LamparaSombra>();
    private Material[] materiales;
    private Color[] coloresOriginales;
    private float alphaActual = 1f;

    private void Start()
    {
        vidaActual = vidaMaxima;
        lamparas.AddRange(FindObjectsByType<LamparaSombra>(FindObjectsSortMode.None));

        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>();

        // Creamos instancias únicas de los materiales para no modificar el asset compartido
        materiales = new Material[renderers.Length];
        coloresOriginales = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            materiales[i] = renderers[i].material;
            coloresOriginales[i] = materiales[i].color;
        }
    }

    private void Update()
    {
        EstaIluminado = ComprobarSiEstaIluminado();

        if (EstaIluminado)
        {
            vidaActual -= dañoPorSegundo * Time.deltaTime;
            vidaActual = Mathf.Max(0f, vidaActual);
        }
        else if (vidaActual < vidaMaxima)
        {
            vidaActual += curacionPorSegundo * Time.deltaTime;
            vidaActual = Mathf.Min(vidaMaxima, vidaActual);
        }

        ActualizarDesvanecimiento();

        // Colliders solo se desactivan/reactivan cuando el desvanecido está completo (alpha ~0)
        bool debeEstarDesvanecido = alphaActual <= 0.01f;
        if (debeEstarDesvanecido != EstaDesvanecido)
        {
            EstaDesvanecido = debeEstarDesvanecido;
            foreach (Collider c in colliders)
            {
                if (c != null) c.enabled = !EstaDesvanecido;
            }
        }
    }

    private void ActualizarDesvanecimiento()
    {
        // El alpha objetivo sigue la vida: 0 vida = invisible, vida completa = visible
        float alphaObjetivo = vidaMaxima > 0f ? vidaActual / vidaMaxima : 0f;
        alphaActual = Mathf.MoveTowards(alphaActual, alphaObjetivo, velocidadDesvanecimiento * Time.deltaTime);

        for (int i = 0; i < materiales.Length; i++)
        {
            if (materiales[i] == null) continue;
            Color c = coloresOriginales[i];
            c.a = alphaActual;
            materiales[i].color = c;
        }
    }

    private bool ComprobarSiEstaIluminado()
    {
        foreach (LamparaSombra lampara in lamparas)
        {
            if (lampara == null || !lampara.encendida) continue;

            Light luz = lampara.GetComponent<Light>();
            if (luz == null || !luz.enabled) continue;

            float distancia = Vector3.Distance(luz.transform.position, transform.position);
            if (distancia > luz.range) continue;

            if (luz.type == LightType.Spot)
            {
                Vector3 direccion = (transform.position - luz.transform.position).normalized;
                float angulo = Vector3.Angle(luz.transform.forward, direccion);
                if (angulo > luz.spotAngle * 0.5f) continue;
            }

            // Si hay obstáculos configurados, chequeamos que no haya nada bloqueando la luz
            if (capaObstaculos.value != 0)
            {
                Vector3 dir = transform.position - luz.transform.position;
                if (Physics.Raycast(luz.transform.position, dir.normalized, dir.magnitude, capaObstaculos))
                {
                    continue; // hay algo tapando la luz, no cuenta
                }
            }

            return true; // está dentro del alcance de esta lámpara
        }
        return false;
    }
}