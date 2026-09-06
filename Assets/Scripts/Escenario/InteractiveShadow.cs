using System.Linq;
using UnityEngine;

public class InteractiveShadow : MonoBehaviour
{
    [Header("Referencias de Escena")]
    public Transform lightTransform;       // Asigna la Luz (Spotlight o Directional)
    public LayerMask targetLayerMask;      // Capa de las paredes/suelo donde se proyecta la sombra

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

    private void Awake()
    {
        // 1. Crear el objeto que contendrá el Collider de la sombra
        shadowObject = new GameObject("Shadow_Collider_" + gameObject.name);
        shadowCollider = shadowObject.AddComponent<MeshCollider>();
        shadowCollider.convex = true; // Permite generar la física de forma limpia

        // 2. Extraer vértices únicos del objeto para evitar raycasts redundantes
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            baseVertices = meshFilter.sharedMesh.vertices.Distinct().ToArray();
        }

        shadowMesh = new Mesh();
    }

    private void FixedUpdate()
    {
        // Solo actualizar el colisionador si el objeto o la luz se han movido
        if (HasChanged() && canUpdate)
        {
            canUpdate = false;
            Invoke(nameof(UpdateShadowCollider), updateInterval);
        }
    }

    private void UpdateShadowCollider()
    {
        if (baseVertices == null || baseVertices.Length == 0) return;

        Vector3[] shadowVertices = new Vector3[baseVertices.Length * 2];
        Vector3 lightDir = (transform.position - lightTransform.position).normalized;

        for (int i = 0; i < baseVertices.Length; i++)
        {
            // Transformar vértice local a posición en el mundo
            Vector3 worldVert = transform.TransformPoint(baseVertices[i]);

            // Si es un Spotlight, la dirección varía por vértice
            Vector3 rayDir = (worldVert - lightTransform.position).normalized;

            // Lanzar Raycast hacia las paredes o suelo
            if (Physics.Raycast(worldVert, rayDir, out RaycastHit hit, 100f, targetLayerMask))
            {
                shadowVertices[i] = hit.point;
                shadowVertices[i + baseVertices.Length] = worldVert; // Unir con el punto de origen
            }
            else
            {
                shadowVertices[i] = worldVert + rayDir * 50f;
                shadowVertices[i + baseVertices.Length] = worldVert;
            }
        }

        // Recalcular la malla física de la sombra
        shadowMesh.Clear();
        shadowMesh.vertices = shadowVertices;
        shadowCollider.sharedMesh = shadowMesh;

        // Actualizar posiciones guardadas
        lastPos = transform.position;
        lastRot = transform.rotation;
        lastScale = transform.lossyScale;

        canUpdate = true;
    }

    private bool HasChanged()
    {
        return transform.position != lastPos ||
               transform.rotation != lastRot ||
               transform.lossyScale != lastScale ||
               lightTransform.hasChanged;
    }
}