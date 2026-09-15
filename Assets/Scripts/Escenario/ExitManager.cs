using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitManager : MonoBehaviour
{
    [SerializeField] private CharacterSwitcher characterSwitcher;
    [SerializeField] private string nextEscene;
    [Header("Desactivar el personaje al llegar a la puerta?")]
    [SerializeField] private bool disableCharacter = true;
    [Header("UI")]
    [SerializeField] private GameObject mesajeForCharacter;
    private readonly HashSet<CharacterMovement> charactherArrive = new HashSet<CharacterMovement>(); //Lista de los personajes

    public void NotifyArrive(CharacterMovement character)
    {
        if (charactherArrive.Contains(character)) return;
        charactherArrive.Add(character);
        characterSwitcher.CharacterArrivedExit(character, disableCharacter);
        if (charactherArrive.Count >= characterSwitcher.TotalCharacters) 
        {
            SceneManager.LoadScene(nextEscene);
        }
        else if(mesajeForCharacter != null)
        {
            mesajeForCharacter.SetActive(true);
        }
    }
}