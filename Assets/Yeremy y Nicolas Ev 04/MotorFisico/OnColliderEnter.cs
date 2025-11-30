using System.Linq;
using PUCV.PhysicEngine2D;
using UnityEngine;
using UnityEngine.Events;

public class OnColliderEnter : MonoBehaviour, IHasCollider
{
    public LayerMask layer;
    public UnityEvent eventoToEnterCollider;
    public UnityEvent eventoToExitCollider;

    public void OnInformCollisionEnter2D(CollisionInfo collisionInfo)
    {
        if (collisionInfo.otherCollider.gameObject.layer == LayerToInt(layer))
        {
            eventoToEnterCollider?.Invoke();
        }
    }

    public void OnInformCollisionExit2D(CollisionInfo collisionInfo)
    {
        if (collisionInfo.otherCollider != null)
        {
            if (collisionInfo.otherCollider.gameObject.layer == LayerToInt(layer))
            {
                eventoToExitCollider?.Invoke();
            }
        }
    }

    public static int LayerToInt(LayerMask _layer)
    {
        int layer = Mathf.RoundToInt(Mathf.Log(_layer.value, 2));
        return layer;
    }
}