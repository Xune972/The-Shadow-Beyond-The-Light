using UnityEngine;
using System;
using System.Collections.Generic;

public class LightExposureDetector : MonoBehaviour
{
    [Header("Punto de detección")]
    [Tooltip("Punto desde el cual se comprueba si el personaje está iluminado. Si se deja vacío, usa este mismo transform.")]
    [SerializeField] private Transform puntoDeteccion;

    [Header("Obstáculos")]
    [Tooltip("Capas que bloquean la luz (paredes, obstáculos)")]
    [SerializeField] private LayerMask capaObstaculos;

    [Header("Actualización")]
    [Tooltip("Cada cuántos segundos se refresca la lista de luces de la escena (por si aparecen o desaparecen nuevas)")]
    [SerializeField] private float intervaloRefrescoLuces = 2f;

    public event Action<bool> OnExposureChanged;
    public bool IsExposed { get; private set; }

    private List<Light> luces = new List<Light>();
    private float temporizadorRefresco;

    private void Awake()
    {
        if (puntoDeteccion == null) puntoDeteccion = transform;
        RefrescarLuces();
    }

    private void Update()
    {
        temporizadorRefresco -= Time.deltaTime;
        if (temporizadorRefresco <= 0f)
        {
            RefrescarLuces();
            temporizadorRefresco = intervaloRefrescoLuces;
        }

        bool expuestoAhora = ComprobarSiEstaIluminado();

        if (expuestoAhora != IsExposed)
        {
            IsExposed = expuestoAhora;
            OnExposureChanged?.Invoke(IsExposed);
        }
    }

    private void RefrescarLuces()
    {
        luces.Clear();
        luces.AddRange(FindObjectsByType<Light>(FindObjectsSortMode.None));
    }

    private bool ComprobarSiEstaIluminado()
    {
        foreach (Light luz in luces)
        {
            if (luz == null || !luz.enabled) continue;

            if (EstaDentroDelAlcance(luz) && HayLineaDeVista(luz.transform.position))
            {
                return true;
            }
        }
        return false;
    }

    private bool EstaDentroDelAlcance(Light luz)
    {
        float distancia = Vector3.Distance(luz.transform.position, puntoDeteccion.position);
        if (distancia > luz.range) return false;

        if (luz.type == LightType.Spot)
        {
            Vector3 direccionHaciaPersonaje = (puntoDeteccion.position - luz.transform.position).normalized;
            float angulo = Vector3.Angle(luz.transform.forward, direccionHaciaPersonaje);
            if (angulo > luz.spotAngle * 0.5f) return false;
        }

        return true;
    }

    private bool HayLineaDeVista(Vector3 origenLuz)
    {
        Vector3 direccion = puntoDeteccion.position - origenLuz;
        float distancia = direccion.magnitude;

        if (Physics.Raycast(origenLuz, direccion.normalized, out RaycastHit hit, distancia, capaObstaculos))
        {
            return false;
        }
        return true;
    }
}