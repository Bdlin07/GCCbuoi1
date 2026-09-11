using UnityEngine;
using UnityEngine.InputSystem;
public class NewMonoBehaviourScript : MonoBehaviour
{
    InputAction moveAction;
    InputAction jumpAction;
    void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(moveAction.ReadValue<float>());
        if(jumpAction.WasPressedThisFrame())
        {
            Debug.Log("Jump Pressed");
        }
        if(jumpAction.IsPressed())
        {
            Debug.Log("Jump Hold");
        }
        if(jumpAction.WasReleasedThisFrame())
        {
            Debug.Log("Jump Released");
        }
    }
}
