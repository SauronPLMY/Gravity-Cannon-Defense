using UnityEngine;
using System.Collections.Generic;

public class GameGravityManager : MonoBehaviour
{
    public static GameGravityManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    [Header("Gravity Settings")]
    public float globalGravityForce = 9.8f;
    public List<GameObject> gravitySources = new List<GameObject>();

    private List<GameRigidbody2D> _sourceRigidbodies = new List<GameRigidbody2D>();
    private List<GameCollider2D> _sourceColliders = new List<GameCollider2D>();

    void Start()
    {
        FindAllGravitySources();
    }

    void FixedUpdate()
    {
        ApplyPlanetaryGravity();
    }

    // --------------------------------------------------------------------

    void FindAllGravitySources()
    {
        _sourceRigidbodies.Clear();
        _sourceColliders.Clear();

        foreach (GameObject source in gravitySources)
        {
            if (source != null)
            {
                GameRigidbody2D rb = source.GetComponent<GameRigidbody2D>();
                GameCollider2D col = source.GetComponent<GameCollider2D>();

                if (rb != null && col != null)
                {
                    _sourceRigidbodies.Add(rb);
                    _sourceColliders.Add(col);
                }
            }
        }
    }

    // --------------------------------------------------------------------

    void ApplyPlanetaryGravity()
    {
        GameRigidbody2D[] allRigidbodies = 
            FindObjectsByType<GameRigidbody2D>(FindObjectsSortMode.None);

        foreach (GameRigidbody2D rb in allRigidbodies)
        {
            if (!rb.useGravity) continue;

            Vector2 totalGravity = Vector2.zero;

            for (int i = 0; i < _sourceRigidbodies.Count; i++)
            {
                Vector2 sourcePos = _sourceColliders[i].transform.position;
                Vector2 objectPos = rb.transform.position;

                Vector2 direction = sourcePos - objectPos;
                float distance = direction.magnitude;

                if (distance < 0.1f) continue;

                float gravityStrength =
                    globalGravityForce * _sourceRigidbodies[i].mass / (distance * distance);

                totalGravity += direction.normalized * gravityStrength;
            }

            rb.AddForce(totalGravity);
        }
    }

    // --------------------------------------------------------------------
    // MÉTODO OFICIAL QUE USARÁN TODOS LOS SCRIPTS
    // --------------------------------------------------------------------

    public Vector2 GetGravityForce(Vector2 objectPosition)
    {
        Vector2 totalGravity = Vector2.zero;

        for (int i = 0; i < _sourceRigidbodies.Count; i++)
        {
            Vector2 sourcePos = _sourceColliders[i].transform.position;
            Vector2 direction = sourcePos - objectPosition;
            float distance = direction.magnitude;

            if (distance < 0.1f) continue;

            float gravityStrength =
                globalGravityForce * _sourceRigidbodies[i].mass / (distance * distance);

            totalGravity += direction.normalized * gravityStrength;
        }

        return totalGravity;
    }
}
