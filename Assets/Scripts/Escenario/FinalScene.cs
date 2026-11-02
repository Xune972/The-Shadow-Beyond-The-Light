using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class FinalScene : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private float startDelay = 2f;
    [SerializeField] private float endDelay = 3f;
    [SerializeField] private string menuSceneName = "Menu";

    private void OnEnable()
    {
        if (director != null)
            director.stopped += OnFinished;
    }

    private void OnDisable()
    {
        if (director != null)
            director.stopped -= OnFinished;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(startDelay);
        director.Play();
    }

    private void OnFinished(PlayableDirector d)
    {
        if (!isActiveAndEnabled)
            return;

        StartCoroutine(LoadMenu());
    }

    private IEnumerator LoadMenu()
    {
        yield return new WaitForSeconds(endDelay);
        SceneManager.LoadScene(menuSceneName);
    }
}
