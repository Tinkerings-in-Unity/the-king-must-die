using System;
using MoreMountains.Feedbacks;
using UnityEngine;
using Opsive.Shared.Game;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.Shared.Events;
using Rewired;
using EventHandler = Opsive.Shared.Events.EventHandler;

public class BleepAbility : MonoBehaviour
{
    [SerializeField] private GameObject startOfBleepParticlePrefab;
    [SerializeField] private GameObject endOfBleepParticlePrefab;
    [SerializeField] private float startParticleYPositionOffset = 0f;
    [SerializeField] private float endParticleYPositionOffset = 0f;
    [SerializeField] private float bleepDelay = 0.3f;
    [SerializeField] private float startParticleDestroyDelay = 1f;
    [SerializeField] private float endParticleDestroyDelay = 1f;
    [SerializeField] private BleepVerticalPositionChecker bleepVerticalChecker;
    [SerializeField] private BleepHorizontalPositionChecker bleepHorizontalChecker;
    [SerializeField] private float bleepForwardOffset;
    [SerializeField] private float bleepCheckersHeight;
    [SerializeField] private bool m_SnapAnimator;
    [SerializeField] private AudioClip bleepAudioClip;
    [SerializeField] private MMFeedbacks bleepStartFeedback;
    [SerializeField] private MMFeedbacks bleepEndFeedback;
    
    
    private Ability _dashAbility;
    private GameObject _instantiatedStartDashParticle;
    private GameObject _instantiatedEndDashParticle;
    private AudioSource _audioSource;
    private UltimateCharacterLocomotion _characterLocomotion;
    private bool _canDash = true;
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
            
            if (double.IsInfinity(positionToBleepTo.y))
            {
                return;
            }
            
            Dash(positionToBleepTo);
            SchedulerBase.Schedule(startParticleDestroyDelay, DestroyStartParticle);
        }
    }

    
    private void Dash(Vector3 positionToBleepTo)
    {
        EventHandler.ExecuteEvent(gameObject, "OnEnableGameplayInput", false);
        var canPlayAudio = !(_audioSource == null || bleepAudioClip != null);

        _canDash = false;

        var cachedTransform = _characterLocomotion.transform;

        var cachedRotation = cachedTransform.rotation;
        
        var positionToInstantiate = cachedTransform.position; 
        
        _instantiatedStartDashParticle = ObjectPoolBase.Instantiate(startOfBleepParticlePrefab, positionToInstantiate + 
            (Vector3.up * startParticleYPositionOffset),cachedRotation);
        
        bleepStartFeedback?.PlayFeedbacks();
        
        if (canPlayAudio)
        {
            _audioSource.clip = bleepAudioClip;
            _audioSource.Play();
        }
                
        _characterLocomotion.SetPositionAndRotation(positionToBleepTo, cachedRotation, m_SnapAnimator, true);
        
        _characterLocomotion.gameObject.SetActive(false);
        
        SchedulerBase.Schedule(bleepDelay, Bleep);
    }
    
    private void CheckHorizontal()
    {
       bleepVerticalChecker.SetHorizontalPosition( transform, bleepHorizontalChecker.GetHorizontalPositionToBleepTo());
    }
    
    private void DestroyStartParticle()
    {
        ObjectPoolBase.Destroy(_instantiatedStartDashParticle);
    }
    
    private void Bleep()
    {
        _instantiatedEndDashParticle = ObjectPoolBase.Instantiate(endOfBleepParticlePrefab, 
            _characterLocomotion.transform.position + (Vector3.up * endParticleYPositionOffset), 
            _characterLocomotion.transform.rotation);
        
        if (_audioSource != null && bleepAudioClip != null)
        {
            _audioSource.clip = bleepAudioClip;
            _audioSource.Play();
        }
        
        _characterLocomotion.gameObject.SetActive(true);
        
        bleepEndFeedback?.PlayFeedbacks();
        
        SchedulerBase.Schedule(endParticleDestroyDelay, DestroyEndParticle);
    }
    
    private void DestroyEndParticle()
    {
        EventHandler.ExecuteEvent(gameObject, "OnEnableGameplayInput", true);
        _canDash = true;
        ObjectPoolBase.Destroy(_instantiatedEndDashParticle);
    }

    public void OnDestroy()
    {
        // EventHandler.UnregisterEvent<Ability, bool>(gameObject, "OnCharacterAbilityActive", OnAbilityActive);
    }
}
