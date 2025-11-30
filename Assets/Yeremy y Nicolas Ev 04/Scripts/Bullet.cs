using PUCV.PhysicEngine2D;
using UnityEngine;

// Aseguramos que el objeto tenga un collider para detectar colisiones
public class Bullet : MonoBehaviour, IHasCollider
{
    public float speed = 10f;      // Velocidad de la bala
    public float mass = 1f;
    public float restitution = 0.7f;

    public CustomRigidbody2D customRigidbody;
    public Transform t_reference;

    Transform gravityAffector;
    Transform waterAffector;
    public LayerMask layer;
    public LayerMask waterLayer;

    public float timer = 5;

    private float time;
    float rotZ;
    float density;
    float buoyancyForceMultiplier;
    void Start()
    {
        rotZ = t_reference.eulerAngles.z;
    }

    float gravityAff;

    void Update()
    {
        if (gravityAffector != null)
        {
            // Aquí calculas la dirección hacia el gravityAffector
            Vector2 direccion = (gravityAffector.position - transform.position).normalized;
            
            // Aplicar alguna fuerza proporcional a la gravedad
            customRigidbody.velocity += direccion * gravityAff * Time.deltaTime;
            transform.up = customRigidbody.velocity;
        }
        else if (waterAffector != null)
        {
           Vector2 velocity = customRigidbody.velocity;

            // Arrastre horizontal y vertical
            Vector2 dragForce = -velocity.normalized * density * velocity.sqrMagnitude * Time.deltaTime;
            velocity += dragForce;

            // Flotación: fuerza hacia arriba en Y global (no depende de la rotación del objeto)
            float buoyancyForce = density * 0.5f;
            float upwardSpeed = Mathf.Max(0, buoyancyForce * 20 - velocity.y);
            velocity.y += upwardSpeed * Time.deltaTime;

            // Limitar velocidad máxima vertical
            float maxUpSpeed = 10f;
            float maxDownSpeed = -5f; // Añade también límite hacia abajo si es necesario
            velocity.y = Mathf.Clamp(velocity.y, maxDownSpeed, maxUpSpeed);

            customRigidbody.velocity = velocity;

            // Rotación del sprite (opcional, si quieres que rote con la velocidad)
            if (velocity.sqrMagnitude > 0.001f)
            transform.up = velocity.normalized;
        }
        else
        {
            // Movimiento normal en dirección inicial
            float rotRad = rotZ * Mathf.Deg2Rad;
            Vector2 direccion = new Vector2(-Mathf.Sin(rotRad), Mathf.Cos(rotRad));
            customRigidbody.velocity = direccion * speed;
            transform.up = direccion;
        }

        time += Time.deltaTime;
        if (time >= timer)
        {
            Destroy(gameObject);
            time = 0;
        }
    }

    public void SetGravityAffector(Transform affector)
    {
        gravityAffector = affector;
    }

    public void OnInformCollisionEnter2D(CollisionInfo collisionInfo)
    {
        if (collisionInfo.otherCollider.gameObject.layer == OnColliderEnter.LayerToInt(layer))
        {
            Debug.Log("Tocamos planeta");
            gravityAff = collisionInfo.otherCollider.gameObject.GetComponent<GravityAffector>().gravity;
            SetGravityAffector(collisionInfo.otherCollider.transform);
        }
        
        if (collisionInfo.otherCollider.gameObject.layer == OnColliderEnter.LayerToInt(waterLayer))
        {
            Debug.Log("Tocamos el agua");
            buoyancyForceMultiplier = collisionInfo.otherCollider.gameObject.GetComponent<WaterAffector>().buoyancy;
            density = collisionInfo.otherCollider.gameObject.GetComponent<WaterAffector>().waterDensity;
            waterAffector = collisionInfo.otherCollider.transform;
        }
    }

    public void OnInformCollisionExit2D(CollisionInfo collisionInfo)
    {
        if (collisionInfo.otherCollider != null)
        {
            if (collisionInfo.otherCollider.gameObject.layer == OnColliderEnter.LayerToInt(layer))
            {
                SetGravityAffector(null);
                rotZ = transform.localEulerAngles.z;
            }
        }
    }
}
