using UnityEngine;

public class DamagingLight : MonoBehaviour
{
    [SerializeField] private Light source;
    public Light Source => source; //Luz que daña al jugador
    private void Reset() => source = GetComponent<Light>();
}