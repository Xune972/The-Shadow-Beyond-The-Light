using UnityEngine;

public class RobotTensionAudio : MonoBehaviour
{
    public RobotPatrol robot;
    public Transform jugador;
    public AudioSource musicaTension;

    public float distanciaLejana = 20f;
    public float distanciaCercana = 2f;

    public float volumenNormal = 0.1f;
    public float volumenMaximo = 0.85f;

    public float pitchNormal = 1f;
    public float pitchMaximo = 1.18f;

    public float velocidadCambio = 3f;

    void Start()
    {
        if (musicaTension != null)
        {
            musicaTension.loop = true;
            musicaTension.volume = volumenNormal;
            musicaTension.pitch = pitchNormal;
            musicaTension.Play();
        }
    }

    void Update()
    {
        if (robot == null || jugador == null ||
            musicaTension == null)
            return;

        float volumenObjetivo = volumenNormal;
        float pitchObjetivo = pitchNormal;

        if (robot.EstaPersiguiendo)
        {
            float distancia = Vector3.Distance(
                jugador.position, robot.transform.position);

            float intensidad = Mathf.InverseLerp(
                distanciaLejana, distanciaCercana, distancia);

            volumenObjetivo = Mathf.Lerp(
                volumenNormal, volumenMaximo, intensidad);

            pitchObjetivo = Mathf.Lerp(
                pitchNormal, pitchMaximo, intensidad);
        }

        musicaTension.volume = Mathf.Lerp(
            musicaTension.volume, volumenObjetivo,
            Time.deltaTime * velocidadCambio);

        musicaTension.pitch = Mathf.Lerp(
            musicaTension.pitch, pitchObjetivo,
            Time.deltaTime * velocidadCambio);
    }

}
