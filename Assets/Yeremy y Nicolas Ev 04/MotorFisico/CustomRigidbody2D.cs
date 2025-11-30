using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PUCV.PhysicEngine2D
{
    public class CustomRigidbody2D : MonoBehaviour
    {
        public Vector2 velocity;
        public bool m_canRebote;

        //-> COMPONENTES BÁSICOS DEL RBD
        public float mass = 1.0f;
        public float restitution = 1;
        public float friction = 0.2f;
        public float linearDamping = 0.01f;
        public float m_damping = 0.7f;

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
