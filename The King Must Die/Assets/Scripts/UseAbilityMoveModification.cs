using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Opsive.Shared.Events;
using Opsive.Shared.Game;
using Opsive.Shared.Input;
using Opsive.Shared.StateSystem;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using Opsive.UltimateCharacterController.Items.Actions;
using Opsive.UltimateCharacterController.Items.Actions.Modules;
using Rewired;
using Unity.Collections;

public class UseAbilityMoveModification :  StateMachineBehaviour // MonoBehaviour
{
    // private const string InputName = "Fire1";
    
    // [SerializeField] protected GameObject character;
    // [SerializeField] private float longPressDuration;
    // [SerializeField] private bool waitForLongPressRelease;
    // [SerializeField] private float changeStateDelay = 1f;
    // [SerializeField] private float inputPollDelay = 0.5f;
    // [SerializeField] private List<Animation> animations = new List<Animation>();
    // private UltimateCharacterLocomotion _characterLocomotion;
    // private ItemAbility _toggleEquipAbility;
    // private ItemAbility _useAbility;
    // private ScheduledEventBase _scheduledEvent;
    // private Player _playerInput;
    // private bool _equipped = true;
    // private bool _useAbilityActive;
    // private MeleeAction action;
    
    // public void Awake()
    // {
    //     _playerInput = ReInput.players.GetPlayer(0);
    //     // var triggerModule = action.TriggerActionModuleGroup.Modules[0] as RepeatCombo;
    //     // triggerModule.UseAnimatorAudioStateSet.States[0].
    // }
    

    // private void Start()
    // {
    //     EventHandler.RegisterEvent<ItemAbility, bool>(character, "OnCharacterItemAbilityActive", OnAbilityActive);
    //
    //     _characterLocomotion = character.GetComponent<UltimateCharacterLocomotion>();
    //     
    //     var itemAbilities = _characterLocomotion.ItemAbilities;
    //     
    //     foreach (var itemAbility in itemAbilities)
    //     {
    //         if (itemAbility.State == "MainEquip")
    //         {
    //             _toggleEquipAbility = itemAbility;
    //         }
    //         
    //         if (itemAbility.SlotID == 0)
    //         {
    //             _useAbility = itemAbility;
    //         }
    //     }
    //     //
    //     // ToggleEquip();
    // }

    // private void OnAbilityActive(ItemAbility ability, bool isActive)
    // {
    //     if (ability.SlotID != _useAbility.SlotID)
    //     {
    //         return;
    //     }
    //
    //     _useAbilityActive = isActive;
    //     
    //     if(_useAbilityActive)
    //     {
    //         StateManager.SetState(character, "Use", true);
    //         if (_scheduledEvent != null &&  _scheduledEvent.Active)
    //         {
    //             SchedulerBase.Cancel(_scheduledEvent);
    //         }
    //         
    //         StartCoroutine(WaitForAnimation());
    //     }
    //     else
    //     {
    //         if (_scheduledEvent != null &&  _scheduledEvent.Active)
    //         {
    //             SchedulerBase.Cancel(_scheduledEvent);
    //         }
    //         _scheduledEvent = SchedulerBase.Schedule(changeStateDelay, StopUseState);
    //     }
    // }
    
    // private IEnumerator WaitForAnimation ()
    // {
    //     Animation anim = null;
    //     foreach (var animation1 in animations)
    //     {
    //         if(!animation1.isPlaying)
    //         {
    //             continue;
    //         }
    //
    //         anim = animation1;
    //     }
    //     
    //     while (anim != null && anim.isPlaying)
    //     {
    //         yield return null;
    //     }
    //     
    //     Debug.Log("Animation completed");
    // }
    
     public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"Anim stat exited");
        // if(stateInfo.IsName("attack")) return;
        // pc_atttacking = false;
        // pc_anim.SetBool("attack", false);
    }
    
    // private void StopUseState()
    // {
    //     StateManager.SetState(character, "Use", false);
    // }

    // public void OnDestroy()
    // {
    //     // EventHandler.UnregisterEvent<ItemAbility, bool>(character, "OnCharacterItemAbilityActive", OnAbilityActive);
    // }
}
