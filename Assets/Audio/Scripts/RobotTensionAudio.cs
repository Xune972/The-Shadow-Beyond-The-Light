using UnityEngine;

public class RobotTensionAudio : MonoBehaviour
{
    public RobotPatrol[] robots;
    public AudioSource musicaTension;

    [Header("Distancias")]
    public float distanciaLejana = 20f;
    public float distanciaCercana = 2f;

    [Header("Volumen")]
    public float volumenNormal = 0.1f;
    public float volumenMaximo = 0.85f;

    [Header("Pitch")]
    public float pitchNormal = 1f;
    public float pitchMaximo = 1.18f;

    public float velocidadCambio = 3f;

    void Start()
    {
        if (musicaTension == null) return;

        musicaTension.loop = true;
        musicaTension.volume = volumenNormal;
        musicaTension.pitch = pitchNormal;
        musicaTension.Play();
    }

    void Update()
    {
        if (musicaTension == null) return;

        float distanciaMinima = distanciaLejana;
        bool hayPersecucion = false;

        foreach (RobotPatrol robot in robots)
        {
            if (robot == null || !robot.EstaPersiguiendo)
                continue;

            Transform objetivo = robot.ObjetivoActual;

            if (objetivo == null) continue;

            float distancia = Vector3.Distance(
                robot.transform.position,
                objetivo.position
            );

            if (!hayPersecucion || distancia < distanciaMinima)
                distanciaMinima = distancia;

            hayPersecucion = true;
        }

        float intensidad = 0f;

        if (hayPersecucion)
        {
            intensidad = Mathf.InverseLerp(
                distanciaLejana,
                distanciaCercana,
                distanciaMinima
            );
        }

        float volumenObjetivo = Mathf.Lerp(
            volumenNormal, volumenMaximo, intensidad
        );

        float pitchObjetivo = Mathf.Lerp(
            pitchNormal, pitchMaximo, intensidad
        );

        musicaTension.volume = Mathf.Lerp(
            musicaTension.volume,
            volumenObjetivo,
            Time.deltaTime * velocidadCambio
        );

        musicaTension.pitch = Mathf.Lerp(
            musicaTension.pitch,
            pitchObjetivo,
            Time.deltaTime * velocidadCambio
        );
    }
}
