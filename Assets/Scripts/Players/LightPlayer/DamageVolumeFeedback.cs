using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DamageVolumeFeedback : MonoBehaviour
{
    [SerializeField] private Volume volume;
    [SerializeField] private ShadowPlayerHealth shadowHealth;
    [SerializeField] private Vector2 vignetteRange = new Vector2(0.25f, 0.6f);
    [SerializeField] private Vector2 aberrationRange = new Vector2(0f, 0.8f);
    [SerializeField] private float smooth = 6f;

    private Vignette vignette;
    private ChromaticAberration aberration;
    private float danger;
    private float shown;

    private void Awake()
    {
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out aberration);
    }

    private void OnEnable() { if (shadowHealth != null) shadowHealth.OnHealthChanged += SetHealth; }
    private void OnDisable() { if (shadowHealth != null) shadowHealth.OnHealthChanged -= SetHealth; }

    private void SetHealth(float percent) => danger = 1f - percent;

    private void Update()
    {
        shown = Mathf.Lerp(shown, danger, smooth * Time.deltaTime);
        if (vignette != null)
            vignette.intensity.Override(Mathf.Lerp(vignetteRange.x, vignetteRange.y, shown));
        if (aberration != null)
            aberration.intensity.Override(Mathf.Lerp(aberrationRange.x, aberrationRange.y, shown));
    }
}