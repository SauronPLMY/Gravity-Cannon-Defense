using UnityEngine;

public class BuoyancyZone : MonoBehaviour
{
    public float fluidDensity = 1.0f;
    public Vector2 buoyancyForce = new Vector2(0f, 9.8f);
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        ApplyBuoyancy(other.gameObject);
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        ApplyBuoyancy(other.gameObject);
    }
    
    private void ApplyBuoyancy(GameObject obj)
    {
        GameRigidbody2D rb = obj.GetComponent<GameRigidbody2D>();
        if (rb != null && rb.useBuoyancy)
        {
            rb.AddBuoyancyForce(buoyancyForce);
        }
    }
}