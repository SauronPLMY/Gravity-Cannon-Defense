using PUCV.PhysicEngine2D;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;
    private float nextFire;

    public CustomRigidbody2D customRigidBody;

    void Update()
    {
        MoveVertical();
        AimMouse();
        AutoShoot();
    }

    void MoveVertical()
    {
        float v = Input.GetAxis("Vertical");
        customRigidBody.velocity = new Vector2(0, v * speed);
    }

    void AimMouse()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouse - transform.position;

        float angle = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void AutoShoot()
    {
        if (Time.time >= nextFire)
        {
            GameObject obj = Instantiate(bulletPrefab, firePoint.position, quaternion.identity);
            obj.TryGetComponent(out Bullet blt);
            blt.t_reference = transform;
            nextFire = Time.time + fireRate;
        }
    }
}
