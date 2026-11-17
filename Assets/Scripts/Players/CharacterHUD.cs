using UnityEngine;
using UnityEngine.UI;

public class CharacterHUD : MonoBehaviour
{
    [SerializeField] private CharacterSwitcher switcher;
    [SerializeField] private Image portrait;
    [SerializeField] private Image healthFill;
    [SerializeField] private Sprite[] portraits; //mismo orden que los personajes del switcher

    private ShadowPlayerHealth current;

    private void OnEnable() => switcher.OnCharacterChanged += SetCharacter;
    private void OnDisable()
    {
        switcher.OnCharacterChanged -= SetCharacter;
        if (current != null) current.OnHealthChanged -= SetFill;
    }

    private void Start() => SetCharacter(switcher.CurrentIndex);

    private void SetCharacter(int index)
    {
        portrait.sprite = portraits[index];

        if (current != null) current.OnHealthChanged -= SetFill;
        current = switcher.ActiveCharacter.GetComponent<ShadowPlayerHealth>();

        if (current != null)
        {
            current.OnHealthChanged += SetFill;
            SetFill(current.HealthPercent);
        }
        else SetFill(1f); //la luz no tiene barra de vida: se ve llena
    }

    private void SetFill(float percent) => healthFill.fillAmount = percent;
}