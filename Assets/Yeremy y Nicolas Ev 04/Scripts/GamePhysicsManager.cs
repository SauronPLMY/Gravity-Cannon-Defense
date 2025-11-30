using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePhysicsManager : MonoBehaviour
{
    private static GamePhysicsManager _instance;
    private List<GameRigidbody2D> _rigidbodies = new List<GameRigidbody2D>();
    private List<GameCollider2D> _colliders = new List<GameCollider2D>();
    
    public static GamePhysicsManager Instance => _instance;
    
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
    }
    
    void FixedUpdate()
    {
        DetectCollisions();
    }
    
    public void RegisterRigidbody(GameRigidbody2D rb)
    {
        if (!_rigidbodies.Contains(rb))
            _rigidbodies.Add(rb);
    }
    
    public void UnregisterRigidbody(GameRigidbody2D rb)
    {
        _rigidbodies.Remove(rb);
    }
    
    public void RegisterCollider(GameCollider2D col)
    {
        if (!_colliders.Contains(col))
            _colliders.Add(col);
    }
    
    public void UnregisterCollider(GameCollider2D col)
    {
        _colliders.Remove(col);
    }
    
    void DetectCollisions()
    {
        for (int i = 0; i < _colliders.Count; i++)
        {
            for (int j = i + 1; j < _colliders.Count; j++)
            {
                if (_colliders[i].CheckCollision(_colliders[j]))
                {
                    HandleCollision(_colliders[i], _colliders[j]);
                    
                    if (_colliders[i].isTrigger || _colliders[j].isTrigger)
                    {
                        HandleTrigger(_colliders[i], _colliders[j]);
                    }
                }
            }
        }
    }
    
    void HandleCollision(GameCollider2D colA, GameCollider2D colB)
    {
        GameRigidbody2D rbA = colA.GetComponent<GameRigidbody2D>();
        GameRigidbody2D rbB = colB.GetComponent<GameRigidbody2D>();
        
        if (rbA != null && rbB != null)
        {
            Vector2 collisionNormal = (colB.Center - colA.Center).normalized;
            
            if (colA.CompareTag("Projectile") || colB.CompareTag("Projectile"))
            {
                HandleProjectileCollision(colA, colB, collisionNormal);
            }
        }
    }
    
    void HandleTrigger(GameCollider2D colA, GameCollider2D colB)
    {
        GameCollider2D triggerCol = colA.isTrigger ? colA : colB;
        GameCollider2D otherCol = colA.isTrigger ? colB : colA;
        
        TriggerHandler handler = triggerCol.GetComponent<TriggerHandler>();
        if (handler != null)
        {
            handler.OnTriggerEnterEvent(otherCol.gameObject);
        }
    }
    
    void HandleProjectileCollision(GameCollider2D colA, GameCollider2D colB, Vector2 normal)
    {
        GameCollider2D projectileCol = colA.CompareTag("Projectile") ? colA : colB;
        GameCollider2D otherCol = colA.CompareTag("Projectile") ? colB : colA;
        
        if (otherCol.CompareTag("Planet"))
        {
            Destroy(projectileCol.gameObject);
        }
    }
}