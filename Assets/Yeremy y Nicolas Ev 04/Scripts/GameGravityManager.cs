using UnityEngine;
using System.Collections.Generic;

public class GameGravityManager : MonoBehaviour
{
    public float globalGravity = 9.8f;
    public List<GameObject> planets = new List<GameObject>();
    
    private List<GameRigidbody2D> _planetRigidbodies = new List<GameRigidbody2D>();
    private List<CustomCollider2D> _planetColliders = new List<CustomCollider2D>();

    void Start()
    {
        FindAllPlanets();
    }

    void FindAllPlanets()
    {
        _planetRigidbodies.Clear();
        _planetColliders.Clear();
        
        foreach (GameObject planet in planets)
        {
            if (planet != null)
            {
                GameRigidbody2D rb = planet.GetComponent<GameRigidbody2D>();
                CustomCollider2D col = planet.GetComponent<CustomCollider2D>();
                
                if (rb != null && col != null)
                {
                    _planetRigidbodies.Add(rb);
                    _planetColliders.Add(col);
                }
            }
        }
    }

    void FixedUpdate()
    {
        ApplyPlanetaryGravity();
    }

    void ApplyPlanetaryGravity()
    {
        GameRigidbody2D[] allRigidbodies = FindObjectsByType<GameRigidbody2D>(FindObjectsSortMode.None);
        
        foreach (GameRigidbody2D rb in allRigidbodies)
        {
            if (!rb.useGravity) continue;
            
            Vector2 totalGravity = Vector2.zero;
            
            for (int i = 0; i < _planetRigidbodies.Count; i++)
            {
                Vector2 planetPos = _planetColliders[i].transform.position;
                Vector2 objectPos = rb.GetWorldPosition();
                Vector2 direction = planetPos - objectPos;
                float distance = direction.magnitude;
                
                if (distance < 0.1f) continue;
                
                float gravityStrength = globalGravity * _planetRigidbodies[i].mass / (distance * distance);
                totalGravity += direction.normalized * gravityStrength;
            }
            
            rb.AddGravityForce(totalGravity);
        }
    }

    public void AddPlanet(GameObject planet)
    {
        if (!planets.Contains(planet))
        {
            planets.Add(planet);
            FindAllPlanets();
        }
    }

    public void RemovePlanet(GameObject planet)
    {
        if (planets.Contains(planet))
        {
            planets.Remove(planet);
            FindAllPlanets();
        }
    }
}