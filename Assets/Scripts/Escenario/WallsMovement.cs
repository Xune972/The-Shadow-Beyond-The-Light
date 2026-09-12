using UnityEngine;
using System.Collections;
public class WallsMovement : MonoBehaviour
{
    [Header("Color")]
    public Color colorNormal = Color.black;
    public Color colorAdvertencia = Color.red;

    private Renderer[] renders;

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 4f;
    [SerializeField] private float distanciaBajada = 42.7f;

    private Vector3 posicionArriba;
    private Vector3 posicionAbajo;

    private Coroutine movimientoActual;

    void Awake()
    {
        posicionArriba = transform.position;

        posicionAbajo = posicionArriba + Vector3.down * distanciaBajada; 

        renders = GetComponentsInChildren<Renderer>(); 

        
        
    }

    
    void Update()
    {
        
    }

    public void Bajar()
    {
        IniciarMovimiento(posicionAbajo);
    }
    public void Subir()
    {
        PonerNormal();
        IniciarMovimiento(posicionArriba);
    }

    private void IniciarMovimiento(Vector3 destino)
    {
        if(movimientoActual != null)
        {
            StopCoroutine(movimientoActual);
        }
        movimientoActual = StartCoroutine(MoverHacia(destino));
    }

    private IEnumerator MoverHacia(Vector3 destino)
    {
        while(Vector3.Distance(transform.position, destino) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, destino, velocidad * Time.deltaTime);
            yield return null;
        }
        transform.position = destino;
        movimientoActual = null;
    }
    public void PonerRojo()
    {
        foreach (Renderer render in renders)
        {
            render.material.color = colorAdvertencia;
        }
    }

    public void PonerNormal()
    {
        foreach (Renderer render in renders)
        {
            render.material.color = colorNormal;
        }
    }

}
