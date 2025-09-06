using UnityEngine;
using UnityEngine.AI;

public class DemonAI : MonoBehaviour
{
    public Transform player;
    public float chaseRange = 10f;
    public float attackRange = 2f;
    public float moveSpeed = 3f;
    public int health = 40;          // demon health
    public int attackDamage = 10;    // damage dealt to player
    public float attackCooldown = 1.5f;

    private NavMeshAgent agent;
    private float lastAttackTime = 0f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }
        agent.speed = moveSpeed;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= chaseRange)
        {
            agent.SetDestination(player.position);

            if (distance <= attackRange)
            {
                AttackPlayer();
            }
        }
    }

    private void AttackPlayer()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;

        Debug.Log("Demon attacked player!");

        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Demon health: " + health);

        if (health <= 0)
        {
            Debug.Log("Demon defeated!");
            Destroy(gameObject);
        }
    }
}
