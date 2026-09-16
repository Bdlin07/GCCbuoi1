using UnityEngine;
using UnityEngine.InputSystem;

public class Bai1MoveAB : MonoBehaviour
{
    public GameObject objectToMove;
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    bool moveToB = false;
    InputAction interactAction;

    void Awake()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    void Start()
    {
        objectToMove.transform.position = pointA.position;
    }

    void Update()
    {
        if(moveToB == true)
        {
            objectToMove.transform.position = Vector3.MoveTowards(
                objectToMove.transform.position,
                pointB.position,
                moveSpeed * Time.deltaTime
            );
        }
        else
        {
            objectToMove.transform.position = Vector3.MoveTowards(
                objectToMove.transform.position,
                pointA.position,
                moveSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player") && interactAction.WasPressedThisFrame())
        {
            moveToB = !moveToB;
        }
    }
}
