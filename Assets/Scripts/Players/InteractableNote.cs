using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableNote : MonoBehaviour
{
    [SerializeField] private NoteUI noteUI;
    [SerializeField] private string playerTag = "ShadowPlayer";

    private bool nearbyPlayer = false;

    private void Update()
    {
        if (nearbyPlayer && Keyboard.current.eKey.wasPressedThisFrame)
            noteUI.Toggle();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag)) nearbyPlayer = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            nearbyPlayer = false;
            noteUI.Hide();
        }
    }
}
