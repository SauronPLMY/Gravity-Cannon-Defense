using UnityEngine;

public class BuoyancyController : MonoBehaviour
{
    public float buoyancyForce = 25f;
    public float waterDrag = 0.8f;
    
    private GameCollider2D _waterCollider;

    void Start()
    {
        _waterCollider = GetComponent<GameCollider2D>();
    }

    void Update()
    {
        CheckBuoyancyForAllProjectiles();
    }

    void CheckBuoyancyForAllProjectiles()
    {
        GameRigidbody2D[] allRigidbodies = FindObjectsByType<GameRigidbody2D>(FindObjectsSortMode.None);
        
        foreach (GameRigidbody2D rb in allRigidbodies)
        {
            if (rb.CompareTag("Projectile") && IsInWater(rb))
            {
                Debug.Log($"PROYECTIL EN AGUA: {rb.name}");
                ApplyBuoyancy(rb);
            }
        }
    }

    bool IsInWater(GameRigidbody2D rb)
    {
        if (_waterCollider == null) return false;
        
        GameCollider2D projectileCollider = rb.GetComponent<GameCollider2D>();
        if (projectileCollider == null) return false;
        
        bool collision = _waterCollider.CheckCollision(projectileCollider);
        
        // Debug visual
        if (collision)
        {
            Debug.DrawLine(_waterCollider.Center, projectileCollider.Center, Color.green, 0.1f);
        }
        
        return collision;
    }

    void ApplyBuoyancy(GameRigidbody2D rb)
    {
        rb.AddForce(Vector2.up * buoyancyForce);
        rb.velocity *= waterDrag;
    }
}