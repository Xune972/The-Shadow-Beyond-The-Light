using System;
using UnityEngine;

// Se encarga solo de detectar si la Sombra esta expuesta a esta luz puntual, que pasa cuando eso ocurre (muerte, reset, sonido, etc)
// Es responsabilidad de otro script que se suscriba a OnExposureChanged (ver ShadowLightReactor), esto respeta el Principio de Responsabilidad unica
public class LightExposureDetector : MonoBehaviour
{
    [Header("Referencias")]
    // Si los dejas vacíos en el Inspector, Awake() los busca solo con GetComponent, siempre que esten en el mismo GameObject
    [SerializeField] private Light sourceLight;
    [SerializeField] private SphereCollider rangeTrigger;

    [Header("Puntos de muestreo de la Sombra")]
    // Transforms hijos de la sombra que representan distintas partes del cuerpo (por ejemplo: Head, Body, Feet), no hace falta que sean muchos
    // la tolerancia la da el umbral de abajo, no la cantidad de puntos
    [SerializeField] private Transform[] shadowSamplePoints;

    [Header("Tolerancia")]
    [Range(0f, 1f)]
    // Fraccion de puntos que tienen que estar iluminados para recien ahi contar como "expuesto". Con 0.5 (default): si la luz le pega a la mitad de los puntos o menos, la Sombra sigue protegida
    // hace falta que sea MÁS de la mitad para que cuente como expuesta, esto le da al jugador un pequeño margen de error
    [SerializeField] private float exposureThreshold = 0.5f;

    [Header("Detección")]
    // Tag que debe tener el Collider del personaje Sombra
    [SerializeField] private string shadowTag = "Shadow";

    // Capas que si bloquean la luz (paredes, cajas, obstáculos).
    [SerializeField] private LayerMask blockerMask;

    [Header("Spot Light")]

    [SerializeField] private bool checkSpotAngle = true;

    [Header("Debug")]
    // Dibuja en la vista Scene una linea por cada punto chequeado (roja si ese punto esta iluminado, verde si esta bloqueado)
    [SerializeField] private bool drawDebugRays = true;

    // True mientras el Collider de la Sombra esté dentro del SphereCollider de rango (lo controla el propio trigger).
    private bool shadowInsideRange;

    // Estado actual de exposicion, privado a propósito: se modifica solo a traves de SetExposureState para no saltear el evento
    private bool shadowExposed;

    // Lectura publica del estado actual, por si algun script prefiere consultarlo directamente en vez de suscribirse al evento
    public bool ShadowExposed => shadowExposed;

    // Cualquier script puede suscribirse con += en su OnEnable y desuscribirse con -= en su OnDisable, sin acoplarse a este detector. Se dispara
    // solo cuando el estado realmente cambia, no en cada frame.
    public event Action<bool> OnExposureChanged;

    private void Awake()
    {
        // Autocompletar referencias si no se asignaron a mano.
        if (sourceLight == null)
        {
            sourceLight = GetComponent<Light>();
        }

        if (rangeTrigger == null)
        {
            rangeTrigger = GetComponent<SphereCollider>();
        }

        // Sin estos dos componentes el script no puede funcionar, lo desactivamos para no tirar errores en cada frame.
        if (sourceLight == null || rangeTrigger == null)
        {
            Debug.LogError(
                "LightExposureDetector necesita una Light y un SphereCollider.",
                this
            );
            enabled = false;
            return;
        }

        // El radio del trigger sigue automaticamente el Range de la Light
        rangeTrigger.radius = sourceLight.range;
    }

    private void FixedUpdate()
    {
        // Si la Sombra ni siquiera esta en el rango de la luz, no hay que gastar Raycasts, directamente queda protegida
        if (!shadowInsideRange)
        {
            SetExposureState(false);
            return;
        }

        CheckShadowExposure();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignoramos cualquier cosa que no sea la Sombra
        if (!other.CompareTag(shadowTag))
        {
            return;
        }

        shadowInsideRange = true;
        Debug.Log("La Sombra entró al rango de la Luz.", this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(shadowTag))
        {
            return;
        }

        shadowInsideRange = false;

        // Al salir del rango, ya sabemos que esta luz no puede estar iluminandola: no hace falta esperar al próximo FixedUpdate para reflejarlo
        SetExposureState(false);
        Debug.Log("La Sombra salió del rango de la Luz.", this);
    }

    private void CheckShadowExposure()
    {
        int illuminatedCount = 0;
        int validPoints = 0;

        foreach (Transform samplePoint in shadowSamplePoints)
        {
            // Por si algun punto quedo sin asignar en el Inspector
            if (samplePoint == null)
            {
                continue;
            }

            validPoints++;

            bool illuminated = IsPointIlluminated(samplePoint.position);
            if (illuminated)
            {
                illuminatedCount++;
            }

            if (drawDebugRays)
            {
                Debug.DrawLine(
                    sourceLight.transform.position,
                    samplePoint.position,
                    illuminated ? Color.red : Color.green,
                    Time.fixedDeltaTime
                );
            }
        }

        // Regla: solo cuenta como expuesta si el porcentaje de puntos iluminados supera el umbral, con el default (0.5),
        // la mitad todavía se considera protegida
        bool exposed = validPoints > 0
            && (illuminatedCount / (float)validPoints) > exposureThreshold;

        SetExposureState(exposed);
    }

    private bool IsPointIlluminated(Vector3 targetPoint)
    {
        Vector3 origin = sourceLight.transform.position;
        Vector3 toPoint = targetPoint - origin;
        float distance = toPoint.magnitude;

        // Aunque el SphereCollider de rango ya filtro por proximidad general, volvemos a chequear contra el Range real de la
        // Light por si el trigger quedó desincronizado
        if (distance > sourceLight.range)
        {
            return false;
        }

        // Evita dividir por cero si el punto estuviera exactamente sobre la posición de la luz
        if (distance <= Mathf.Epsilon)
        {
            return true;
        }

        Vector3 direction = toPoint / distance;

    
        // Tiramos el rayo desde la luz hasta el punto de muestreo, QueryTriggerInteraction.Ignore hace que colliders marcados
        // como Trigger nunca bloqueen la luz 
        bool blocked = Physics.Raycast(
            origin,
            direction,
            distance,
            blockerMask,
            QueryTriggerInteraction.Ignore
        );

        // Si nada bloqueo el rayo, ese punto esta iluminado
        return !blocked;
    }

    private void SetExposureState(bool exposed)
    {
        // Solo actuamos si el estado realmente cambio, para no spamear el evento ni la consola en cada FixedUpdate
        if (shadowExposed == exposed)
        {
            return;
        }

        shadowExposed = exposed;

        if (shadowExposed)
        {
            Debug.Log("SOMBRA ILUMINADA", this);
        }
        else
        {
            Debug.Log("SOMBRA PROTEGIDA", this);
        }

        // Avisa a quien este suscrito (por ejemplo ShadowLightReactor) en lugar de que ese script tenga que consultar ShadowExposed
        // en su propio Update
        OnExposureChanged?.Invoke(shadowExposed);
    }
}