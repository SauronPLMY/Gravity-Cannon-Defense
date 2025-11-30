using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public int health = 3;

    private Transform player;
    private GameRigidbody2D rb;

    void Start()
    {
        rb = GetComponent<GameRigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
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
}
