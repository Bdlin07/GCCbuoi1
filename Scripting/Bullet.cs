using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public int damage = 1;
    Vector2 moveDirection;

    void Awake()
    {
        Destroy(gameObject, 3f);
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction;
    }

    void Update()
    {
        transform.position = transform.position +
                             (Vector3)(moveDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        NPCHealth npc = other.GetComponent<NPCHealth>();

        if(npc != null)
        {
            npc.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
