using UnityEngine;
using System.Collections;
public class WallsMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 4f;
    [SerializeField] private float distanciaBajada = 42.7f;

    private Vector3 posicionArriba;
    private Vector3 posicionAbajo;

    private Coroutine movimientoActual;

    void Start()
    {
        posicionArriba = transform.position;

        posicionAbajo = posicionArriba + Vector3.down * distanciaBajada; 
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
    
}
