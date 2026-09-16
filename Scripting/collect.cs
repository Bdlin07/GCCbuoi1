using UnityEngine;

public class collect : MonoBehaviour
{
    public int coinCount = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        coins coin = other.GetComponent<coins>();

        if (coin != null)
        {
            coinCount = coinCount + 1;
            Debug.Log("So xu dang co: " + coinCount);
        }
    }
}
