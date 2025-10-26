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

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCam = Camera.main.transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        switch (enemyType)
        {
            case EnemyType.Dodge:
                navMeshAgent.speed = 100f;
                navMeshAgent.acceleration = 100f;
                navMeshAgent.angularSpeed = 500f;
                InvokeRepeating("DodgeEnemyLoop", 0f, 0.5f);
                break;
            case EnemyType.Push:
                InvokeRepeating("PushEnemyLoop", 0f, 0.5f);
                break;
            case EnemyType.Mirror:
                InvokeRepeating("MirrorEnemyLoop", 0f, 0.1f);
                break;
        }
    }

    private void Update()
    {
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
        // Make the enemy face the player (horizontal only)
        if (player != null)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0f; // Keep only the horizontal direction
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = lookRotation;
            }
        }

        // Make the mirror face the opposite direction the player is looking at,
        // using the camera's forward vector. This avoids Euler wrapping / 0..360 ambiguity
        // and correctly mirrors pitch (up/down) and yaw (left/right).
        if (mirrorObject != null && playerCam != null)
        {
            // Desired forward for the mirror is the opposite of the camera's forward.
            Vector3 desiredForward = -playerCam.forward;

            // If you want the mirror to remain upright in world space, provide Vector3.up as the up vector.
            Quaternion mirrorRotation = Quaternion.LookRotation(desiredForward, Vector3.up);

            // If the mirror model is oriented such that it's facing the wrong way by default,
            // you can apply a Y offset, e.g. Quaternion.Euler(0, 180f, 0), like:
            // mirrorObject.transform.rotation = mirrorRotation * Quaternion.Euler(0, 180f, 0);
            mirrorObject.transform.rotation = mirrorRotation;
        }
    }

}
