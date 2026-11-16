using UnityEngine;

public class NoteZoneTrigger : MonoBehaviour
{
    [Tooltip("Nombre de la zona. Tiene que coincidir con el Zone Id de las notas.")]
    [SerializeField] private string zoneId = "Zona1";

    [Tooltip("Tags de los personajes que activan la zona.")]
    [SerializeField] private string[] playerTags = { "ShadowPlayer", "Player" };

    private void OnTriggerEnter(Collider other)
    {
        foreach (string tag in playerTags)
        {
            if (!string.IsNullOrEmpty(tag) && other.CompareTag(tag))
            {
                NoteProgress.SetCurrentZone(zoneId);
                return;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Collider c = GetComponent<Collider>();
        if (c == null) return;

        Gizmos.color = new Color(0f, 1f, 0.5f, 0.25f);
        Gizmos.DrawCube(c.bounds.center, c.bounds.size);
    }
}