using System.Collections;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    [Header("Referencias Específicas")]
    [Tooltip("Arrastra aquí ÚNICAMENTE al personaje de sombras.")]
    [SerializeField] private GameObject shadowPlayer;

    [Tooltip("Punto exacto donde reaparecerá el personaje de sombras.")]
    [SerializeField] private Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        // Solo activa el respawn si el objeto que cayó es el personaje de sombras asignado
        // (o si tocó un colisionador hijo de ese personaje)
        if (shadowPlayer != null && (other.gameObject == shadowPlayer || other.transform.IsChildOf(shadowPlayer.transform)))
        {
            RespawnPlayer(shadowPlayer);
        }
    }

    private void RespawnPlayer(GameObject player)
    {
        StartCoroutine(RespawnRoutine(player));
    }

    private IEnumerator RespawnRoutine(GameObject player)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        CharacterMovement movement = player.GetComponent<CharacterMovement>();

        // 1. Bloquear controles
        if (movement != null)
        {
            movement.SetControlsEnabled(false);
        }

        if (rb != null)
        {
            // Primero limpiamos la velocidad (todavía NO kinemático)
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            // Recién ahora lo pasamos a kinemático
            rb.isKinematic = true;
        }

        // 2. Mover al punto de reaparición
        if (respawnPoint != null)
        {
            player.transform.position = respawnPoint.position;
            player.transform.rotation = respawnPoint.rotation;
        }

        // 3. Esperar al ciclo de físicas
        yield return new WaitForFixedUpdate();

        // 4. Reactivar físicas y recién después limpiar inercia remanente
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (movement != null)
        {
            movement.SetControlsEnabled(true);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position, transform.lossyScale);
    }
}