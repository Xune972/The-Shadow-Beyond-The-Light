using System.Collections;
using UnityEngine;

public class ShadowPlayerHealth : MonoBehaviour
{
    [Header("Referencias de Luz")]
    [SerializeField] private LightExposureDetector lightDetector;

    [Header("Ajustes de Vida")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float damagePerSecond = 35f; // Se muere en ~3 segundos bajo la luz
    [SerializeField] private float healPerSecond = 20f;   // Se cura en ~5 segundos en la sombra

    [Header("Efecto Visual (Transparencia)")]
    [SerializeField] private Renderer characterRenderer;

    [Header("Referencia a Muerte")]
    [SerializeField] private KillZone killZone;

    private float currentHealth;
    private bool isExposedToLight;
    private Material characterMaterial;
    private Color originalColor;

    [Header("Audio - Latido")]
    [SerializeField] private AudioSource heartbeatSource;
    [SerializeField] private AudioClip heartbeatClip;
    [SerializeField] private Vector2 volumeRange = new Vector2(0.4f, 1f);  // vida llena -> vida casi en 0
    [SerializeField] private Vector2 pitchRange = new Vector2(1f, 1.4f);   // el latido se acelera
    [SerializeField] private float fadeSpeed = 4f;

    private float heartbeatFade;        

    public float CurrentHealth => currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;

        // Asignar el Renderer si no se arrastró manualmente
        if (characterRenderer == null)
            characterRenderer = GetComponentInChildren<Renderer>();

        if (characterRenderer != null)
        {
            // Creamos una instancia única del material para este personaje
            characterMaterial = characterRenderer.material;
            originalColor = characterMaterial.color;
        }

        // Buscar la KillZone en escena si no está asignada
        if (killZone == null)
            killZone = FindFirstObjectByType<KillZone>();

        if (heartbeatSource != null)
        {
            heartbeatSource.clip = heartbeatClip;
            heartbeatSource.loop = true;
            heartbeatSource.playOnAwake = false;
            heartbeatSource.spatialBlend = 0f; // 2D
        }
        else Debug.LogWarning("ShadowPlayerHealth: falta asignar Heartbeat Source", this);
    }

    private void UpdateHeartbeat()
    {
        if (heartbeatSource == null || heartbeatClip == null) return;

       
        float target = isExposedToLight ? 1f : 0f;
        heartbeatFade = Mathf.MoveTowards(heartbeatFade, target, fadeSpeed * Time.deltaTime);

        if (heartbeatFade > 0f)
        {
            if (!heartbeatSource.isPlaying) heartbeatSource.Play();

           
            float danger = 1f - (currentHealth / maxHealth);

            heartbeatSource.volume = Mathf.Lerp(volumeRange.x, volumeRange.y, danger) * heartbeatFade;
            heartbeatSource.pitch = Mathf.Lerp(pitchRange.x, pitchRange.y, danger);
        }
        else if (heartbeatSource.isPlaying)
        {
            heartbeatSource.Stop();
        }
    }

    private void OnEnable()
    {
        if (lightDetector != null)
            lightDetector.OnExposureChanged += HandleExposureChanged;
    }

    private void OnDisable()
    {
        if (lightDetector != null)
            lightDetector.OnExposureChanged -= HandleExposureChanged;
        if (heartbeatSource != null) heartbeatSource.Stop();
    }

    private void HandleExposureChanged(bool isExposed)
    {
        isExposedToLight = isExposed;
    }

    private void Update()
    {
        HandleHealthAndFade();
        UpdateHeartbeat();
    }

    private void HandleHealthAndFade()
    {
        // 1. Modificar la vida progresivamente
        if (isExposedToLight)
        {
            currentHealth -= damagePerSecond * Time.deltaTime;
        }
        else if (currentHealth < maxHealth)
        {
            currentHealth += healPerSecond * Time.deltaTime;
        }

        // Limitar el rango entre 0 y el máximo
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // 2. Modificar el Alpha (transparencia) del material
        UpdateMaterialAlpha();

        // 3. Evaluar Muerte
        if (currentHealth <= 0f)
        {
            TriggerDeath();
        }
    }

    private void UpdateMaterialAlpha()
    {
        if (characterMaterial == null) return;

        float alphaPercentage = currentHealth / maxHealth;
        Color newColor = originalColor;

        // La opacidad disminuye en paralelo a la vida restante
        newColor.a = alphaPercentage;
        characterMaterial.color = newColor;
    }

    private void TriggerDeath()
    {
        // Restablecer valores de salud y transparencia para el reaparecer
        currentHealth = maxHealth;
        UpdateMaterialAlpha();
        heartbeatFade = 0f;
        if (heartbeatSource != null) heartbeatSource.Stop();

        // Reutilizar el Respawn limpio configurado en la KillZone
        if (killZone != null)
        {
            killZone.SendMessage("RespawnPlayer", gameObject, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.LogWarning("No se encontró la KillZone para procesar el Respawn por luz.", this);
        }
    }
}