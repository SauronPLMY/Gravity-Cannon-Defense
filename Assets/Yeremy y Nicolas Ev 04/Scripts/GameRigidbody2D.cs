using UnityEngine;
using PUCV.PhysicEngine2D;

public class GameRigidbody2D : MonoBehaviour, IHasCollider
{
    public Vector2 velocity;
    public float mass = 1.0f;
    public bool useGravity = true;
    public float gravityScale = 1.0f;
    public bool useBuoyancy = false;
    public float buoyancyDensity = 0.5f;
    
    private Vector2 _totalForce;
    private PUCV.PhysicEngine2D.CustomCollider2D _customCollider;
    private bool _isColliding;

    void Start()
    {
        _customCollider = GetComponent<PUCV.PhysicEngine2D.CustomCollider2D>();
    }

    public Vector2 GetWorldPosition()
    {
        return transform.position;
    }
    
    public void SetWorldPosition(Vector2 newPos)
    {
        transform.position = newPos;
    }

    public void AddForce(Vector2 force)
    {
        _totalForce += force;
    }

    public void AddGravityForce(Vector2 gravity)
    {
        if (useGravity)
        {
            _totalForce += gravity * mass * gravityScale;
        }
    }

    public void AddBuoyancyForce(Vector2 buoyancyForce)
    {
        if (useBuoyancy)
        {
            _totalForce += buoyancyForce * buoyancyDensity;
        }
    }

    public void ClearForces()
    {
        _totalForce = Vector2.zero;
    }

    public Vector2 GetTotalForce()
    {
        return _totalForce;
    }

    public void UpdatePhysics(float deltaTime)
    {
        if (_isColliding)
        {
            _isColliding = false;
            ClearForces();
            return;
        }

        if (mass > 0)
        {
            velocity += _totalForce / mass * deltaTime;
        }
        
        Vector2 newPosition = GetWorldPosition() + velocity * deltaTime;
        SetWorldPosition(newPosition);
        
        ClearForces();
    }

    public void OnInformCollisionEnter2D(CollisionInfo collisionInfo)
    {
        _isColliding = true;
        
        Vector2 normal = collisionInfo.contactNormal;
        velocity = Vector2.Reflect(velocity, normal.normalized) * 0.8f;
        
        Debug.Log($"Colisión detectada! Normal: {normal}, Velocidad resultante: {velocity}");
    }
}