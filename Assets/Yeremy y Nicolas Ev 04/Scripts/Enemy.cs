using PUCV.PhysicEngine2D;
using UnityEngine;

public class Enemy : MonoBehaviour, IHasCollider
{
    public float speed = 2f;
    public int health = 3;

    private Transform player;
    private CustomRigidbody2D rb;
    public LayerMask bulletLayer;

    void Start()
    {
        rb = GetComponent<CustomRigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;

        initialColor = GetComponent<SpriteRenderer>().color;
    }

    void Update()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = dir * speed;
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
            Destroy(gameObject);
    }

    private Color initialColor;
    public void OnInformCollisionEnter2D(CollisionInfo collisionInfo)
    {
        if (collisionInfo.otherCollider.gameObject.layer == OnColliderEnter.LayerToInt(bulletLayer))
        {
            TakeDamage(1);
            GetComponent<SpriteRenderer>().color = Color.white;
            Invoke(nameof(Restore), 0.15f);
        }
    }

    void Restore()
    {
        GetComponent<SpriteRenderer>().color = initialColor;
    }

    public void OnInformCollisionExit2D(CollisionInfo collisionInfo)
    {
    }
}
