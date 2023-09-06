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
using Opsive.UltimateCharacterController.Traits;
using Rewired;
using Utility.Events;
using EventHandler = Opsive.Shared.Events.EventHandler;


public class LockOnAbility : MonoBehaviour
{
    [SerializeField] private bool faceTarget;
    [SerializeField] private float searchRange;
    [SerializeField] private AudioClip bleepAudioClip;
    [SerializeField] private MMFeedbacks bleepStartFeedback;
    [SerializeField] private MMFeedbacks bleepEndFeedback;
    
    
    private AudioSource _audioSource;
    private UltimateCharacterLocomotion _characterLocomotion;
    private Player _player;
    private Dictionary<float,Transform> _activeEnemies;
    private Transform _currentTarget;
    private State _magicProjectileState;
    private State _useState;
    private RotateTowards _rotateTowards;
    
    public void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        EventBus.Subscribe<WeaponAttackingEvent>(OnAttackStateChanged);
        EventBus.Subscribe<StopAttackStateEvent >(OnStopAttackState);
    }

    private void OnAttackStateChanged(WeaponAttackingEvent updateEvent)
    {

        // switch (updateEvent.First)
        // {
        //     case WeaponAttackState.AttackingOne:
        //         break;
        //     case WeaponAttackState.AttackingTwo:
        //         break;
        //     case WeaponAttackState.StoppedAttacking:
        //         break;
        //     case WeaponAttackState.StoppedAttackingTwo:
        //         break;
        //     case WeaponAttackState.AttackingProjectile:
        //         break;
        //     case WeaponAttackState.StoppedAttackingProjectile:
        //         break;
        //     case WeaponAttackState.AttackingSpecial:
        //         break;
        //     default:
        //         throw new ArgumentOutOfRangeException();
        // }
        
    }

    private void OnStopAttackState(StopAttackStateEvent updateEvent)
    {
        // StateManager.SetState(gameObject, "Use", false);
        // StateManager.SetState(gameObject, "MagicProjectile", false);
    }
    
    private void OnDestroy()
    {
        EventBus.Unsubscribe<WeaponAttackingEvent>(OnAttackStateChanged);
        EventBus.Unsubscribe<StopAttackStateEvent>(OnStopAttackState);
    }
    
    
    private void Start()
    {
        _characterLocomotion = gameObject.GetComponent<UltimateCharacterLocomotion>();
        _player = ReInput.players.GetPlayer(0);
        _rotateTowards = _characterLocomotion.GetAbility<RotateTowards>();
        
        foreach (var state in _characterLocomotion.States)
        {
            if (state.Name == "MagicProjectile")
            {
                _magicProjectileState = state;
            }
            
            if (state.Name == "Use")
            {
                _useState = state;
            }
        }

        UpdateEnemies();
    }

    private void Update()
    {
        UpdateEnemies();

        var closestEnemy = GetClosestEnemy();
        
        _currentTarget = closestEnemy;

        if (faceTarget)
        {
            if (_magicProjectileState.Active && _currentTarget != null) // || _useState.Active && _currentTarget != null)
            {
                _characterLocomotion.TryStartAbility(_rotateTowards);
                _rotateTowards.Target = _currentTarget;
                return;
            }
            
            _rotateTowards.Target = null;
        }
    }

    
    private Transform GetClosestEnemy()
    {
        _currentTarget = null;
        var sortedByDistance = _activeEnemies.OrderBy(e => e.Key);
        
        foreach (var keyValuePair in sortedByDistance)
        {
            return keyValuePair.Value;
        }

        return null;
    }
    
    private void UpdateEnemies()
    {
        _currentTarget = null;
        
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        _activeEnemies = new Dictionary<float,Transform>();
        
        foreach (var enemy in enemies)
        {
            if (!enemy.activeInHierarchy)
            {
                continue;
            }

            var dis = Vector3.Distance(transform.position, enemy.transform.position);

            if (dis > searchRange)
            {
                continue;
            }
            
            _activeEnemies.Add(dis, enemy.transform);
        }
    }
}
