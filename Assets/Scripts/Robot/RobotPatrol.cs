using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class RobotPatrol : MonoBehaviour
{
    private enum State
    {
        Patrol,
        Alerta,
        Chase
    }

    [Header("Movement")]
    public NavMeshAgent agent;
    public float patrolRadius = 15f;
    public float patrolWaitTime = 1f;

    [Header("Vision")]
    public Transform visionPoint;
    public float visionDistance = 12f;
    [Range(0f, 360f)]
    public float visionAngle = 90f;
    public LayerMask obstacleMask;
    public float detectionInterval = 0.15f;

    [Header("Eyes")]
    public Renderer[] eyeRenderers;
    public Light[] eyeLights;
    public Color alertColor = Color.red;

    [Header("Alerta")]
    
    public AudioSource audioSource;
    public AudioClip detectedClip;
    public float alertDuration = 1f;

    private State currentState = State.Patrol;
    private Transform target;
    private float alertTimer;
    private float patrolTimer;
    private float detectionTimer;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        

        if (agent == null || !agent.isOnNavMesh)
        {
            Debug.LogError(
                "RobotEnemyAI: falta el NavMeshAgent " +
                "o el robot no está sobre el NavMesh.", this);
            enabled = false;
            return;
        }

        ChoosePatrolDestination();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Alerta:
                Alert();
                break;

            case State.Chase:
                Chase();
                break;
        }
    }

    private void Patrol()
    {
        detectionTimer -= Time.deltaTime;

        if (detectionTimer <= 0f)
        {
            detectionTimer = detectionInterval;

            Transform player = FindVisiblePlayer();

            if (player != null)
            {
                BeginAlert(player);
                return;
            }
        }

        patrolTimer -= Time.deltaTime;

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            if (patrolTimer <= 0f)
            {
                patrolTimer = patrolWaitTime;
                ChoosePatrolDestination();
            }
        }
    }

    private Transform FindVisiblePlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] shadowPlayers = GameObject.FindGameObjectsWithTag("ShadowPlayer");

        Transform bestPlayer = null;
        float nearestDistance = float.MaxValue;

        Vector3 origin = visionPoint != null
            ? visionPoint.position
            : transform.position + Vector3.up * 1.5f;

        Vector3 forward = visionPoint != null
            ? visionPoint.forward
            : transform.forward;

        foreach (GameObject player in players)
        {
            CheckPlayer(player.transform);
        }

        foreach (GameObject player in shadowPlayers)
        {
            CheckPlayer(player.transform);
        }

        return bestPlayer;

        void CheckPlayer(Transform player)
        {
            Vector3 destination = player.position + Vector3.up;
            Vector3 direction = destination - origin;
            float distance = direction.magnitude;

            if (distance > visionDistance || distance < 0.01f)
                return;

            float angle = Vector3.Angle(forward, direction);

            if (angle > visionAngle * 0.5f)
                return;

            if (Physics.Raycast(
                origin,
                direction.normalized,
                distance,
                obstacleMask,
                QueryTriggerInteraction.Ignore))
            {
                return;
            }

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                bestPlayer = player;
            }
        }
    }

    private void BeginAlert(Transform player)
    {
        target = player;
        currentState = State.Alerta;
        alertTimer = alertDuration;

        agent.isStopped = true;
        agent.ResetPath();

        SetEyesRed();

       

        if (audioSource != null && detectedClip != null)
        {
            audioSource.PlayOneShot(detectedClip);
        }
    }

    private void Alert()
    {
        alertTimer -= Time.deltaTime;

        if (alertTimer <= 0f)
        {
          

            currentState = State.Chase;
            agent.isStopped = false;
            agent.speed = agent.speed; 

            
            Chase();
        }
    }

    private void Chase()
    {
        
        if (target != null && agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);
        }
    }

    private void ChoosePatrolDestination()
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 randomPoint = transform.position +
                Random.insideUnitSphere * patrolRadius;

            if (NavMesh.SamplePosition(
                randomPoint, out NavMeshHit hit,
                3f, NavMesh.AllAreas))
            {
                agent.isStopped = false;
                agent.SetDestination(hit.position);
                return;
            }
        }
    }

    private void SetEyesRed()
    {
        foreach (Renderer eye in eyeRenderers)
        {
            if (eye == null)
                continue;

            foreach (Material material in eye.materials)
            {
                if (material.HasProperty("_BaseColor"))
                    material.SetColor("_BaseColor", alertColor);

                if (material.HasProperty("_Color"))
                    material.SetColor("_Color", alertColor);

                if (material.HasProperty("_EmissionColor"))
                {
                    material.EnableKeyword("_EMISSION");
                    material.SetColor("_EmissionColor", alertColor * 3f);
                }
            }
        }

        foreach (Light eyeLight in eyeLights)
        {
            if (eyeLight == null)
                continue;

            eyeLight.color = alertColor;
            eyeLight.enabled = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Transform point = visionPoint != null
            ? visionPoint
            : transform;

        Vector3 origin = visionPoint != null
            ? visionPoint.position
            : transform.position + Vector3.up * 1.5f;

        Vector3 forward = point.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, visionDistance);

        Vector3 left = Quaternion.Euler(
            0f, -visionAngle * 0.5f, 0f) * forward;

        Vector3 right = Quaternion.Euler(
            0f, visionAngle * 0.5f, 0f) * forward;

        Gizmos.DrawRay(origin, left * visionDistance);
        Gizmos.DrawRay(origin, right * visionDistance);
    }
}
