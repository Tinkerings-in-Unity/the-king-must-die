using System;
using System.Collections.Generic;
using Opsive.Shared.Game;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using Rewired;
using UnityEngine;

public class SkillsInventory : MonoBehaviour
{
    [SerializeField] private List<int> preloadedBasicSkills = new List<int>();
    [SerializeField] private List<int> preloadedSpecialSkills = new List<int>();
    [SerializeField] private int skillTypeCount;
    [SerializeField] private float resetDelay = 1f;

    private ItemAbility _useBasicSkillAbility;
    private ItemAbility _useSpecialSkillAbility;
    private List<int> _basicSkills = new List<int>();
    private List<int> _specialSkills = new List<int>();
    private UltimateCharacterLocomotion _characterLocomotion;
    private Player _player;
    private int _basicSkillIndex = 0;
    private int _specialSkillIndex = 0;
    private int _skillTypeIndex = 0;
    private ScheduledEventBase _scheduledEvent;

    private void Awake()
    {
        //load indices
        foreach (var preloadedBasicSkill in preloadedBasicSkills)
        {
            _basicSkills.Add(preloadedBasicSkill);
        }
        
        //load acquired basic skills

        foreach (var preloadedSpecialSkill in preloadedSpecialSkills)
        {
            _specialSkills.Add(preloadedSpecialSkill);
        }
        
        //load acquired special skills
    }

    private void Start()
    {
        _characterLocomotion = gameObject.GetComponent<UltimateCharacterLocomotion>();
        _player = ReInput.players.GetPlayer(0);
        
        foreach (var itemAbility in _characterLocomotion.ItemAbilities)
        {
            switch (itemAbility.State)
            {
                case "Weapon Basic Skill":
                    _useBasicSkillAbility = itemAbility;
                    break;
                case "Weapon Special Skill":
                    _useSpecialSkillAbility = itemAbility;
                    break;
            }
        }

        _useBasicSkillAbility.ActionID = _basicSkillIndex;
        _useSpecialSkillAbility.ActionID = _specialSkillIndex;
    }

    public void AddBasicSkill(int skill)
    {
        _basicSkills.Add(skill);
        Debug.Log("Basic skill added");
    }

    public void AddSpecialSkill(int skill)
    {
        _specialSkills.Add(skill);
        Debug.Log("Special skill added");
    }

    private void Update()
    {
        if (_player.GetButtonDown("Skill Type Up"))
        {
            if (_scheduledEvent != null &&  _scheduledEvent.Active)
            {
                SchedulerBase.Cancel(_scheduledEvent);
            }
            
            _skillTypeIndex--;
            
            if(_skillTypeIndex < 1)
            {
                _skillTypeIndex = skillTypeCount;
                
                _scheduledEvent = SchedulerBase.Schedule(resetDelay, Reset);
            }
            
        }
        
        if (_player.GetButtonDown("Skill Type Down"))
        {
            if (_scheduledEvent != null &&  _scheduledEvent.Active)
            {
                SchedulerBase.Cancel(_scheduledEvent);
            }
            
            _skillTypeIndex++;
            
            if(_skillTypeIndex > skillTypeCount)
            {
                _skillTypeIndex = 1;
                
                _scheduledEvent = SchedulerBase.Schedule(resetDelay, Reset);
            }
        }
        
        if (_player.GetButtonDown("Equip Next"))
        {
            if (_scheduledEvent != null &&  _scheduledEvent.Active)
            {
                SchedulerBase.Cancel(_scheduledEvent);
            }

            Debug.Log("Skill type " + _skillTypeIndex);

            if (_skillTypeIndex == 1)
            {
                _basicSkillIndex++;
                
                if(_basicSkillIndex > _basicSkills.Count - 1)
                {
                    _basicSkillIndex = 0;
                }
                
                _useBasicSkillAbility.ActionID = _basicSkills[_basicSkillIndex];
                    
                _scheduledEvent = SchedulerBase.Schedule(resetDelay, Reset);
            }
            else if (_skillTypeIndex == 2)
            {
                _specialSkillIndex ++;
                
                if(_specialSkillIndex > _specialSkills.Count - 1)
                {
                    _specialSkillIndex = 0;
                }
                
                _useSpecialSkillAbility.ActionID = _specialSkills[_specialSkillIndex];
                
                _scheduledEvent = SchedulerBase.Schedule(resetDelay, Reset);
            }
        }
        
        if (_player.GetButtonDown("Equip Prev"))
        {
            if (_scheduledEvent != null &&  _scheduledEvent.Active)
            {
                SchedulerBase.Cancel(_scheduledEvent);
            }

            Debug.Log("Skill type " + _skillTypeIndex);

            if (_skillTypeIndex == 1)
            {
                _basicSkillIndex--;
                
                if(_basicSkillIndex < 0)
                {
                    _basicSkillIndex = _basicSkills.Count - 1;
                }
                
                _useBasicSkillAbility.ActionID = _basicSkills[_basicSkillIndex];
                    
                _scheduledEvent = SchedulerBase.Schedule(resetDelay, Reset);
            }
            else if (_skillTypeIndex == 2)
            {
                _specialSkillIndex --;
                
                if(_specialSkillIndex < 0)
                {
                    _specialSkillIndex = _specialSkills.Count - 1;
                }
                
                _useSpecialSkillAbility.ActionID = _specialSkills[_specialSkillIndex];
                
                _scheduledEvent = SchedulerBase.Schedule(resetDelay, Reset);
            }
        }
    }
    
    private void Reset()
    {
        _skillTypeIndex = 0;
    }
}