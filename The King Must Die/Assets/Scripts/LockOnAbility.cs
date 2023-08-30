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
        
        //get active enemies
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

    private void Update()
    {
        UpdateEnemies();

        var closestEnemy = GetClosestEnemy();
        
        _currentTarget = closestEnemy;

        if (faceTarget)
        {
            if (_magicProjectileState.Active || _useState.Active)
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
        
        foreach (var enemy in enemies)
        {
            var dis = Vector3.Distance(transform.position, enemy.transform.position);
            
            if (_activeEnemies.ContainsValue(enemy.transform))
            {
                if (!enemy.activeInHierarchy || dis > searchRange)
                {
                    foreach (var keyValuePair in _activeEnemies)
                    {
                        if (keyValuePair.Value != enemy.transform)
                        {
                            continue;
                        }

                        _activeEnemies.Remove(keyValuePair.Key);
                        break;
                    }
                }
            
                continue;
            }
            
            if (!enemy.activeSelf)
            {
                continue;
            }
            
            

            if (dis > searchRange)
            {
                continue;
            }
            
            _activeEnemies.Add(dis, enemy.transform);
        }
    }
}