using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int attackDamage = 10; // damage per touch attack

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Demon"))
        {
            Debug.Log("Player attacked demon!");

            DemonAI demon = other.GetComponent<DemonAI>();
            if (demon != null)
            {
                demon.TakeDamage(attackDamage); // Apply damage instead of destroying
            }
        }
    }
}
