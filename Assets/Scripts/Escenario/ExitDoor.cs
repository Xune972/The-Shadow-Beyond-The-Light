using Unity.VisualScripting;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [Header("Personaje permitido en la puerta")]
    [SerializeField] private CharacterMovement[] selectedPlayer;
    private void OnTriggerEnter(Collider other)
    {
        CharacterMovement player = other.GetComponent<CharacterMovement>();
        if (player == null) return;
        if(!IsSelectedPlayer (player)) return;
        Debug.Log("El personaje llegó a la puerta", this);
    }

    private bool IsSelectedPlayer(CharacterMovement player)
    {
        foreach (CharacterMovement p in selectedPlayer)
        {
            if(p == player) return true;
        }
        return false;
    }    
}
