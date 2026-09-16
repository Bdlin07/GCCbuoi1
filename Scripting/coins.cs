using UnityEngine;

public class coins : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        collect player = other.GetComponent<collect>();

        if (player != null)
        {
            Destroy(gameObject);
        }
    }
}
