using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum EnemyType
    {
        Dodge,
        Push,
        Mirror
    }

    public EnemyType enemyType;

    [SerializeField] private GameObject head, body, legs;

    Transform player, playerCam;
    NavMeshAgent navMeshAgent;
    [SerializeField] GameObject mirrorObject;

    // Mirror movement tuning
    [SerializeField] float mirrorWanderRadius = 2f;
    [SerializeField, Range(0f, 1f)] float mirrorApproachChance = 0.2f; // chance per check to approach player
    [SerializeField] float mirrorApproachRadius = 10f;               // only consider approaching if player within this distance
    [SerializeField] float mirrorApproachCooldown = 3f;              // minimum seconds between approach attempts
    float lastMirrorApproachTime;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCam = Camera.main.transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        lastMirrorApproachTime = -mirrorApproachCooldown;

        switch (enemyType)
        {
            case EnemyType.Dodge:
                navMeshAgent.speed = 100f;
                InvokeRepeating("DodgeEnemyLoop", 0f, 1f);
                break;
            case EnemyType.Push:
                InvokeRepeating("PushEnemyLoop", 0f, 0.5f);
                break;
            case EnemyType.Mirror:
                // Slower, calmer wandering for mirror enemies
                navMeshAgent.speed = 10f;               // slower movement
                navMeshAgent.angularSpeed = 120f;        // reasonable turning speed
                // Prevent the NavMeshAgent from rotating the transform so we can
                // control facing independently (always face the player).
                navMeshAgent.updateRotation = false;
                InvokeRepeating("MirrorEnemyLoop", 0f, 0.6f); // update less frequently
                break;
        }
    }

    private void Update()
    {
        // If this is a mirror enemy, always face the player (horizontal only),
        // but keep moving using the NavMeshAgent (agent rotation is disabled).
        if (enemyType == EnemyType.Mirror && player != null && navMeshAgent != null)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0f; // Keep only the horizontal direction
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                // Rotate towards the target at navMeshAgent.angularSpeed degrees per second.
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, navMeshAgent.angularSpeed * Time.deltaTime);
            }

            // Keep the mirror object oriented to mirror the player's camera continuously.
            if (mirrorObject != null && playerCam != null)
            {
                Vector3 desiredForward = -playerCam.forward;
                Quaternion mirrorRotation = Quaternion.LookRotation(desiredForward, Vector3.up);
                mirrorObject.transform.rotation = mirrorRotation;
            }
        }
    }

    private void PushEnemyLoop()
    {
        navMeshAgent.SetDestination(player.position);
    }

    private void DodgeEnemyLoop()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 5f;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 5f, NavMesh.AllAreas);
        navMeshAgent.SetDestination(hit.position);
    }

    private void MirrorEnemyLoop()
    {
        if (navMeshAgent == null) return;

        // Occasionally decide to approach the player (gentle, rate-limited and probabilistic)
        bool didApproach = false;
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);
            if (distanceToPlayer <= mirrorApproachRadius && Time.time - lastMirrorApproachTime >= mirrorApproachCooldown)
            {
                if (Random.value <= mirrorApproachChance)
                {
                    // Move toward the player (calmly)
                    navMeshAgent.SetDestination(player.position);
                    lastMirrorApproachTime = Time.time;
                    didApproach = true;
                }
            }
        }

        if (didApproach) return;

        // Gentle random wandering: sample a nearby navmesh point occasionally and move there.
        // Only pick a new target if there is no path or we're nearly at the current target.
        if (!navMeshAgent.hasPath || navMeshAgent.remainingDistance < 0.5f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * mirrorWanderRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            // sample with a small max distance so target stays close
            if (NavMesh.SamplePosition(randomDirection, out hit, mirrorWanderRadius, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
            }
        }
    }

}
