using System.Collections.Generic;
using UnityEngine;

namespace PUCV.PhysicEngine2D
{
    public class PhysicsManager2D : MonoBehaviour
    {
        private static PhysicsManager2D _instance;
        private bool _registered;
        private List<CustomCollider2D> _colliders = new List<CustomCollider2D>();
        
        //Per FixedUpdateList
        private List<InternalCollisionInfo> _currentCollisionList = new List<InternalCollisionInfo>();
        private List<InternalCollisionInfo> _prevCollisionList = new List<InternalCollisionInfo>();

        private void Awake()
        {
            if (_instance != null)
            {
                DestroyImmediate(this);
                return;
            }
            
            //Singleton
            _instance = this;
            if (transform.parent != null) 
            {
                transform.parent = null;
                DontDestroyOnLoad(_instance);
            }
        }

        public static void RegisterCollider(CustomCollider2D customCollider2D)
        {
            if(_instance) _instance._colliders.Add(customCollider2D);
        }

        public static void UnregisterCollider(CustomCollider2D customCollider2D)
        {
            if(_instance) _instance._colliders.Remove(customCollider2D);
        }

        private void FixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            StepCalculateCollisions(deltaTime);
            StepApplyMTVAndReflectionToRigidbodies(deltaTime);
            StepApplyMovementToRigidbodies(deltaTime);
            StepInformCollisionsEnter(deltaTime);
            StepInformCollisionsExit(deltaTime);
        }

        private void StepApplyMTVAndReflectionToRigidbodies(float deltaTime)
        {
            
            foreach (var currCollisionInfo in _currentCollisionList)
            {
                if (currCollisionInfo.bodyACollider.m_isTrigger ||
                    currCollisionInfo.bodyBCollider.m_isTrigger)
                {
                    continue;
                }

                var customRigidbody2DA = currCollisionInfo.bodyARigidbody;
                var customRigidbody2DB = currCollisionInfo.bodyBRigidbody;

                //Move rigidbodies according to MTV
                if (currCollisionInfo.hasMTV)
                {
                    if (customRigidbody2DA)
                    {
                        Vector2 position = customRigidbody2DA.GetWorldPosition();
                        position += currCollisionInfo.mtvA;
                        customRigidbody2DA.SetWoldPosition(position);
                    }
                    if (customRigidbody2DB)
                    {
                        Vector2 position = customRigidbody2DB.GetWorldPosition();
                        position += currCollisionInfo.mtvB;
                        customRigidbody2DB.SetWoldPosition(position);
                    }
                }
                
                
                //Reflect velocities
                if (customRigidbody2DA)
                {
                    if (!customRigidbody2DA.m_canRebote) 
                    {
                        customRigidbody2DA.velocity = new Vector2(customRigidbody2DA.velocity.x, 0);
                        continue;
                    }

                    // Vector2 velocity = customRigidbody2DA.velocity;
                    // velocity = currCollisionInfo.contactNormalAB.normalized*velocity.magnitude;
                    // customRigidbody2DA.velocity = velocity;

                    Vector2 v = customRigidbody2DA.velocity;
                    Vector2 n = currCollisionInfo.contactNormalAB.normalized;
                    
                    float initialEnergy = v.sqrMagnitude;
                    
                    float restitution = customRigidbody2DA.restitution;
                    
                    float normalSpeed = Vector2.Dot(v, n);
                    Vector2 normalComponent = normalSpeed * n;
                    Vector2 tangentComponent = v - normalComponent;
                    
                    Vector2 reflectedNormal = -normalComponent * restitution;
                    
                    float friction = customRigidbody2DA.friction;
                    Vector2 dampedTangent = tangentComponent * (1f - friction);
                    
                    Vector2 finalVelocity = reflectedNormal + dampedTangent;
                    
                    float energyRatio = finalVelocity.sqrMagnitude / Mathf.Max(initialEnergy, 0.001f);
                    if (energyRatio > 1.2f)
                    {
                        finalVelocity = finalVelocity.normalized * Mathf.Sqrt(initialEnergy);
                    }
                    
                    customRigidbody2DA.velocity = finalVelocity * customRigidbody2DA.m_damping;
                }
                if (customRigidbody2DB)
                {
                    if (!customRigidbody2DB.m_canRebote) 
                    {
                        customRigidbody2DB.velocity = new Vector2(customRigidbody2DB.velocity.x, 0);
                        continue;
                    }
                    // Vector2 velocity = customRigidbody2DB.velocity;
                    // velocity = currCollisionInfo.contactNormalBA*velocity.magnitude;
                    // customRigidbody2DB.velocity = velocity;

                    Vector2 v = customRigidbody2DB.velocity;
                    Vector2 n = currCollisionInfo.contactNormalAB.normalized;
                    
                    float initialEnergy = v.sqrMagnitude;
                    
                    float restitution = customRigidbody2DB.restitution;
                    
                    float normalSpeed = Vector2.Dot(v, n);
                    Vector2 normalComponent = normalSpeed * n;
                    Vector2 tangentComponent = v - normalComponent;
                    
                    Vector2 reflectedNormal = -normalComponent * restitution;
                    
                    float friction = customRigidbody2DB.friction;
                    Vector2 dampedTangent = tangentComponent * (1f - friction);
                    
                    Vector2 finalVelocity = reflectedNormal + dampedTangent;
                    
                    float energyRatio = finalVelocity.sqrMagnitude / Mathf.Max(initialEnergy, 0.001f);
                    if (energyRatio > 1.2f)
                    {
                        finalVelocity = finalVelocity.normalized * Mathf.Sqrt(initialEnergy);
                    }
                    
                    customRigidbody2DB.velocity = finalVelocity * customRigidbody2DB.m_damping;
                }
            }
        }

        private void StepCalculateCollisions(float deltaTime)
        {
            _prevCollisionList = new List<InternalCollisionInfo>(_currentCollisionList);

            //var currCollisionList = SimpleCollisionMath2D.DetectCollisions(_colliders);
            var currCollisionList = SAT2DMath.DetectCollisions(_colliders);
            
            //Check if collision was present last frame
            foreach (var currCollisionInfo in currCollisionList)
            {
                currCollisionInfo.wasCollidedLastFrame = false;

                foreach (var prevCollisionInfo in _prevCollisionList)
                {
                    if ((prevCollisionInfo.bodyACollider == currCollisionInfo.bodyACollider &&
                         prevCollisionInfo.bodyBCollider == currCollisionInfo.bodyBCollider) ||
                        (prevCollisionInfo.bodyACollider == currCollisionInfo.bodyBCollider &&
                         prevCollisionInfo.bodyBCollider == currCollisionInfo.bodyACollider))
                    {
                        currCollisionInfo.wasCollidedLastFrame = true;
                        break;
                    }
                }
            }

            _currentCollisionList = currCollisionList;
        }
        
        private void StepInformCollisionsEnter(float deltaTime)
        {
            foreach (var currCollisionInfo in _currentCollisionList)
            {
                if (currCollisionInfo.wasCollidedLastFrame) continue;
                CollisionInfo a = currCollisionInfo.GetCollInfoForBodyA();
                CollisionInfo b = currCollisionInfo.GetCollInfoForBodyB();
                currCollisionInfo.bodyACollider.InformOnCollisionEnter2D(a);
                currCollisionInfo.bodyBCollider.InformOnCollisionEnter2D(b);
            }
        }

        private void StepInformCollisionsExit(float deltaTime)
        {
            foreach (var prev in _prevCollisionList)
            {
                bool stillColliding = _currentCollisionList.Exists(curr =>
                    (prev.bodyACollider == curr.bodyACollider && prev.bodyBCollider == curr.bodyBCollider) ||
                    (prev.bodyACollider == curr.bodyBCollider && prev.bodyBCollider == curr.bodyACollider)
                );

                if (!stillColliding)
                {
                    // Disparar exit
                    CollisionInfo a = prev.GetCollInfoForBodyA();
                    CollisionInfo b = prev.GetCollInfoForBodyB();
                    prev.bodyACollider.InformOnCollisionExit2D(a);
                    prev.bodyBCollider.InformOnCollisionExit2D(b);
                }
            }
        }

        void StepApplyMovementToRigidbodies(float deltaTime)
        {
            if (_colliders == null || _colliders.Count == 0) return;
            foreach (CustomCollider2D collider in _colliders)
            {
                CustomRigidbody2D rigidbody = collider.rigidBody;
                if (rigidbody == null) continue;
                Vector2 rigidbodyPos = rigidbody.GetWorldPosition();
                rigidbodyPos += rigidbody.velocity*deltaTime;
                rigidbody.SetWoldPosition(rigidbodyPos);
            }
        }
    }
    
    public class InternalCollisionInfo
    {
        public CustomCollider2D bodyACollider;
        public CustomRigidbody2D bodyARigidbody;
        public CustomCollider2D bodyBCollider;
        public CustomRigidbody2D bodyBRigidbody;
        public bool wasCollidedLastFrame;
        //Minimum Translation Vector
        public bool hasMTV;
        public Vector2 mtvA;
        public Vector2 mtvB;

        public int missingFrames;

        public Vector2 contactPoint;
        public Vector2 contactNormalAB;
        public Vector2 contactNormalBA;

        public InternalCollisionInfo(
            CustomCollider2D colA, 
            CustomCollider2D colB, 
            Vector2 point, 
            Vector2 normal
            )
        {
            bodyACollider = colA;
            bodyARigidbody = colA.rigidBody;
            bodyBCollider = colB;
            bodyBRigidbody = colB.rigidBody;
            contactPoint = point;
            contactNormalAB = -normal;
            contactNormalBA = normal;
        }

        public CollisionInfo GetCollInfoForBodyA()
        {
            return new CollisionInfo()
            {
                otherCollider = bodyBCollider,
                otherRigidbody = bodyBRigidbody,
                contactPoint = contactPoint,
                contactNormal = contactNormalAB,
                mtv = mtvA,
            };
        }

        public CollisionInfo GetCollInfoForBodyB()
        {
            return new CollisionInfo()
            {
                otherCollider = bodyACollider,
                otherRigidbody = bodyARigidbody,
                contactPoint = contactPoint,
                contactNormal = contactNormalBA,
                mtv = mtvB,
            };
        }
    }

    public class CollisionInfo
    {
        public CustomCollider2D otherCollider;
        public CustomRigidbody2D otherRigidbody;

        public Vector2 contactPoint;
        public Vector2 contactNormal;

        public Vector2 mtv;
    }
}

