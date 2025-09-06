using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DemonAgent : Agent
{
    public Transform player;
    public Transform goal;
    public LayerMask obstacleMask;
    public float maxRayDistance = 8f;
    public int rayCount = 5;

    private NavMeshAgent navAgent;
    private Rigidbody rb;
    private Vector3 startPos;
    private Quaternion startRot;
    private float maxSpeed;

    public int damage = 10; // amount to hurt the player

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        if (navAgent != null) maxSpeed = navAgent.speed;
        startPos = transform.position;
        startRot = transform.rotation;
    }

    public override void OnEpisodeBegin()
    {
        if (navAgent != null && navAgent.isOnNavMesh)
        {
            // Reset position
            Vector3 randomPos = startPos + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                navAgent.Warp(hit.position);
                navAgent.ResetPath();  // Safe because agent is on NavMesh
                transform.rotation = startRot;
            }
            else
            {
                navAgent.Warp(startPos);
                navAgent.ResetPath();
                transform.rotation = startRot;
            }
        }
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 toPlayer = player.position - transform.position;
        sensor.AddObservation(toPlayer / 20f);

        if (goal != null) sensor.AddObservation((goal.position - transform.position) / 20f);
        else sensor.AddObservation(Vector3.zero);

        sensor.AddObservation(rb != null ? rb.velocity.magnitude / maxSpeed : 0f);

        float half = (rayCount - 1) * 0.5f;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = (i - half) * (45f / half);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, out RaycastHit hit, maxRayDistance, obstacleMask))
            {
                sensor.AddObservation(hit.distance / maxRayDistance);
            }
            else sensor.AddObservation(1f);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float turn = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        Vector3 forward = transform.forward * move * navAgent.speed * Time.deltaTime;
        navAgent.Move(forward);
        transform.Rotate(Vector3.up, turn * 120f * Time.deltaTime);

        AddReward(-0.0005f);

        float distToPlayer = Vector3.Distance(transform.position, player.position);
        AddReward(-distToPlayer * 0.0001f);

        // Reward for reaching player
        if (distToPlayer < 1.2f)
        {
            AddReward(1.0f);
            DealDamageToPlayer();
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var contOut = actionsOut.ContinuousActions;
        contOut[0] = Input.GetKey(KeyCode.W) ? 1f : (Input.GetKey(KeyCode.S) ? -1f : 0f);
        contOut[1] = Input.GetKey(KeyCode.A) ? -1f : (Input.GetKey(KeyCode.D) ? 1f : 0f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DealDamageToPlayer(other.gameObject);
        }
        else if (other.CompareTag("Hazard"))
        {
            AddReward(-0.5f);
            EndEpisode();
        }
    }

    private void DealDamageToPlayer(GameObject targetPlayer = null)
    {
        if (targetPlayer == null) targetPlayer = player.gameObject;
        PlayerHealth ph = targetPlayer.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(damage);
            AddReward(0.5f); // reward for successfully attacking
        }
    }
}
