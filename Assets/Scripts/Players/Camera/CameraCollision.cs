using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [SerializeField] private Transform orbitPivot;
    [SerializeField] private float maxDistance = 4f;
    [SerializeField] private float radius = 0.3f;
    [SerializeField] private LayerMask collisionMask;

    private void LateUpdate()
    {
        Vector3 dir = -orbitPivot.forward;
        float distance = maxDistance;

        if (Physics.SphereCast(orbitPivot.position, radius, dir,
            out RaycastHit hit, maxDistance, collisionMask,
            QueryTriggerInteraction.Ignore))
        {
            distance = hit.distance;
        }

        transform.position = orbitPivot.position + dir * distance;
        transform.rotation = orbitPivot.rotation;
    }
}