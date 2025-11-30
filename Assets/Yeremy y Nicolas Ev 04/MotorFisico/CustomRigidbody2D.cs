using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PUCV.PhysicEngine2D
{
    public class CustomRigidbody2D : MonoBehaviour
    {
        public Vector2 velocity;
        //-> COMPONENTES BÁSICOS DEL RBD
        public float restitutionValue = 1;
        public float frictionValue = 0.5f;
        public float dampingValue = 0.7f;

        private CustomCollider2D _customCollider;

        public Vector2 GetWorldPosition()
        {
            return transform.position;
        }
        
        public void SetWoldPosition(Vector2 newPos)
        {
            transform.position = newPos;
        }

        public CustomCollider2D GetCollider()
        {
            return _customCollider;
        }
    }
}
