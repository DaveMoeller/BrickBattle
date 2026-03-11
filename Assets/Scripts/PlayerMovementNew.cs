using UnityEngine;
using UnityEngine.InputSystem; //

public class PlayerMovementNew : MonoBehaviour
{
    public InputAction moveAction; // Reference the "Move" action asset in the Inspector
    public float movementSpeed = 5.0f;

    void OnEnable()
    {
        moveAction.Enable(); // Enable the action when the object is enabled
    }

    void OnDisable()
    {
        moveAction.Disable(); // Disable the action when the object is disabled
    }

    void Update()
    {
        // Read the 2D vector value from the action
        Vector2 moveVector = moveAction.ReadValue<Vector2>();

        // Use the x (horizontal) and y (vertical) components for movement
        Vector3 movement = new Vector3(moveVector.x, 0.0f, moveVector.y);
        transform.Translate(movement * movementSpeed * Time.deltaTime);
    }
}
