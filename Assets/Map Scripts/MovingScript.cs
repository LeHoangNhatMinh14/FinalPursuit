using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public enum MovementDirection { LeftRight, ForwardBack, UpDown }
    
    [Header("Movement Settings")]
    public MovementDirection direction = MovementDirection.LeftRight;
    public float speed = 3f;
    public float distance = 5f;
    
    private Vector3 startPos;
    private Vector3 movementAxis;

    void Start()
    {
        startPos = transform.position;
        SetMovementAxis();
    }

    void Update()
    {
        float offset = Mathf.PingPong(Time.time * speed, distance * 2) - distance;
        transform.position = startPos + movementAxis * offset;
    }

    void SetMovementAxis()
    {
        switch(direction)
        {
            case MovementDirection.LeftRight:
                movementAxis = Vector3.right; // X axis
                break;
            case MovementDirection.ForwardBack:
                movementAxis = Vector3.forward; // Z axis
                break;
            case MovementDirection.UpDown:
                movementAxis = Vector3.up; // Y axis
                break;
        }
    }
}