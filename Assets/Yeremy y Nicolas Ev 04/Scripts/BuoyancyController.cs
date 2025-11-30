using UnityEngine;

/// <summary>
/// Controla la flotabilidad de los proyectiles u otros objetos dentro de un fluido.
/// Detecta colisiones con el agua y aplica fuerza de empuje y arrastre.
/// </summary>
public class BuoyancyController : MonoBehaviour
{
    public float buoyancyForce = 25f;  // Fuerza hacia arriba aplicada a objetos
    public float waterDrag = 0.8f;     // Factor de reducción de velocidad dentro del agua

    private GameCollider2D _waterCollider; // Collider que representa el agua

    void Start()
    {
        // Obtener el collider del agua desde este objeto
        _waterCollider = GetComponent<GameCollider2D>();
    }

    void Update()
    {
        // Revisar todos los cuerpos registrados para ver si están dentro del agua
        CheckBuoyancyForAllBodies();
    }

    void CheckBuoyancyForAllBodies()
    {
        // Obtener todos los Rigidbody del juego
        GameRigidbody2D[] allBodies = FindObjectsByType<GameRigidbody2D>(FindObjectsSortMode.None);

        foreach (GameRigidbody2D rb in allBodies)
        {
            // Solo aplicar a proyectiles y objetos que tengan Rigidbody válido
            if (rb != null && rb.CompareTag("Projectile") && IsInWater(rb))
            {
                // Aplicar fuerza de flotabilidad
                ApplyBuoyancy(rb);
            }
        }
    }

    bool IsInWater(GameRigidbody2D rb)
    {
        if (_waterCollider == null) return false;

        GameCollider2D objectCollider = rb.GetComponent<GameCollider2D>();
        if (objectCollider == null) return false;

        // Revisar colisión entre el objeto y el agua
        bool collision = _waterCollider.CheckCollision(objectCollider);

        // Dibujar debug line para ver visualmente en escena (opcional)
        if (collision)
        {
            Debug.DrawLine(_waterCollider.Center, objectCollider.Center, Color.green, 0.1f);
        }

        return collision;
    }

    void ApplyBuoyancy(GameRigidbody2D rb)
    {
        // Aplicar fuerza hacia arriba proporcional a la fuerza de flotación
        rb.AddForce(Vector2.up * buoyancyForce);

        // Aplicar arrastre en el agua reduciendo velocidad
        rb.velocity *= waterDrag;
    }
}
