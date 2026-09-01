using UnityEngine;

public class CharacterSwitcher : MonoBehaviour
{
    [Header("Characters")]
    [SerializeField] private CharacterMovement[] characters;
    [SerializeField] private int startingCharacterIndex = 0;

    [Header("Input")]
    [SerializeField] private KeyCode switchKey = KeyCode.Tab;

    private int currentCharacterIndex;

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
        currentCharacterIndex++;

        if (currentCharacterIndex >= characters.Length)
            currentCharacterIndex = 0;

        UpdateActiveCharacter();
    }


    private void UpdateActiveCharacter()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] == null)
                continue;

            bool shouldHaveControl =
                i == currentCharacterIndex;

            characters[i].SetControlsEnabled(
                shouldHaveControl
            );
        }
    }
}