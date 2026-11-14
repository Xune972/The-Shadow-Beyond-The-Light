using UnityEngine;
using TMPro;

public class PromptUI : MonoBehaviour
{
    public static PromptUI Instance { get; private set; }

    [Tooltip("Objeto que contiene el texto (por ejemplo un Panel). Se activa y desactiva.")]
    [SerializeField] private GameObject panel;

    [SerializeField] private TMP_Text label;

  
    private Object currentOwner;

    private void Awake()
    {
        Instance = this;
        if (panel != null) panel.SetActive(false);
    }

    public void Show(Object owner, string message)
    {
        currentOwner = owner;
        if (label != null) label.text = message;
        if (panel != null) panel.SetActive(true);
    }

 
    public void Hide(Object owner)
    {
        if (currentOwner != owner) return;
        currentOwner = null;
        if (panel != null) panel.SetActive(false);
    }
}