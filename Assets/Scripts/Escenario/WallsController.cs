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
    public float tiempoRojo = 1.5f;
    public float tiempoEntreMovimientos = 11f;
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
            pared1.PonerRojo();
            pared2.PonerRojo();
            yield return new WaitForSeconds(tiempoRojo);
            
            pared1.Bajar();
            pared2.Bajar();

            yield return new WaitForSeconds(tiempoEntreMovimientos);
            pared1.Subir();
            pared2.Subir();

            pared3.PonerRojo();
            pared4.PonerRojo();
            yield return new WaitForSeconds(tiempoRojo);
          

            pared3.Bajar();
            pared4.Bajar();

            yield return new WaitForSeconds(tiempoEntreMovimientos);
            pared3.Subir();
            pared4.Subir();

            yield return new WaitForSeconds(tiempoEntreMovimientos);

            pared3.PonerRojo();
            pared1.PonerRojo();

            yield return new WaitForSeconds(tiempoRojo);
            pared3.Bajar();
            pared1.Bajar();

           

            yield return new WaitForSeconds(tiempoEntreMovimientos);
            pared2.PonerRojo();
            pared4.PonerRojo();

            yield return new WaitForSeconds(tiempoRojo);
            pared2.Bajar();
            pared4.Bajar();
            pared3.Subir();
            pared1.Subir();

            yield return new WaitForSeconds(tiempoEntreMovimientos);
            pared2.PonerRojo();
            pared3.PonerRojo();
            yield return new WaitForSeconds(tiempoRojo);
            

            pared2.Bajar();
            pared3.Bajar();

            yield return new WaitForSeconds(tiempoEntreMovimientos);
            pared2.Subir();
            pared3.Subir();

            pared1.PonerRojo();
            pared4.PonerRojo();
            yield return new WaitForSeconds(tiempoRojo);

            pared1.Bajar();
            pared4.Bajar();

            yield return new WaitForSeconds(tiempoEntreMovimientos);

            pared1.Subir();
            pared4.Subir();
            yield return new WaitForSeconds(tiempoEntreMovimientos);
        }
    }
}
