using UnityEngine;

public class NoteUI : MonoBehaviour
{
    [SerializeField] private GameObject note;

    private void Start() => note.SetActive(false);

    public void Toggle() => note.SetActive(!note.activeSelf);

    public void Hide() => note.SetActive(false);
}