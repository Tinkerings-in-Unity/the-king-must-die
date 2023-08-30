    using Opsive.Shared.Audio;
    using Opsive.Shared.Game;
    using Opsive.UltimateCharacterController.Game;
    using Opsive.UltimateCharacterController.Items.Actions.Impact;
    using Opsive.UltimateCharacterController.Traits.Damage;
    using System.Collections.Generic;
    using Opsive.UltimateCharacterController.Character.Identifiers;
    using Opsive.UltimateCharacterController.Objects;
    using UnityEngine;

    /// <summary>
    /// Creates an explosion which applies a force and damage to any object that is within the specified radius.
    /// </summary>
    public class ProjectileDamage : MonoBehaviour, IDamageSource
    {
        [Tooltip("Should the object explode when the object is enabled?")]
        [SerializeField] protected bool m_ExplodeOnEnable;
        [Tooltip("Determines how far out the explosion affects other objects.")]
        [SerializeField] protected float m_Radius = 5;
        [Tooltip("The impact damage data.")]
        [SerializeField] protected ImpactDamageData m_ImpactDamageData = new ImpactDamageData()
        {
            DamageAmount = 10,
            ImpactForce = 2,
            ImpactForceFrames = 1,
            LayerMask = ~(1 << LayerManager.IgnoreRaycast | 1 << LayerManager.Water | 1 << LayerManager.SubCharacter | 1 << LayerManager.Overlay | 
                          1 << LayerManager.VisualEffect)
        };
        [Tooltip("The Impact Actions.")]
        [SerializeField] protected ImpactActionGroup m_ImpactActionGroup = ImpactActionGroup.DefaultDamageGroup(true);
        [Tooltip("Does the explosion require line of sight in order to damage the hit object?")]
        [SerializeField] protected bool m_LineOfSight;
        [Tooltip("The duration of the explosion.")]
        [SerializeField] protected float m_Lifespan = 3;
        [Tooltip("The maximum number of objects that the explosions can detect.")]
        [SerializeField] protected int m_MaxCollisionCount = 100;
        [Tooltip("A set of AudioClips that can be played when the explosion occurs.")]
        [SerializeField] protected AudioClipSet m_ExplosionAudioClipSet = new AudioClipSet();

        public bool ExplodeOnEnable { get { return m_ExplodeOnEnable; } set { m_ExplodeOnEnable = value; } }
        public float Radius { get { return m_Radius; } set { m_Radius = value; } }
        public ImpactDamageData ImpactDamageData { get { return m_ImpactDamageData; } set { m_ImpactDamageData = value; } }
        public ImpactActionGroup ImpactActionGroup { get { return m_ImpactActionGroup; } set { m_ImpactActionGroup = value; } }
        public bool LineOfSight { get { return m_LineOfSight; } set { m_LineOfSight = value; } }
        public float Lifespan { get { return m_Lifespan; } set { m_Lifespan = value; } }
        public AudioClipSet ExplosionAudioClipSet { get { return m_ExplosionAudioClipSet; } set { m_ExplosionAudioClipSet = value; } }

        [System.NonSerialized] private GameObject m_GameObject;
        private Transform m_Transform;
        private HashSet<object> m_ObjectExplosions = new HashSet<object>();
        private Collider[] m_CollidersHit;
        private RaycastHit m_RaycastHit;
        private ScheduledEventBase m_DestructionEvent;
        private ImpactCallbackContext m_CachedImpactCallbackContext;
        private ImpactCollisionData m_CachedImpactCollisionData;
        private ImpactDamageData m_CachedDamageData;
        private GameObject m_Owner;
        private IDamageSource m_OwnerDamageSource;

        IDamageSource IDamageSource.OwnerDamageSource => m_OwnerDamageSource;
        GameObject IDamageSource.SourceOwner => m_Owner;
        GameObject IDamageSource.SourceGameObject => gameObject;
        Component IDamageSource.SourceComponent => this;

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        private void Awake()
        {
            m_GameObject = gameObject;
            m_Transform = transform;
            m_CollidersHit = new Collider[m_MaxCollisionCount];
            m_ImpactActionGroup.Initialize(gameObject,null);
            m_CachedDamageData = new ImpactDamageData();
            m_CachedImpactCollisionData = new ImpactCollisionData();
            m_CachedImpactCallbackContext = new ImpactCallbackContext();
            m_ImpactActionGroup.ImpactActions[m_ImpactActionGroup.Count - 1] = new CharacterKnockBack();
        }

        /// <summary>
        /// Explode if requested when the component is enabled.
        /// </summary>
        private void OnEnable()
        {
            if (m_ExplodeOnEnable) {
                Explode(m_ImpactDamageData, null, null);
            }
        }

        public void Explode(GameObject hitObject)
        {
            Explode(m_ImpactDamageData, null, hitObject, null);
        }


        public void Explode(IImpactDamageData impactDamageData, GameObject owner, GameObject hitObject, IDamageSource ownerSource = null)
        {
            if (impactDamageData == null) {
                Debug.LogError("The impact damage data cannot be null.");
                return;
            }

            m_OwnerDamageSource = ownerSource;
            m_Owner = owner == null ? gameObject : owner;
            
            var layerMask = impactDamageData.LayerMask;

            if (hitObject.CompareTag("Enemy"))
            {
                m_ObjectExplosions.Add(hitObject);

                var hitCollider = hitObject.GetComponentInChildren<CharacterColliderBaseIdentifier>()
                    .GetComponentInChildren<CapsuleCollider>();

                Vector3 closestPoint;
                if (hitCollider is BoxCollider || hitCollider is SphereCollider || hitCollider is CapsuleCollider ||
                    (hitCollider is MeshCollider ))
                {
                    closestPoint = hitCollider.ClosestPoint(m_Transform.position);
                }
                else
                {
                    closestPoint = hitCollider.ClosestPointOnBounds(m_Transform.position);
                }

                var hitDirection = closestPoint - m_Transform.position;
                var damageModifier = Mathf.Max(1 - (hitDirection.magnitude / m_Radius), 0.01f);

                var impactData = GetImpactData();
                {
                    impactData.SetImpactSource(this);
                    impactData.SetImpactTarget(hitCollider);
                    impactData.ImpactPosition = closestPoint;
                    impactData.ImpactDirection = hitDirection;
                    impactData.ImpactStrength = damageModifier;
                }

                var impactCallback = m_CachedImpactCallbackContext;
                impactCallback.ImpactCollisionData = impactData;
                impactCallback.ImpactDamageData = m_ImpactDamageData;
                m_ImpactActionGroup.OnImpact(impactCallback, true);
            }

            m_ObjectExplosions.Clear();

            // An audio clip can play when the object explodes.
            m_ExplosionAudioClipSet.PlayAudioClip(m_GameObject);

            // m_DestructionEvent = Scheduler.Schedule(m_Lifespan, Destroy);
        }

        /// <summary>
        /// Get the impact data for a raycast hit.
        /// </summary>
        /// <returns>The impact data with the raycast hit.</returns>
        public virtual ImpactCollisionData GetImpactData()
        {
            var impactData = m_CachedImpactCollisionData;
            impactData.Reset();
            impactData.Initialize();
            return impactData;
        }

        /// <summary>
        /// The object has been disabled.
        /// </summary>
        public void OnDisable()
        {
            if (m_DestructionEvent != null) {
                Scheduler.Cancel(m_DestructionEvent);
                m_DestructionEvent = null;
            }
        }

        /// <summary>
        /// Place the object back in the ObjectPool.
        /// </summary>
        private void Destroy()
        {
            m_DestructionEvent = null;
            ObjectPoolBase.Destroy(gameObject);
        }
    }