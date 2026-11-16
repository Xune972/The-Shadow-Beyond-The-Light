using UnityEngine;
using TMPro;


public class NoteCounterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    [Tooltip("{0} = notas leídas, {1} = total de la zona.")]
    [SerializeField] private string format = "Notas: {0}/{1}";

    private void OnEnable()
    {
        NoteProgress.Changed += Refresh;
        Refresh();
    }

    private void Start()
    {
        
        Refresh();
    }

    private void OnDisable()
    {
        NoteProgress.Changed -= Refresh;
    }

    private void Refresh()
    {
        if (label == null) return;

        string zone = NoteProgress.CurrentZone;
        int total = NoteProgress.GetTotal(zone);

        if (zone == null || total == 0)
        {
            label.text = "";
            return;
        }

        int collected = NoteProgress.GetCollected(zone);
        label.text = format
            .Replace("{0}", collected.ToString())
            .Replace("{1}", total.ToString());
    }
}