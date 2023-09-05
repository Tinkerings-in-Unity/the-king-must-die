using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.Events;

public class WeaponController : MonoBehaviour
{

    [SerializeField] private Animator animator;
    private static readonly int AttackingOne = Animator.StringToHash("attackingOne");
    private static readonly int AttackingTwo = Animator.StringToHash("attackingTwo");
    private static readonly int AttackingProjectile = Animator.StringToHash("attackingProjectile");
    private static readonly int AttackingSpecial = Animator.StringToHash("attackingSpecial");

    private void Awake()
    {
        EventBus.Subscribe<WeaponAttackingEvent>(OnAttackStateChanged);
    }

    private void OnAttackStateChanged(WeaponAttackingEvent updateEvent)
    {

        switch (updateEvent.First)
        {
            case WeaponAttackState.AttackingOne:
                animator.SetBool(AttackingOne, true);
                animator.SetBool(AttackingTwo, false);
                animator.SetBool(AttackingProjectile, false);
                break;
            case WeaponAttackState.AttackingTwo:
                animator.SetBool(AttackingTwo, true);
                animator.SetBool(AttackingOne, false);
                animator.SetBool(AttackingProjectile, false);
                break;
            case WeaponAttackState.StoppedAttacking:
                animator.SetBool(AttackingOne, false);
                animator.SetBool(AttackingTwo, false);
                animator.SetBool(AttackingProjectile, false);
                animator.SetBool(AttackingSpecial, false);
                break;
            case WeaponAttackState.StoppedAttackingTwo:
                animator.SetBool(AttackingTwo, false);
                break;
            case WeaponAttackState.AttackingProjectile:
                animator.SetBool(AttackingProjectile, true);
                break;
            case WeaponAttackState.StoppedAttackingProjectile:
                animator.SetBool(AttackingProjectile, false);
                break;
            case WeaponAttackState.AttackingSpecial:
                animator.SetBool(AttackingSpecial, true);
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
