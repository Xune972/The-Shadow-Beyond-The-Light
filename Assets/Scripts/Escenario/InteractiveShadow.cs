using System.Linq;
using UnityEngine;

public class InteractiveShadow : MonoBehaviour
{
    [Header("Referencias de Escena")]
    public Transform lightTransform;       // Asigna la Luz (Spotlight o Directional)
    public LayerMask targetLayerMask;      // Capa de la pared/suelo donde se proyecta la sombra

    [Header("Ajustes de Extrusión y Física")]
    [Tooltip("Grosor de la plataforma de sombra en la pared para que el personaje pueda pisarla adecuadamente.")]
    public float shadowThickness = 0.5f;

    [Header("Ajustes de Rendimiento")]
    [Range(0.02f, 0.2f)]
    public float updateInterval = 0.05f;   // Frecuencia de actualización de la sombra física

    private GameObject shadowObject;
    private MeshCollider shadowCollider;
    private Mesh shadowMesh;
    private Vector3[] baseVertices;
    private bool canUpdate = true;

    // Guardar estado previo para optimización
    private Vector3 lastPos;
    private Quaternion lastRot;
    private Vector3 lastScale;
    private Vector3 lastLightPos;

    private void Awake()
    {
        // 1. Crear el objeto contenedor del collider de la sombra
        shadowObject = new GameObject("Shadow_Collider_" + gameObject.name);
        shadowCollider = shadowObject.AddComponent<MeshCollider>();
        shadowCollider.convex = true; // Necesario para colliders dinámicos sin rigidbodies complejos
        shadowObject = new GameObject("Shadow_Collider_" + gameObject.name);
        shadowObject.layer = LayerMask.NameToLayer("ShadowCollider");

        // 2. Extraer vértices únicos del MeshFilter del objeto
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            baseVertices = meshFilter.sharedMesh.vertices.Distinct().ToArray();
        }

        shadowMesh = new Mesh();
    }

    private void FixedUpdate()
    {
        if (HasChanged() && canUpdate)
        {
            canUpdate = false;
            Invoke(nameof(UpdateShadowCollider), updateInterval);
        }
    }

    private void UpdateShadowCollider()
    {
        if (baseVertices == null || baseVertices.Length == 0 || lightTransform == null) return;

        Vector3[] shadowVertices = new Vector3[baseVertices.Length * 2];

        for (int i = 0; i < baseVertices.Length; i++)
        {
            // Posición real del vértice en el mundo 3D
            Vector3 worldVert = transform.TransformPoint(baseVertices[i]);

            // Dirección del rayo desde la luz hacia el vértice
            Vector3 rayDir = (worldVert - lightTransform.position).normalized;

            // Lanzar Raycast para encontrar la pared/piso de la izquierda
            if (Physics.Raycast(worldVert, rayDir, out RaycastHit hit, 100f, targetLayerMask))
            {
                // Convertir la coordenada del impacto en la pared al espacio local del shadowObject
                Vector3 localHitPoint = shadowObject.transform.InverseTransformPoint(hit.point);

                // Vértice en la superficie de la pared
                shadowVertices[i] = localHitPoint;

                // Vértice extruido hacia afuera (darle grosor para que no sea un plano infinitamente delgado)
                Vector3 localNormal = shadowObject.transform.InverseTransformDirection(hit.normal);
                shadowVertices[i + baseVertices.Length] = localHitPoint + (localNormal * shadowThickness);
            }
            else
            {
                // Si el rayo no toca pared, se coloca fuera de la vista
                Vector3 fallbackPoint = worldVert + rayDir * 20f;
                shadowVertices[i] = shadowObject.transform.InverseTransformPoint(fallbackPoint);
                shadowVertices[i + baseVertices.Length] = shadowVertices[i];
            }
        }

        // Reconstruir la malla física
        shadowMesh.Clear();
        shadowMesh.vertices = shadowVertices;

        // Asignar al MeshCollider para que recalcule la geometría convexa
        shadowCollider.sharedMesh = null; // Forzar refresco en la física de Unity
        shadowCollider.sharedMesh = shadowMesh;

        // Guardar transformaciones actuales para detectar si hubo movimiento en el siguiente frame
        lastPos = transform.position;
        lastRot = transform.rotation;
        lastScale = transform.lossyScale;
        lastLightPos = lightTransform.position;

        canUpdate = true;
    }

    private bool HasChanged()
    {
        return transform.position != lastPos ||
               transform.rotation != lastRot ||
               transform.lossyScale != lastScale ||
               lightTransform.position != lastLightPos;
    }

    private void OnDestroy()
    {
        // Limpieza de memoria si el objeto se destruye
        if (shadowObject != null)
        {
            Destroy(shadowObject);
        }
    }
}