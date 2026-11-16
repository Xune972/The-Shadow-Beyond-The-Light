using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PromptUI : MonoBehaviour
{
    public static PromptUI Instance { get; private set; }

    [Tooltip("Objeto que contiene el texto (por ejemplo un Panel). Se activa y desactiva.")]
    [SerializeField] private GameObject panel;

    [SerializeField] private TMP_Text label;

    private readonly List<Object> owners = new List<Object>();
    private readonly List<string> messages = new List<string>();

    private void Awake()
    {
        Instance = this;
        if (panel != null) panel.SetActive(false);
    }

    public void Show(Object owner, string message)
    {
        int i = owners.IndexOf(owner);
        if (i >= 0)
        {
            owners.RemoveAt(i);
            messages.RemoveAt(i);
        }

        owners.Add(owner);
        messages.Add(message);
        Refresh();
    }

    public void Hide(Object owner)
    {
        int i = owners.IndexOf(owner);
        if (i < 0) return;

        owners.RemoveAt(i);
        messages.RemoveAt(i);
        Refresh();
    }

    private void Refresh()
    {
        if (owners.Count == 0)
        {
            if (panel != null) panel.SetActive(false);
            return;
        }

        if (label != null) label.text = messages[messages.Count - 1];
        if (panel != null) panel.SetActive(true);
    }
}
