
using Opsive.Shared.Events;
using Opsive.Shared.Game;
using Opsive.Shared.StateSystem;
using Opsive.UltimateCharacterController.Camera;
using Opsive.UltimateCharacterController.Character;
using System.Collections.Generic;
using UnityEngine;

public class CustomObjectFader : StateBehavior
{
    [Tooltip("The radius of the camera's collision sphere to prevent it from clipping with other objects.")]
    [SerializeField]
    protected float m_CollisionRadius = 0.05f;

    [Tooltip("The alpha value that the obstructed object will fade to.")] 
    [SerializeField] private float fadeAlphaValue = 0.1f;

    [Tooltip("The maximum number of obstructing colliders that can be faded at one time.")] [SerializeField]
    protected int m_MaxObstructingColliderCount = 20;

    [Tooltip(
        "The offset to apply to the character when determining if the character should fade/is considered obstructed.")]
    [SerializeField]
    protected Vector3 m_TransformOffset = new Vector3(0, 0.9f, 0);

    private Transform m_Transform;
    private CameraController m_CameraController;

    private GameObject m_Character;
    private UltimateCharacterLocomotion m_CharacterLocomotion;
    private CharacterLayerManager m_CharacterLayerManager;
    private Transform m_CharacterTransform;
    private Vector3 m_FadeOffset;

    private RaycastHit[] m_RaycastsHit;
    private HashSet<Material> m_ObstructionHitSet;


    protected override void Awake()
    {
        base.Awake();

        m_Transform = transform;
        m_CameraController = gameObject.GetCachedComponent<CameraController>();

        m_RaycastsHit = new RaycastHit[m_MaxObstructingColliderCount];
        m_ObstructionHitSet = new HashSet<Material>();

        EventHandler.RegisterEvent<GameObject>(gameObject, "OnCameraAttachCharacter", OnAttachCharacter);

        enabled = false;
    }

    protected virtual void OnAttachCharacter(GameObject character)
    {
        enabled = character != null && !m_CameraController.ActiveViewType.FirstPersonPerspective;

        m_Character = character;

        if (m_Character != null)
        {
            m_CharacterTransform = m_Character.transform;
            m_CharacterLocomotion = m_Character.GetCachedComponent<UltimateCharacterLocomotion>();
            m_CharacterLayerManager = m_Character.GetCachedComponent<CharacterLayerManager>();


            EventHandler.RegisterEvent(m_Character, "OnRespawn", OnRespawn);

            FadeObstructingObjects();
        }
    }

    private void Update()
    {
        FadeObstructingObjects();
    }

    private void FadeObstructingObjects()
    {
        var characterPosition = m_CharacterTransform.TransformPoint(m_TransformOffset);
        var direction = (m_Transform.position - characterPosition);
        var start = characterPosition - direction.normalized * m_CollisionRadius;
        m_CharacterLocomotion.EnableColliderCollisionLayer(false);
        // Fire a sphere to prevent the camera from colliding with other objects.
        var hitCount = Physics.SphereCastNonAlloc(start, m_CollisionRadius, direction.normalized, m_RaycastsHit,
            direction.magnitude, m_CharacterLayerManager.IgnoreInvisibleCharacterWaterLayers,
            QueryTriggerInteraction.Ignore);
        m_CharacterLocomotion.EnableColliderCollisionLayer(true);

        foreach (var material in m_ObstructionHitSet)
        {
            material.SetFloat("_Alpha", 1f);
        }

        m_ObstructionHitSet.Clear();
        if (hitCount > 0)
        {
            for (int i = 0; i < hitCount; ++i)
            {
                var renderers = m_RaycastsHit[i].transform.gameObject.GetCachedComponents<Renderer>();
                var obstructing = false;
                for (int j = 0; j < renderers.Length; ++j)
                {
                    var materials = renderers[j].materials;
                    for (int k = 0; k < materials.Length; ++k)
                    {
                        var material = materials[k];
                        m_ObstructionHitSet.Add(material);
                    }
                }
            }
        }

        foreach (var material in m_ObstructionHitSet)
        {
            material.SetFloat("_Alpha", fadeAlphaValue);
        }
    }


    private void OnRespawn()
    {
        FadeObstructingObjects();
    }

    public override void StateChange()
    {
        foreach (var material in m_ObstructionHitSet)
        {
            material.SetFloat("_Alpha", 1f);
        }

        m_ObstructionHitSet = new HashSet<Material>();
    }

    private void OnDestroy()
    {
        OnAttachCharacter(null);

        EventHandler.UnregisterEvent<GameObject>(gameObject, "OnCameraAttachCharacter", OnAttachCharacter);
    }
}