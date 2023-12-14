using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using UnityEngine;
using Opsive.Shared.Game;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.Shared.Events;
using Opsive.Shared.StateSystem;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using Opsive.UltimateCharacterController.Traits;
using Rewired;
using UnityEngine.Rendering.Universal;
using EventHandler = Opsive.Shared.Events.EventHandler;

public class AOEProjectileFireAbility : MonoBehaviour
{
    [SerializeField] private float canFireAgainDelay = 1f;
    [SerializeField] private float targetingRange;
    [SerializeField] private AudioClip bleepAudioClip;
    [SerializeField] private MMFeedbacks bleepStartFeedback;
    [SerializeField] private MMFeedbacks bleepEndFeedback;
    public DecalProjector MainIndicator;
    
    
    private AudioSource _audioSource;
    private UltimateCharacterLocomotion _characterLocomotion;
    private bool _canFire = true;
    private bool _aoeActivated;
    private Player _player;
    private List<Use> _useAbilities = new List<Use>();
    private Vector3 _targetPoint = Vector3.zero;
    private Transform _cachedTransform;
    private RestrictPosition _restrictPositionAbility;
    
    public void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _cachedTransform = transform;
    }
    
    
    private void Start()
    {
        _characterLocomotion = gameObject.GetComponent<UltimateCharacterLocomotion>();
        _restrictPositionAbility = _characterLocomotion.GetAbility<RestrictPosition>();
        // _useAbilities = _characterLocomotion.GetAbilities<Use>().ToList();
        _player = ReInput.players.GetPlayer(0);
    }

    private void Update()
    {
        if (_player.GetButtonDown("Action") && _canFire)
        {
            Target(_cachedTransform.position);
        }
        
        if (_player.GetButtonUp("Action") && _aoeActivated)
        {
            FireAOE();
        }

        if (!_aoeActivated)
        {
            return;
        }
        
        var vertical = _player.GetAxis("Vertical");
        var horizontal = _player.GetAxis("Horizontal");
        
        Vector3 targetPos = (_cachedTransform.position + new Vector3(vertical, 2f, horizontal));
        Vector3 Distance = targetPos - _cachedTransform.position;
        Distance = Vector3.ClampMagnitude((Distance * 2), targetingRange);
        
        // _targetPoint = _cachedTransform.position + Distance;

        // var newTargetPoint = new Vector3(_targetPoint.x + vertical, _targetPoint.z, _targetPoint.z + horizontal);
        //     
        var dis = Vector3.Distance(_cachedTransform.position, new Vector3(targetPos.x, _cachedTransform.position.y, targetPos.z));

        // if (dis > targetingRange)
        // {
        //     return;
        // }

        // if (dis <= targetingRange)
        // {
        //     _targetPoint = targetPos;
        // }

        Debug.Log($"PLayer forward {_cachedTransform.forward}");
        var forwardPos = _cachedTransform.forward + new Vector3(vertical, 2f, horizontal);
        Debug.Log($"Forward + Input {forwardPos}");
        var newPos = _cachedTransform.position + forwardPos;
        Debug.Log($"New Pos {newPos}");
        _targetPoint += new Vector3(forwardPos.x, 0f, forwardPos.z);
        Debug.Log(_targetPoint);
        
        
        MainIndicator.transform.position = _targetPoint;


    }

    
    private void Target(Vector3 currentPos)
    {
        // EventHandler.ExecuteEvent(gameObject, "OnEnableGameplayInput", false);
        
        MainIndicator.enabled = true;
        MainIndicator.material.SetFloat("_Fill", 0f);

        _targetPoint = currentPos + new Vector3(0f, 2f, targetingRange);
        
        _restrictPositionAbility.MinXPosition = currentPos.x;
        _restrictPositionAbility.MaxXPosition = currentPos.x;
        _restrictPositionAbility.MinZPosition = currentPos.z;
        _restrictPositionAbility.MaxZPosition = currentPos.z;

        _characterLocomotion.TryStartAbility(_restrictPositionAbility);
        Debug.Log("Targeting");
        _canFire = false;
               
        _aoeActivated = true;
        // _characterLocomotion.TryStartAbility(_characterLocomotion.GetAbility<Generic>());
    }
    
    
    private void FireAOE()
    {
        // _characterLocomotion.TryStopAbility(_characterLocomotion.GetAbility<Generic>());
        _characterLocomotion.TryStopAbility(_restrictPositionAbility);
        Debug.Log("Fire");
        _aoeActivated = false;
        EventHandler.ExecuteEvent(gameObject, "OnEnableGameplayInput", true);
        MainIndicator.enabled = false;
        SchedulerBase.Schedule(canFireAgainDelay, ResetFiring);
    }
    
    private void ResetFiring()
    {
        _canFire = true;
    }
}
