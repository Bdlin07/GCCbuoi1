using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    InputAction MoveAction;
    InputAction JumpAction;
    InputAction MoveTopdownAction;

    void Awake()
    {
        MoveAction = InputSystem.actions.FindAction("Move");
        JumpAction = InputSystem.actions.FindAction("Jump");
        MoveTopdownAction = InputSystem.actions.FindAction("MoveTopdown");
    }
    void Update()
    {
        if (MoveAction.IsPressed())
        {
            Debug.Log(MoveAction.ReadValue<float>());
        }

        if (JumpAction.WasPressedThisFrame())
        {
            Debug.Log("Jump Pressed");
        }
        if (JumpAction.IsPressed())
        {
            Debug.Log("Jump Hold");
        }
        if (JumpAction.WasReleasedThisFrame())
        {
            Debug.Log("Jump Release");
        }
        if (MoveTopdownAction.IsPressed())
        {
            Debug.Log(MoveTopdownAction.ReadValue<Vector2>());
        }
    }
}