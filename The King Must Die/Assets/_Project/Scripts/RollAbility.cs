using System;
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

public class RollAbility : MonoBehaviour
{
    [SerializeField] private float endRollDelay = 1f;
    [SerializeField] private float canRollAgainDelay = 1f;
    [SerializeField] private AudioClip bleepAudioClip;
    [SerializeField] private MMFeedbacks bleepStartFeedback;
    [SerializeField] private MMFeedbacks bleepEndFeedback;
    
    
    private AudioSource _audioSource;
    private UltimateCharacterLocomotion _characterLocomotion;
    private Health _health;
    private bool _canRoll = true;
    private bool _rollActivated;
    private Player _player;
    
    public void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    
    private void Start()
    {
        _characterLocomotion = gameObject.GetComponent<UltimateCharacterLocomotion>();
        _health = gameObject.GetComponent<Health>();
        _player = ReInput.players.GetPlayer(0);
    }

    private void Update()
    {
        if (_player.GetButtonDown("Jump") && _canRoll)
        {
            Roll();
            SchedulerBase.Schedule(endRollDelay, EndOfRoll);
        }
    }

    
    private void Roll()
    {
        EventHandler.ExecuteEvent(gameObject, "OnEnableGameplayInput", false);
        var canPlayAudio = !(_audioSource == null || bleepAudioClip != null);

        _canRoll = false;
        
        StateManager.SetState(gameObject, "Roll", true);
        
        bleepStartFeedback?.PlayFeedbacks();
        
        if (canPlayAudio)
        {
            _audioSource.clip = bleepAudioClip;
            _audioSource.Play();
        }
               
        _rollActivated = true;
        _health.Invincible = true;
        _characterLocomotion.TryStartAbility(_characterLocomotion.GetAbility<Generic>());
    }
    
    
    private void EndOfRoll()
    {
        StateManager.SetState(gameObject, "Roll", false);

        if (_audioSource != null && bleepAudioClip != null)
        {
            _audioSource.clip = bleepAudioClip;
            _audioSource.Play();
        }
        
        bleepEndFeedback?.PlayFeedbacks();
        
        _health.Invincible = false;
        
        _characterLocomotion.TryStopAbility(_characterLocomotion.GetAbility<Generic>());
        
        EventHandler.ExecuteEvent(gameObject, "OnEnableGameplayInput", true);
        
        SchedulerBase.Schedule(canRollAgainDelay, ResetCanRoll);
    }
    
    private void ResetCanRoll()
    {
        _canRoll = true;
    }
}
