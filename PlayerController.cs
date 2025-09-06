using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform attackOrigin;  // Empty GameObject in front of player
    public float attackRange = 2f;
    public int attackDamage = 10;
    public LayerMask enemyLayer;    // Assign to "Enemy" layer in Inspector
    public float attackCooldown = 0.5f;

    private Rigidbody rb;
    private float lastAttackTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleMovement();

        if (Input.GetKeyDown(KeyCode.F))   // Attack once per press
        {
            Attack();
        }
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v).normalized * moveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + move);
    }

    void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;

        RaycastHit hit;
        if (Physics.Raycast(attackOrigin.position, attackOrigin.forward, out hit, attackRange, enemyLayer))
        {
            DemonAI demon = hit.collider.GetComponent<DemonAI>();
            if (demon != null)
            {
                demon.TakeDamage(attackDamage); // reduce health by attackDamage
            }
        }
        else
        {
            Debug.Log("Attack missed!");
        }
    }
}
