using UnityEngine;

// Principio de responsabilidad unica (LightExposureDetector) se encarga unicamente de detectar si la Sombra
// esta iluminada, esta clase se encarga únicamente de reaccionar a ese cambio (muerte, reset, sonido, etc)
// ninguna de las dos clases sabe nada de la otra más alla del evento, este script no hace ningún Raycast ni conoce como se detecta la luz
public class ShadowLightReactor : MonoBehaviour
{
    [Header("Luces a observar")]
    // Arrastra aca todos los GameObjects que tengan un LightExposureDetector, Un mismo Reactor puede escuchar
    // varias luces a la vez
    [SerializeField] private LightExposureDetector[] lightsToWatch;

    // Se suscribe al activarse el objeto
    private void OnEnable()
    {
        SetSubscription(subscribe: true);
    }

    // ...y se desuscribe al desactivarse o destruirse, sin esto, si el objeto se desactiva, quedaría "escuchando" para siempre y el
    // evento puede intentar llamar a un metodo de un objeto que ya no deberia reaccionar
    private void OnDisable()
    {
        SetSubscription(subscribe: false);
    }

    private void SetSubscription(bool subscribe)
    {
        // Si nunca se asigno el array en el Inspector, lightsToWatch es null y el foreach tiraria NullReferenceException
        if (lightsToWatch == null)
        {
            return;
        }

        foreach (LightExposureDetector detector in lightsToWatch)
        {
            // Por si algun slot del array quedo vacio en el Inspector
            if (detector == null)
            {
                continue;
            }

            if (subscribe)
            {
                // Suscribirse: evento += manejadorDelEvento;
                detector.OnExposureChanged += HandleExposureChanged;
            }
            else
            {
                // Desuscribirse: evento -= manejadorDelEvento;
                detector.OnExposureChanged -= HandleExposureChanged;
            }
        }
    }

    // Este metodo lo invoca el evento de CADA LightExposureDetector al que estamos suscriptos, cada vez que esa luz cambia su estado
    // "exposed" es el nuevo estado que llego desde OnExposureChanged
    private void HandleExposureChanged(bool exposed)
    {
        if (exposed)
        {
            //aca va la logica de muerte / daño / game over
            Debug.Log("Reactor: la Sombra quedó expuesta a la luz.", this);
        }
        else
        {
            //aca va la logica de reset / estado seguro
            Debug.Log("Reactor: la Sombra volvió a estar protegida.", this);
        }
    }
}