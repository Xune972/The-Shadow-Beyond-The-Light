using UnityEngine;

public class CharacterSwitcher : MonoBehaviour
{
    [Header("Characters")]
    [SerializeField] private CharacterMovement[] characters;
    [SerializeField] private int startingCharacterIndex = 0;

    [Header("Camera")]
    [SerializeField] private CameraTargetSwitcher cameraTargetSwitcher;

    [Header("Input")]
    [SerializeField] private KeyCode switchKey = KeyCode.Tab;

    private int currentCharacterIndex;
    public int TotalCharacters => characters.Length;

    public CharacterMovement ActiveCharacter =>
        characters[currentCharacterIndex];

    private void Awake()
    {
        if (characters == null || characters.Length == 0)
        {
            Debug.LogError(
                "CharacterSwitcher necesita al menos un personaje.",
                this
            );

            enabled = false;
            return;
        }

        currentCharacterIndex = Mathf.Clamp(
            startingCharacterIndex,
            0,
            characters.Length - 1
        );

        UpdateActiveCharacter();
    }

    private void Update()
    {
        if (Input.GetKeyDown(switchKey))
            SwitchCharacter();
    }

    private void SwitchCharacter()
    {
        int trys = 0;
        do
        {
            currentCharacterIndex = (currentCharacterIndex + 1) % characters.Length;
            trys++;
        }
        while (!characters[currentCharacterIndex].gameObject.activeSelf); // Busca un pj activo para hacer el cambio

        UpdateActiveCharacter();
    }

    private void UpdateActiveCharacter()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] == null)
                continue;

            characters[i].SetControlsEnabled(
                i == currentCharacterIndex
            );
        }

        UpdateCameraTarget();
    }

    private void UpdateCameraTarget()
    {
        if (cameraTargetSwitcher == null)
            return;

        CharacterMovement activeCharacter =
            characters[currentCharacterIndex];

        if (activeCharacter == null)
            return;

        cameraTargetSwitcher.SetTarget(
            activeCharacter.CameraTarget
        );
    }

    public void CharacterArrivedExit(CharacterMovement character, bool disableCharacter)
    {
        if (character == ActiveCharacter)
        {
            SwitchToOther(character);
        }
        character.SetControlsEnabled( false );
        if (disableCharacter)
        {
            character.gameObject.SetActive(false);
        }
    }

    private void SwitchToOther(CharacterMovement characterLeave) // Detecta al personaje que no fue a la puerta para seleccionarlo
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] != null && characters[i] != characterLeave && characters[i].gameObject.activeSelf)
            {
                currentCharacterIndex = i;
                UpdateActiveCharacter();
                return;
            }
        }
    }
}