using UnityEngine;
using PUCV.PhysicEngine2D;

public class ProjectileCollision : MonoBehaviour, IHasCollider
{
    private GameRigidbody2D _gameRb;
    
    void Start()
    {
        _gameRb = GetComponent<GameRigidbody2D>();
    }
    public void OnInformCollisionEnter2D(CollisionInfo collisionInfo)
    {
        Debug.Log("¡PROYECTIL COLISIONÓ!");

        if (collisionInfo.otherCollider.GetComponent<Planet>() != null)
        {
            Debug.Log("¡Golpeó un planeta!");
            Destroy(gameObject, 0.1f);
        }
    }
}