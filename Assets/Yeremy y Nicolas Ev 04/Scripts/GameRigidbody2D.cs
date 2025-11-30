using UnityEngine;

public class GameRigidbody2D : MonoBehaviour
{
    public Vector2 velocity;
    public bool useGravity = true;
    public float mass = 1f;

    public Vector2 position
    {
        get => transform.position;
        set => transform.position = value;
    }

    private void Awake()
    {
        position = transform.position;
    }

    public void SetPosition(Vector2 newPos)
    {
        position = newPos;
    }

    public void ApplyPosition(Vector2 newPos)
    {
        position = newPos;
    }

    public void AddForce(Vector2 force)
    {
        velocity += force / mass * Time.fixedDeltaTime;
    }

    private void FixedUpdate()
    {
        position += velocity * Time.fixedDeltaTime;
    }
}
