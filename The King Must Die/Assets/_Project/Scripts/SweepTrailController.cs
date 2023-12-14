using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using Utility.Events;

public class SweepTrailController : MonoBehaviour
{
    [SerializeField] private TrailRenderer sweepTrailRenderer;

    [SerializeField] private SplineFollower sweepTrailSplineFollower;
    
    private void Awake()
    {
        EventBus.Subscribe<WeaponAttackingEvent>(OnAttackStateChanged);
    }

    private void OnAttackStateChanged(WeaponAttackingEvent updateEvent)
    {

        switch (updateEvent.First)
        {
            case WeaponAttackState.AttackingOne:
                break;
            case WeaponAttackState.AttackingTwo:
                break;
            case WeaponAttackState.StoppedAttacking:
                break;
            case WeaponAttackState.StoppedAttackingTwo:
                break;
            case WeaponAttackState.AttackingProjectile:
                break;
            case WeaponAttackState.StoppedAttackingProjectile:
                break;
            case WeaponAttackState.AttackingSpecial:
                break;
            case WeaponAttackState.SpawnSpecialAttackParticles:
                break;
            case WeaponAttackState.StartSpearSweepTrail:
                sweepTrailSplineFollower.Restart();
                sweepTrailSplineFollower.follow = true;
                sweepTrailRenderer.emitting = true;
                break;
            case WeaponAttackState.StopSpearSweepTrail:
                sweepTrailRenderer.emitting = false;
                sweepTrailSplineFollower.follow = false;
                break;
            case WeaponAttackState.StartSpearJabTrail:
                break;
            case WeaponAttackState.StopSpearJabTrail:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    } 
    
    private void OnDestroy()
    {
        EventBus.Unsubscribe<WeaponAttackingEvent>(OnAttackStateChanged);
    }
}
