using System;
using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateCharacterController.Character;
using UnityEngine;
using Utility.Events;

public class WeaponControllerPassThrough : MonoBehaviour
{
   [SerializeField] private CharacterIK characterIK;
   
   
   public void StoppedAttacking()
   {
      var weaponAttackingEvent = new WeaponAttackingEvent();
      weaponAttackingEvent.Set(WeaponAttackState.StoppedAttacking);
      EventBus.Publish(weaponAttackingEvent);
   }
   
   public void SpecialStarting()
   {
      var weaponAttackingEvent = new WeaponAttackingEvent();
      weaponAttackingEvent.Set(WeaponAttackState.AttackingSpecial);
      EventBus.Publish(weaponAttackingEvent);
   } 
   
   public void CanStopAttackState()
   {
      var stopAttackStateEvent = new StopAttackStateEvent();
      EventBus.Publish(stopAttackStateEvent);
   }
   
   public void SpawnSpecialAttackParticles()
   {
      var weaponAttackingEvent = new WeaponAttackingEvent();
      weaponAttackingEvent.Set(WeaponAttackState.SpawnSpecialAttackParticles);
      EventBus.Publish(weaponAttackingEvent);
   }
}
