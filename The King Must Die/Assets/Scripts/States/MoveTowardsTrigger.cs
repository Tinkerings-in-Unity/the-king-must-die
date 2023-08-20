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

    private void OnAbilityActive(Ability ability, bool active)
    {
        if (ability is MoveTowards && !active)
        {
            ChangeState(_stateGameObject, false);
            gameObject.SetActive(false);
        }
    }

}