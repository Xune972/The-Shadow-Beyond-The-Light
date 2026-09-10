using System;
using UnityEngine;

public class LightExposureDetector : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Light sourceLight;
    [SerializeField] private SphereCollider rangeTrigger;

    [Header("Puntos de muestreo de la Sombra")]
    [SerializeField] private Transform[] shadowSamplePoints;

    [Header("Tolerancia")]
    [Range(0f, 1f)]
    [SerializeField] private float exposureThreshold = 0.5f;

    [Header("Detección")]
    [SerializeField] private string shadowTag = "Shadow";
    [SerializeField] private LayerMask blockerMask;

    [Header("Spot Light")]
    [SerializeField] private bool checkSpotAngle = true;

    [Header("Debug")]
    [SerializeField] private bool drawDebugRays = true;

    private bool shadowInsideRange;
    private bool shadowExposed;

    public bool ShadowExposed => shadowExposed;
    public event Action<bool> OnExposureChanged;

    private void Awake()
    {
        if (sourceLight == null) sourceLight = GetComponent<Light>();
        if (rangeTrigger == null) rangeTrigger = GetComponent<SphereCollider>();

        if (sourceLight == null || rangeTrigger == null)
        {
            Debug.LogError($"{name}: Necesita un componente Light y un SphereCollider.", this);
            enabled = false;
            return;
        }

        rangeTrigger.isTrigger = true;
        rangeTrigger.radius = sourceLight.range;

        // Búsqueda de respaldo si no se asignaron los puntos manualmente en el Inspector
        if (shadowSamplePoints == null || shadowSamplePoints.Length == 0)
        {
            GameObject shadowObj = GameObject.FindWithTag(shadowTag);
            if (shadowObj != null)
            {
                shadowSamplePoints = shadowObj.GetComponentsInChildren<Transform>();
            }
        }
    }

    private void FixedUpdate()
    {
        // Actualización dinámica del radio por si cambia el Range de la luz en juego
        if (rangeTrigger != null && sourceLight != null && rangeTrigger.radius != sourceLight.range)
        {
            rangeTrigger.radius = sourceLight.range;
        }

        if (!shadowInsideRange)
        {
            SetExposureState(false);
            return;
        }

        CheckShadowExposure();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(shadowTag)) return;

        shadowInsideRange = true;
        Debug.Log("La Sombra entró al rango de la Luz.", this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(shadowTag)) return;

        shadowInsideRange = false;
        SetExposureState(false);
        Debug.Log("La Sombra salió del rango de la Luz.", this);
    }

    private void CheckShadowExposure()
    {
        if (shadowSamplePoints == null || shadowSamplePoints.Length == 0) return;

        int illuminatedCount = 0;
        int validPoints = 0;

        foreach (Transform samplePoint in shadowSamplePoints)
        {
            if (samplePoint == null) continue;

            validPoints++;

            bool illuminated = IsPointIlluminated(samplePoint.position);
            if (illuminated) illuminatedCount++;

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

        bool exposed = validPoints > 0 && ((float)illuminatedCount / validPoints) > exposureThreshold;
        SetExposureState(exposed);
    }

    private bool IsPointIlluminated(Vector3 targetPoint)
    {
        Vector3 origin = sourceLight.transform.position;
        Vector3 toPoint = targetPoint - origin;
        float distance = toPoint.magnitude;

        // 1. Chequeo de rango de distancia
        if (distance > sourceLight.range) return false;
        if (distance <= Mathf.Epsilon) return true;

        Vector3 direction = toPoint / distance;

        // 2. CORREGIDO: Chequeo de ángulo para Spot Lights
        if (sourceLight.type == LightType.Spot && checkSpotAngle)
        {
            float angleToTarget = Vector3.Angle(sourceLight.transform.forward, direction);

            // Si el punto está fuera del cono de la linterna, está a oscuras
            if (angleToTarget > (sourceLight.spotAngle * 0.5f))
            {
                return false;
            }
        }

        // 3. Chequeo de obstáculos por Raycast
        bool blocked = Physics.Raycast(
            origin,
            direction,
            distance,
            blockerMask,
            QueryTriggerInteraction.Ignore
        );

        return !blocked;
    }

    private void SetExposureState(bool exposed)
    {
        if (shadowExposed == exposed) return;

        shadowExposed = exposed;

        if (shadowExposed)
            Debug.Log("SOMBRA ILUMINADA", this);
        else
            Debug.Log("SOMBRA PROTEGIDA", this);

        OnExposureChanged?.Invoke(shadowExposed);
    }
}