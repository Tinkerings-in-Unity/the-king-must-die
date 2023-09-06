using System.Linq;
using UnityEngine;
using Opsive.UltimateCharacterController.Items;

public class SpearParticlesSpawnPointSetter : MonoBehaviour
{
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Transform specialParticlesSpawnPoint;
    
    
    private void Start()
    {
        var itemSlots = gameObject.GetComponentsInChildren<CharacterItemSlot>();

        var magicProjectile = itemSlots.First(i => i.ID == 1).transform.GetChild(0); 
        
        for(var i = 0; i < magicProjectile.childCount; i ++)
        {
            var child = magicProjectile.GetChild(i);
            if(child.name != "Origin")
            {
                continue;
            }

            if (projectileSpawnPoint != null)
            {
                child.SetParent(projectileSpawnPoint);
                child.transform.localPosition = Vector3.zero;
                child.rotation = projectileSpawnPoint.rotation;
            }
            break;
        }

        var items = gameObject.GetComponentInChildren<ItemPlacement>().transform;

        
        for (var i = 0; i < items.childCount; i++)
        {
            var child = items.GetChild(i);
            
            if (child.name != "SpearWeapon")
            {
                continue;
            }
            
            for(var a = 0; a < child.childCount; a ++)
            {
                var childOfChild = child.GetChild(a);
                
                if(childOfChild.name != "Origin")
                {
                    continue;
                }
        
                if (specialParticlesSpawnPoint != null)
                {
                    childOfChild.SetParent(specialParticlesSpawnPoint);
                    childOfChild.transform.localPosition = Vector3.zero;
                    childOfChild.rotation = projectileSpawnPoint.rotation;
                }
                break;
            }
        }

        

    }
}
