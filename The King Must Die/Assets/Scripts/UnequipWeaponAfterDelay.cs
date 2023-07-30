using System.Collections;
using System.Linq;
using UnityEngine;
using Opsive.Shared.Events;
using Opsive.Shared.Game;
using Opsive.Shared.Input;
using Opsive.Shared.StateSystem;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using Unity.Collections;

public class UnequipWeaponAfterDelay : MonoBehaviour
{
    private const string InputName = "Fire1";
    
    [SerializeField] protected GameObject character;
    [SerializeField] private float longPressDuration;
    [SerializeField] private bool waitForLongPressRelease;
    [SerializeField] private float unequipDelay = 1f;
    [SerializeField] private float inputPollDelay = 0.5f;
    private UltimateCharacterLocomotion _characterLocomotion;
    private ItemAbility _toggleEquipAbility;
    private ItemAbility _useAbility;
    private ScheduledEventBase _scheduledEvent;
    private IPlayerInput _playerInput;
    private bool _equipped = true;
    private bool _useAbilityActive;
    
    public void Awake()
    {
        EventHandler.RegisterEvent<ItemAbility, bool>(gameObject, "OnCharacterItemAbilityActive", OnAbilityActive);
        _playerInput = character.GetComponentInChildren<IPlayerInput>();
    }
    
    
    

    private void Start()
    {
        _characterLocomotion = character.GetComponent<UltimateCharacterLocomotion>();

        var itemAbilities = _characterLocomotion.ItemAbilities;
        
        foreach (var itemAbility in itemAbilities)
        {
            if (itemAbility.State == "MainEquip")
            {
                _toggleEquipAbility = itemAbility;
            }
            
            if (itemAbility.State == "Use")
            {
                _useAbility = itemAbility;
            }
        }
        
        ToggleEquip();
    }

    private void OnAbilityActive(ItemAbility ability, bool isActive)
    {
        if (ability != _useAbility)
        {
            return;
        }

        if (_scheduledEvent != null &&  _scheduledEvent.Active && isActive)
        {
            // StateManager.SetState(character, "Aim", true);
            // _characterLocomotion.TryStartAbility(_aimAbility);
            SchedulerBase.Cancel(_scheduledEvent);
        }
        
        _useAbilityActive = isActive;
    }
    
    private void Update()
    {

        if (_useAbilityActive)
        {
            return;
        }
        
        var isUsingWeapon = _playerInput.GetButtonDown(InputName) || _playerInput.GetDoublePress(InputName) ||
                            _playerInput.GetLongPress(InputName, longPressDuration, waitForLongPressRelease);
        
        if (isUsingWeapon)
        {
            if (!_equipped)
            {
                if (_scheduledEvent != null &&  _scheduledEvent.Active)
                {
                    SchedulerBase.Cancel(_scheduledEvent);
                }
                
                ToggleEquip();
            }
        }
        else
        {
            var isMoving = Mathf.Abs(_playerInput.GetAxisRaw("Horizontal")) > 0 ||
                           Mathf.Abs(_playerInput.GetAxisRaw("Vertical")) > 0;
                
            if(isMoving)
            {
                if (_equipped)
                {
                    if (_scheduledEvent != null &&  _scheduledEvent.Active)
                    {
                        return;
                    }

                    // ToggleEquip();
                    
                    _scheduledEvent = SchedulerBase.Schedule(unequipDelay, ToggleEquip);
                }
            }
        }
    }


    
    private void ToggleEquip()
    {
        
        _equipped = !_equipped;
        
        StateManager.SetState(character, "Aim", _equipped);
        
        _characterLocomotion.TryStartAbility(_toggleEquipAbility);
    }

    public void OnDestroy()
    {
        EventHandler.UnregisterEvent<ItemAbility, bool>(gameObject, "OnCharacterItemAbilityActive", OnAbilityActive);
    }
}
