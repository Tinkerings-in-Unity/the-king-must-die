using System;
using System.Linq;
using MoreMountains.Feedbacks;
using UnityEngine;
using Opsive.Shared.Game;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.Shared.Events;
using Opsive.Shared.StateSystem;
using Opsive.UltimateCharacterController.Items;
using Opsive.UltimateCharacterController.Items.Actions;
using Opsive.UltimateCharacterController.Items.Actions.Modules.Magic;
using Opsive.UltimateCharacterController.Traits;
using Rewired;
using EventHandler = Opsive.Shared.Events.EventHandler;

public class SpearProjectilesSpawnPointSetter : MonoBehaviour
{
    public void Awake()
    {
    }
    
    
    private void Start()
    {
        var itemSlots = gameObject.GetComponentsInChildren<CharacterItemSlot>();

        var spear = itemSlots.First(i => i.ID == 0).transform.GetChild(0);
        var magicProjectile = itemSlots.First(i => i.ID == 1).transform.GetChild(0); 
        
        Transform castOrigin;
        Transform spearProjectileSpawnPoint = null;

        for (var a = 0; a < spear.childCount; a++)
        {
            var child = spear.GetChild(a);
            if(!child.CompareTag("MagicProjectileSource"))
            {
                continue;
            }

            spearProjectileSpawnPoint = child;
            break;
        }

        for(var i = 0; i < magicProjectile.childCount; i ++)
        {
            var child = magicProjectile.GetChild(i);
            if(child.name != "Origin")
            {
                continue;
            }

            // castOrigin = child;
            if (spearProjectileSpawnPoint != null)
            {
                child.SetParent(spearProjectileSpawnPoint);
                child.transform.localPosition = Vector3.zero;
                child.rotation = spearProjectileSpawnPoint.rotation;
            }
            break;
        }

    }
}
