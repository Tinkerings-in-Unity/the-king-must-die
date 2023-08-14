using System;
using Opsive.Shared.Audio;
using Opsive.Shared.Game;
using Opsive.Shared.StateSystem;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Traits;
using EventHandler = Opsive.Shared.Events.EventHandler;

public class MoveTowardsTrigger : StateTriggerBase
{

    [SerializeField] private Transform moveTowardsDestination;
    [SerializeField] private Transform moveTowardsStart;
    
    private GameObject _stateGameObject;
    
    protected override void ChangeState(GameObject stateGameObject, bool activate)
    {
        base.ChangeState(stateGameObject, activate);

        _stateGameObject = stateGameObject;

        if (activate)
        {
            EventHandler.RegisterEvent<Ability, bool>(stateGameObject, "OnCharacterAbilityActive", OnAbilityActive);

            EventHandler.ExecuteEvent(gameObject, "OnEnableGameplayInput", false);

            var characterLocomotion = stateGameObject.GetComponent<UltimateCharacterLocomotion>();
            characterLocomotion.SetPositionAndRotation(moveTowardsStart.position, moveTowardsStart.rotation, true);
            
            characterLocomotion.MoveTowardsAbility.InputMultiplier = 0.7f;
            characterLocomotion.MoveTowardsAbility.MoveTowardsLocation(moveTowardsDestination.position);
        }
        else
        {
            EventHandler.UnregisterEvent<Ability, bool>(stateGameObject, "OnCharacterAbilityActive", OnAbilityActive);

            EventHandler.ExecuteEvent(gameObject, "OnEnableGameplayInput", true);
            
            TriggerExit(stateGameObject);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
    }


    // protected override void TriggerExit(GameObject other)
    // {
    //     StateBehavior stateBehavior;
    //     if ((m_RequireCharacter && (stateBehavior = other.GetComponentInParent<UltimateCharacterLocomotion>()) != null) ||
    //         (!m_RequireCharacter && (stateBehavior = other.GetComponentInParent<StateBehavior>()) != null))
    //     {
    //         if (m_ActivateStateEvent != null && m_ActivateStateEvent.Active)
    //         {
    //             Scheduler.Cancel(m_ActivateStateEvent);
    //             m_ActivateStateEvent = null;
    //         }
    //         else
    //         {
    //             // The state shouldn't change when the object dies. It can be changed when the character respawns.
    //             var health = other.gameObject.GetCachedParentComponent<Health>();
    //             if (health != null && !health.IsAlive())
    //             {
    //                 // When the character respawns the trigger enter/exit event may not fire. Register that the state should be deactivated so when the 
    //                 // character respawns the state can then be disabled.
    //                 if (m_DeathDeactivations == null)
    //                 {
    //                     m_DeathDeactivations = new List<GameObject>();
    //                 }
    //
    //                 if (!m_DeathDeactivations.Contains(stateBehavior.gameObject))
    //                 {
    //                     m_DeathDeactivations.Add(stateBehavior.gameObject);
    //                     EventHandler.RegisterEvent(stateBehavior.gameObject, "OnRespawn", OnRespawn);
    //                 }
    //
    //                 return;
    //             }
    //
    //             StateManager.SetState(stateBehavior.gameObject, m_StateName, false);
    //             if (m_CharacterTransformChange)
    //             {
    //                 EventHandler.ExecuteEvent(stateBehavior.gameObject, "OnCharacterImmediateTransformChange", true);
    //             }
    //         }
    //     }
    // }

    private void OnAbilityActive(Ability ability, bool active)
    {
        if (ability is MoveTowards && !active)
        {
            Debug.Log("Move towards ability ended");
            ChangeState(_stateGameObject, false);
        }
    }

}