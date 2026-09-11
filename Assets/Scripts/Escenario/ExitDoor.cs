using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private ExitManager exitManager;
    [Header("Personaje permitido en la puerta")]
    [SerializeField] private CharacterMovement[] selectedCharacter;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HitBox"))
        {
            CharacterMovement character = other.GetComponentInParent<CharacterMovement>();
            if (character == null) return;
            if (!IsSelectedCharacter(character)) return;
            Debug.Log("El personaje llegó a la puerta", this);
            exitManager.NotifyArrive(character);
        }
    }

    private bool IsSelectedCharacter(CharacterMovement character)
    {
        foreach (CharacterMovement c in selectedCharacter)
        {
            if(c == character) return true;
        }
        return false;
    }    
}