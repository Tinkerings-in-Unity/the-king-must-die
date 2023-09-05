using System.Collections;
using System.Collections.Generic;
using Opsive.Shared.StateSystem;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using UnityEngine;
using Utility.Events;

public class AnimatorIdleBehaviour : StateMachineBehaviour
{
    
    private GameObject _character;
    private WeaponController _weaponController;
    private UltimateCharacterLocomotion _characterLocomotion;
    private ItemAbility _toggleEquipAbility;

    private void Awake()
    {
    }
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_character == null)
        {
            _character = GameObject.FindGameObjectWithTag("Player");
            
            _characterLocomotion = _character.GetComponent<UltimateCharacterLocomotion>();

            var itemAbilities = _characterLocomotion.ItemAbilities;
        
            foreach (var itemAbility in itemAbilities)
            {
                if (itemAbility.State == "MainEquip")
                {
                    _toggleEquipAbility = itemAbility;
                }
            
            }
        }
        
        var weaponAttackingEvent = new WeaponAttackingEvent();
        weaponAttackingEvent.Set(WeaponAttackState.StoppedAttacking);
        EventBus.Publish(weaponAttackingEvent);
        
        var weaponAttackingEvent2 = new WeaponAttackingEvent();
        weaponAttackingEvent2.Set(WeaponAttackState.StoppedAttackingTwo);
        EventBus.Publish(weaponAttackingEvent2);
        
        var weaponAttackingEvent3 = new WeaponAttackingEvent();
        weaponAttackingEvent3.Set(WeaponAttackState.StoppedAttackingProjectile);
        EventBus.Publish(weaponAttackingEvent3);

        StateManager.SetState(_character, "Use", false);
        StateManager.SetState(_character, "MagicProjectile", false);
        ToggleEquip();
        
    }
    
    private void ToggleEquip()
    {
        return;
        // _equipped = !_equipped;
        //
        // StateManager.SetState(character, "Aim", _equipped);
        
        _characterLocomotion.TryStartAbility(_toggleEquipAbility);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
