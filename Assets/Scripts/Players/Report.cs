using UnityEngine;

public class No : MonoBehaviour
{
    [SerializeField] private GameObject Note;
    private bool nearbyPlayer=false;

    private void Start()
    {
        Note.SetActive(false);
    }
    void Update()
    {
       if(nearbyPlayer&&Input.GetKeyDown(KeyCode.R))
        {
            if(Note.activeSelf==false)
            {
                Note.SetActive(true);
            }
            else
            {
                Note.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ShadowPlayer"))
        {
            nearbyPlayer=true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ShadowPlayer"))
        {
            nearbyPlayer = false;
            Note.SetActive(false);

        }
    }

}
