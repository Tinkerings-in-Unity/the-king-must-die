using System.Collections.Generic;
using NodeCanvas.Framework;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities.AI;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;


namespace NodeCanvas.Tasks.Actions {

	public class Wander : ActionTask {

		public float TargetDistance;
		
		private NavMeshAgent _navMeshAgent;
		private UltimateCharacterLocomotion _characterLocomotion;
		private NavMeshAgentMovement _navMeshAgentMovement;
		
		
		//Use for initialization. This is called only once in the lifetime of the task.
		//Return null if init was successfull. Return an error string otherwise
		protected override string OnInit()
		{
			_characterLocomotion = agent.GetComponent<UltimateCharacterLocomotion>();
			_navMeshAgentMovement = _characterLocomotion.GetAbility<NavMeshAgentMovement >();
			_navMeshAgent = blackboard.GetVariable<NavMeshAgent>("myNavMeshAgent").value;
			
			return null;
		}

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute() {
			if (_characterLocomotion == null)
			{
				_characterLocomotion = agent.GetComponent<UltimateCharacterLocomotion>();
				_navMeshAgentMovement = _characterLocomotion.GetAbility<NavMeshAgentMovement >();
				if (_characterLocomotion == null)
				{
					EndAction(false);
				}
			}

			_characterLocomotion.MoveTowardsAbility.InputMultiplier = 0.7f;
			_navMeshAgentMovement.ArrivedDistance = TargetDistance;

			var positions = new HashSet<Vector3>();
			
			for (var i = 0; i < 10; i++)
			{
				var randomPosition = Random.onUnitSphere * TargetDistance;
				randomPosition.y = 0;

				var pos = agent.transform.position + randomPosition;

				positions.Add(pos);
			}
			
			foreach (var position in positions)
			{
				if (CanReachPosition(position))
				{
					_navMeshAgent.SetDestination(position);
					break;
				}
			}
		}
		
		public bool CanReachPosition(Vector3 position)
		{
			var path = new NavMeshPath();
			_navMeshAgent.CalculatePath(position, path);
			return path.status == NavMeshPathStatus.PathComplete;
		}
		
		//Called once per frame while the action is active.
		protected override void OnUpdate() {
			if (_navMeshAgentMovement.HasArrived)
			{
				_navMeshAgentMovement.StopAbility();
			
				EndAction(true);
			}
			
		}

		//Called when the task is disabled.
		protected override void OnStop() {
			
		}

		//Called when the task is paused.
		protected override void OnPause() {
			
		}
	}
}