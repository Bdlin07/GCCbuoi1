using UnityEngine;

public class NPCHealth : MonoBehaviour
{
    public int health = 3;

    public void TakeDamage(int damage)
    {
        health = health - damage;
        Debug.Log("Mau NPC: " + health);

        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
