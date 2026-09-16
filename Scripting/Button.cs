using UnityEngine;
using UnityEngine.InputSystem;

public class Button : MonoBehaviour
{
    public GameObject door;
    public float doorSpeed = 2f;
    public float openHeight = 3f;

    InputAction interactAction;
    bool isPlayerInRange = false;
    bool doorIsOpen = false;
    Vector3 closedPosition;
    Vector3 openPosition;

    void Awake()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    void Start()
    {
        closedPosition = door.transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    void Update()
    {
        if(isPlayerInRange && interactAction.WasPressedThisFrame())
        {
            doorIsOpen = !doorIsOpen;
        }

        if(doorIsOpen == true)
        {
            door.transform.position = Vector3.MoveTowards(
                door.transform.position,
                openPosition,
                doorSpeed * Time.deltaTime
            );
        }
        else
        {
            door.transform.position = Vector3.MoveTowards(
                door.transform.position,
                closedPosition,
                doorSpeed * Time.deltaTime
            );
        }
    }
}
