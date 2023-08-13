using System;
using MoreMountains.Feedbacks;
using UnityEngine;
using Opsive.Shared.Game;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.Shared.Events;
using Opsive.Shared.StateSystem;
using Rewired;
using EventHandler = Opsive.Shared.Events.EventHandler;

public class DashAbility : MonoBehaviour
{
    [SerializeField] protected Transform destination;
    [SerializeField] private GameObject startOfBleepParticlePrefab;
    [SerializeField] private GameObject endOfBleepParticlePrefab;
    [SerializeField] private float startParticleYPositionOffset = 0f;
    [SerializeField] private float endParticleYPositionOffset = 0f;
    [SerializeField] private float bleepDelay = 0.1f;
    [SerializeField] private float startParticleDestroyDelay = 1f;
    [SerializeField] private float endParticleDestroyDelay = 1f;
    [SerializeField] private BleepVerticalPositionChecker bleepVerticalChecker;
    [SerializeField] private BleepHorizontalPositionChecker bleepHorizontalChecker;
    [SerializeField] private float bleepForwardOffset;
    [SerializeField] private float bleepCheckersHeight;
    [SerializeField] private bool snapAnimator;
    [SerializeField] private bool stopAllAbilities;
    [SerializeField] private AudioClip bleepAudioClip;
    [SerializeField] private MMFeedbacks bleepStartFeedback;
    [SerializeField] private MMFeedbacks bleepEndFeedback;
    
    
    private Ability _dashAbility;
    private GameObject _instantiatedStartDashParticle;
    private GameObject _instantiatedEndDashParticle;
    private AudioSource _audioSource;
    private UltimateCharacterLocomotion _characterLocomotion;
    private bool _canDash = true;
    private bool _dashActivated;
    private Player _player;
    
    public void Awake()
    {
        // EventHandler.RegisterEvent<Ability, bool>(gameObject, "OnCharacterAbilityActive", OnAbilityActive);
        _audioSource = GetComponent<AudioSource>();
    }
    
    
    
    private void Start()
    {
        _characterLocomotion = gameObject.GetComponent<UltimateCharacterLocomotion>();
        bleepHorizontalChecker.Setup(transform ,bleepForwardOffset, bleepCheckersHeight);
        bleepVerticalChecker.Setup( transform ,bleepHorizontalChecker.GetHorizontalPositionToBleepTo(), bleepCheckersHeight);
        _player = ReInput.players.GetPlayer(0);
    }

    private void Update()
    {
        CheckHorizontal();

        if (_player.GetButtonDown("Jump") && _canDash)
        {
            var positionToBleepTo = bleepVerticalChecker.GetPositionToDashTo();
            
            if (double.IsInfinity(positionToBleepTo.y) || positionToBleepTo.y > 3.2f)
            {
                return;
            }
            
            Dash(positionToBleepTo);
            SchedulerBase.Schedule(startParticleDestroyDelay, DestroyStartParticle);
        }

        if (!_characterLocomotion.MoveTowardsAbility.IsActive && _dashActivated)
        {
            _dashActivated = false;
            Bleep();
            Debug.Log("Disable dash");
        }
    }

    
    private void Dash(Vector3 positionToBleepTo)
    {
        EventHandler.ExecuteEvent(gameObject, "OnEnableGameplayInput", false);
        var canPlayAudio = !(_audioSource == null || bleepAudioClip != null);

        _canDash = false;
        
        StateManager.SetState(gameObject, "Bleep", true);

        var cachedTransform = _characterLocomotion.transform;

        var cachedRotation = cachedTransform.rotation;
        
        var positionToInstantiate = cachedTransform.position; 
        
        // _instantiatedStartDashParticle = ObjectPoolBase.Instantiate(startOfBleepParticlePrefab, positionToInstantiate + 
        //     (Vector3.up * startParticleYPositionOffset),cachedRotation);
        
        bleepStartFeedback?.PlayFeedbacks();
        
        if (canPlayAudio)
        {
            _audioSource.clip = bleepAudioClip;
            _audioSource.Play();
        }
               
        

        _dashActivated = true;
        // _characterLocomotion.MoveTowardsAbility.MoveTowardsLocation(destination.position);
        _characterLocomotion.TryStartAbility(_characterLocomotion.GetAbility<Generic>());
        _characterLocomotion.MoveTowardsAbility.MoveTowardsLocation(positionToBleepTo);
        
        // _characterLocomotion.gameObject.SetActive(false);
        
    }
    
    private void CheckHorizontal()
    {
        // destination.position = new Vector3(destination.position.x, destination.position.x,
        //     bleepHorizontalChecker.GetHorizontalPositionToBleepTo());
       bleepVerticalChecker.SetHorizontalPosition( transform, bleepHorizontalChecker.GetHorizontalPositionToBleepTo());
    }
    
    private void DestroyStartParticle()
    {
        if (_instantiatedStartDashParticle == null)
        {
            return;
        }
        
        if (ObjectPoolBase.InstantiatedWithPool(_instantiatedStartDashParticle))
        {
            ObjectPoolBase.Destroy(_instantiatedStartDashParticle);
        }
        else
        {
            DestroyImmediate(_instantiatedStartDashParticle);
        }
    }
    
    private void Bleep()
    {
        StateManager.SetState(gameObject, "Bleep", false);

        // _instantiatedEndDashParticle = ObjectPoolBase.Instantiate(endOfBleepParticlePrefab, 
        //     _characterLocomotion.transform.position + (Vector3.up * endParticleYPositionOffset), 
        //     _characterLocomotion.transform.rotation);
        
        if (_audioSource != null && bleepAudioClip != null)
        {
            _audioSource.clip = bleepAudioClip;
            _audioSource.Play();
        }
        
        // _characterLocomotion.gameObject.SetActive(true);
        
        bleepEndFeedback?.PlayFeedbacks();
        
        _characterLocomotion.TryStopAbility(_characterLocomotion.GetAbility<Generic>());
        
        EventHandler.ExecuteEvent(gameObject, "OnEnableGameplayInput", true);
        // _canDash = true;
        
        SchedulerBase.Schedule(endParticleDestroyDelay, DestroyEndParticle);
    }
    
    private void DestroyEndParticle()
    {
        // EventHandler.ExecuteEvent(gameObject, "OnEnableGameplayInput", true);
        _canDash = true;

        if (_instantiatedEndDashParticle == null)
        {
            return;
        }
        
        if (ObjectPoolBase.InstantiatedWithPool(_instantiatedEndDashParticle))
        {
            ObjectPoolBase.Destroy(_instantiatedEndDashParticle);
        }
        else
        {
            DestroyImmediate(_instantiatedEndDashParticle);
        }
    }

    public void OnDestroy()
    {
        // EventHandler.UnregisterEvent<Ability, bool>(gameObject, "OnCharacterAbilityActive", OnAbilityActive);
    }
}
