using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities.AI;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;
using LookAt = Opsive.UltimateCharacterController.Character.Abilities.LookAt;


namespace NodeCanvas.Tasks.Actions {

	public class SeekPlayer : ActionTask
	{

		public float TargetDistance;
		private NavMeshAgent _navMeshAgent;
		
		private Transform _player;
		private UltimateCharacterLocomotion _characterLocomotion;
		private NavMeshAgentMovement _navMeshAgentMovement;
		
		
		//Use for initialization. This is called only once in the lifetime of the task.
		//Return null if init was successfull. Return an error string otherwise
		protected override string OnInit()
		{
			_player = GameObject.FindWithTag("Player").transform;
			_characterLocomotion = agent.GetComponent<UltimateCharacterLocomotion>();
			_navMeshAgentMovement = _characterLocomotion.GetAbility<NavMeshAgentMovement >();
			_navMeshAgent = blackboard.GetVariable<NavMeshAgent>("myNavMeshAgent").value;
			
			return null;
		}

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute() {
			if (_player == null)
			{
				_player = GameObject.FindWithTag("Player").transform;
				if (_player == null)
				{
					EndAction(false);
				}
			}

			if (_characterLocomotion == null)
			{
				_characterLocomotion = agent.GetComponent<UltimateCharacterLocomotion>();
				_navMeshAgentMovement = _characterLocomotion.GetAbility<NavMeshAgentMovement >();
				if (_characterLocomotion == null)
				{
					EndAction(false);
				}
			}

			_characterLocomotion.MoveTowardsAbility.InputMultiplier = 2f;
			// var randomPosition = Random.onUnitSphere * TargetDistance;
			// randomPosition.y = 0;

			var positions = new List<Vector3>();
			
			// for (var i = 0; i < 10; i++)
			// {
				var randomPosition = Random.onUnitSphere * TargetDistance;
				randomPosition.y = 0;

				var pos = _player.position + randomPosition;

			// 	if (CanReachPosition(pos))
			// 	{
			// 		positions.Add(pos);
			// 	}
			// }
			//
			// positions = positions.OrderBy((p) => (p - agent.transform.position).sqrMagnitude).ToList();
			//
			// if(positions.Count == 0)
			// {
			// 	EndAction(false);
			// }
			// else
			// {
			// 	var destination = positions[0];
			//
			// 	_navMeshAgent.SetDestination(destination);
			// }

			if (CanReachPosition(pos))
			{
				_navMeshAgent.SetDestination(pos);
			}
			else
			{
				EndAction(false);
			}

		}
		
		public bool CanReachPosition(Vector3 position)
		{
			var path = new NavMeshPath();
			_navMeshAgent.CalculatePath(position, path);
			return path.status == NavMeshPathStatus.PathComplete;
		}
		
		private Vector3 GetRandomPositionAroundPlayer()
		{
			return Vector3.zero;
		}

		//Called once per frame while the action is active.
		protected override void OnUpdate() {
			if (_navMeshAgentMovement.HasArrived)
			{
				_navMeshAgentMovement.StopAbility();
				// agent.transform.LookAt(_player);
			
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