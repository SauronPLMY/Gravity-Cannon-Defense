using UnityEngine;

public class GameRigidbody2D : MonoBehaviour
{
    [Header("Physics Properties")]
    public float mass = 1.0f;
    public float drag = 0.1f;
    public bool useGravity = true;
    
    [Header("Current State")]
    public Vector2 velocity;
    public Vector2 position;
    
    private Vector2 _totalForces;
    
    void Start()
    {
        position = transform.position;
        RegisterRigidbody();
    }

    void OnEnable()
    {
        RegisterRigidbody();
    }

    void OnDisable()
    {
        UnregisterRigidbody();
    }

    void OnDestroy()
    {
        UnregisterRigidbody();
    }

    void RegisterRigidbody()
    {
        if (GamePhysicsManager.Instance != null)
        {
            GamePhysicsManager.Instance.RegisterRigidbody(this);
        }
    }

    void UnregisterRigidbody()
    {
        if (GamePhysicsManager.Instance != null)
        {
            GamePhysicsManager.Instance.UnregisterRigidbody(this);
        }
    }
    
    void FixedUpdate()
    {
        ApplyForces();
        ApplyMovement();
        UpdateTransform();
    }
    
    public void AddForce(Vector2 force)
    {
        _totalForces += force;
    }
    
    void ApplyForces()
    {
        if (mass > 0)
        {
            Vector2 acceleration = _totalForces / mass;
            velocity += acceleration * Time.fixedDeltaTime;
        }
        
        velocity *= (1f - drag * Time.fixedDeltaTime);
        
        _totalForces = Vector2.zero;
    }
    
    void ApplyMovement()
    {
        position += velocity * Time.fixedDeltaTime;
    }
    
    void UpdateTransform()
    {
        transform.position = position;
    }
    
    public void SetPosition(Vector2 newPosition)
    {
        position = newPosition;
        transform.position = position;
    }
}