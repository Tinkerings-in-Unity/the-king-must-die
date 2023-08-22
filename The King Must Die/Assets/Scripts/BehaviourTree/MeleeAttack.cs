using NodeCanvas.Framework;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions {

	public class MeleeAttack : ActionTask {

		[RequiredField]
		public BBParameter<int> meleeActionID;
		
		private UltimateCharacterLocomotion _characterLocomotion;
		private ItemAbility _useAbility;
		
		//Use for initialization. This is called only once in the lifetime of the task.
		//Return null if init was successfull. Return an error string otherwise
		protected override string OnInit() {
			_characterLocomotion = agent.GetComponent<UltimateCharacterLocomotion>();
			_useAbility = _characterLocomotion.GetAbility<Use>();
			return null;
		}

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute() {
			if (_characterLocomotion.TryStartAbility(_useAbility))
			{
				EndAction(true);
			}
			else
			{
				EndAction(false);
			}
		}

		//Called once per frame while the action is active.
		protected override void OnUpdate() {
			
		}

		//Called when the task is disabled.
		protected override void OnStop() {
			
		}

		//Called when the task is paused.
		protected override void OnPause() {
			
		}
	}
}