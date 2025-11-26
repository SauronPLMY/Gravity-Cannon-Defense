using UnityEngine;

public class GameCollider2D : MonoBehaviour
{
    public enum ShapeType { Circle, Rectangle }
    public ShapeType shapeType = ShapeType.Circle;
    
    public float radius = 0.5f;
    public Vector2 size = Vector2.one;
    public bool isTrigger = false;
    
    public Vector2 Center => (Vector2)transform.position;

    void Start()
    {
        RegisterCollider();
    }

    void OnEnable()
    {
        RegisterCollider();
    }

    void OnDisable()
    {
        UnregisterCollider();
    }

    void OnDestroy()
    {
        UnregisterCollider();
    }

    void RegisterCollider()
    {
        if (GamePhysicsManager.Instance != null)
        {
            GamePhysicsManager.Instance.RegisterCollider(this);
        }
    }

    void UnregisterCollider()
    {
        if (GamePhysicsManager.Instance != null)
        {
            GamePhysicsManager.Instance.UnregisterCollider(this);
        }
    }

    public bool CheckCollision(GameCollider2D other)
    {
        if (shapeType == ShapeType.Circle && other.shapeType == ShapeType.Circle)
        {
            return CircleVsCircle(this, other);
        }
        return false;
    }
    
    bool CircleVsCircle(GameCollider2D a, GameCollider2D b)
    {
        float distance = Vector2.Distance(a.Center, b.Center);
        float minDistance = a.radius + b.radius;
        return distance <= minDistance;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = isTrigger ? Color.yellow : Color.cyan;
        if (shapeType == ShapeType.Circle)
        {
            Gizmos.DrawWireSphere(Center, radius);
        }
        else
        {
            Gizmos.DrawWireCube(Center, size);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Center, 0.1f);
    }
}