using UnityEngine;

public class CannonController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float shootForce = 20f;
    public float rotationSpeed = 90f;
    public KeyCode shootKey = KeyCode.Space;
    public float projectileSpawnDistance = 2f;

    private float _currentAngle = 0f;

    void Update()
    {
        HandleRotation();
        HandleShooting();
    }

    void HandleRotation()
    {
        float rotationInput = 0f;

        if (Input.GetKey(KeyCode.UpArrow))
            rotationInput = 1f;
        else if (Input.GetKey(KeyCode.DownArrow))
            rotationInput = -1f;

        _currentAngle += rotationInput * rotationSpeed * Time.deltaTime;
        _currentAngle = Mathf.Clamp(_currentAngle, -80f, 80f);
        
        transform.rotation = Quaternion.Euler(0f, 0f, _currentAngle);
    }

    void HandleShooting()
    {
        if (Input.GetKeyDown(shootKey))
        {
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        if (projectilePrefab == null) return;

        Vector2 shootDirection = transform.right;
        Vector2 spawnPosition = (Vector2)transform.position + shootDirection * projectileSpawnDistance;

        GameObject newProjectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        GameRigidbody2D projectileRb = newProjectile.GetComponent<GameRigidbody2D>();
        
        if (projectileRb != null)
        {
            projectileRb.velocity = shootDirection * shootForce;
        }
    }
}