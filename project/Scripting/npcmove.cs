using System.Collections;
using UnityEngine;
using TMPro;

public class npcmove : MonoBehaviour
{
    [SerializeField] private TMP_Text npcText;
    [SerializeField] private float DelayTime;
    public float moveSpeed = 2f;
    public float stopDistance = 1f;
    Transform player;
    Coroutine timerCoroutine;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if(playerObject != null)
        {
            player = playerObject.transform;
        }

        npcText.text = "Phúc Gay";
        npcText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(player != null)
        {
            float distance = Mathf.Abs(player.position.x - transform.position.x);

            if(distance > stopDistance)
            {
                Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            npcText.gameObject.SetActive(true);
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }
            timerCoroutine = StartCoroutine(HideTextAfterDelay());
        }
    }

    IEnumerator HideTextAfterDelay()
    {
        yield return new WaitForSeconds(DelayTime);
        npcText.gameObject.SetActive(false);
    }
}
