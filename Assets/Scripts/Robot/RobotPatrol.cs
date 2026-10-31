using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RobotPatrol : MonoBehaviour
{
    [Header("Configuracion patrulla")]
    [SerializeField] private float radioPatrulla = 15f;
    [SerializeField] private float tiempoEspera = 2f;
    [SerializeField] private float distanciaLlegada = 1f;

    [Header("Busqueda de destinos")]
    [SerializeField] private int intentosMaximos = 30;
    [SerializeField] private float distanciaMuestreo = 2f;

    private NavMeshAgent agente;
    private float temporizador;
    private bool esperando;

    private void Start()
    {
        agente = GetComponent<NavMeshAgent>();

        agente.stoppingDistance = distanciaLlegada;

        ElegirDestino();
    }

    private void Update()
    {
        if (agente == null || !agente.isOnNavMesh)
            return;

        if (esperando)
        {
            temporizador -= Time.deltaTime;

            if (temporizador <= 0f)
            {
                esperando = false;
                ElegirDestino();
            }

            return;
        }

       
        if (agente.pathPending)
            return;

        
        if (agente.pathStatus == NavMeshPathStatus.PathInvalid ||
            agente.pathStatus == NavMeshPathStatus.PathPartial)
        {
            ElegirDestino();
            return;
        }

       
        if (agente.hasPath &&
            agente.remainingDistance <= distanciaLlegada + 0.2f)
        {
            agente.ResetPath();
            esperando = true;
            temporizador = tiempoEspera;
        }
    }

    private void ElegirDestino()
    {
        if (agente == null || !agente.isOnNavMesh)
            return;

        for (int i = 0; i < intentosMaximos; i++)
        {
            Vector3 puntoAleatorio =
                transform.position +
                Random.insideUnitSphere * radioPatrulla;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(
                puntoAleatorio,
                out hit,
                distanciaMuestreo,
                agente.areaMask))
            {
                NavMeshPath camino = new NavMeshPath();

                if (agente.CalculatePath(hit.position, camino) &&
                    camino.status == NavMeshPathStatus.PathComplete)
                {
                    if (agente.SetDestination(hit.position))
                    {
                        esperando = false;
                        return;
                    }
                }
            }
        }

        
        esperando = true;
        temporizador = 1f;

        Debug.LogWarning(
            "RobotPatrulla: no se encontro un destino valido.",
            gameObject
        );
    }
}
