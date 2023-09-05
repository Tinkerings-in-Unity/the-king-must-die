using System.Collections;
using System.Collections.Generic;
using Opsive.Shared.StateSystem;
using UnityEngine;
using Utility.Events;

public class AnimatorMagicProjectileBehaviour : StateMachineBehaviour
{
    private GameObject _character;

    private void Awake()
    {
        _character = GameObject.FindGameObjectWithTag("Player");
    }
    
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var weaponAttackingEvent = new WeaponAttackingEvent();
        weaponAttackingEvent.Set(WeaponAttackState.AttackingProjectile);
        EventBus.Publish(weaponAttackingEvent);
        StateManager.SetState(_character, "MagicProjectile", true);
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var weaponAttackingEvent = new WeaponAttackingEvent();
        weaponAttackingEvent.Set(WeaponAttackState.StoppedAttackingProjectile);
        EventBus.Publish(weaponAttackingEvent);
        StateManager.SetState(_character, "MagicProjectile", false);
    }

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}
}
