using UnityEngine;
using System.Collections;
public class WallsController : MonoBehaviour
{
    [Header("Paredes")]
    public WallsMovement pared1;
    public WallsMovement pared2;
    public WallsMovement pared3;
    public WallsMovement pared4;

    [Header("Tiempos")]
    public float tiempoRojo = 0.5f;
    public float tiempoEntreMovimientos = 1f;
    void Start()
    {
        StartCoroutine(CicloParedes());
    }

   
    void Update()
    {
        
    }

    IEnumerator CicloParedes()
    {
        while(true)
        {
            yield return new WaitForSeconds(tiempoRojo);

            pared1.Bajar();
            pared2.Bajar();

            yield return new WaitForSeconds(tiempoEntreMovimientos);
            pared1.Subir();
            pared2.Subir();
            pared3.Bajar();
            pared4.Bajar();

            yield return new WaitForSeconds(tiempoEntreMovimientos);
            pared3.Subir();
            pared4.Subir();

            yield return new WaitForSeconds(tiempoEntreMovimientos);
            pared3.Bajar();
            pared1.Bajar();

            yield return new WaitForSeconds(tiempoEntreMovimientos);
            pared2.Bajar();
            pared4.Bajar();
            pared3.Subir();
            pared1.Subir();

            yield return new WaitForSeconds(tiempoEntreMovimientos);
        }
    }
}
