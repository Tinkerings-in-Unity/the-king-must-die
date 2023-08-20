using NodeCanvas.Framework;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities.AI;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;


namespace NodeCanvas.Tasks.Conditions {

	public class CustomCheckDistanceToObject : ConditionTask<Transform>
	{

		[RequiredField]
		public BBParameter<GameObject> _player;
		public CompareMethod checkType = CompareMethod.LessThan;
		public BBParameter<float> distance = 10;

		[SliderField(0, 0.1f)]
		public float floatingPoint = 0.05f;
		
		private NavMeshAgent _navMeshAgent;
		
		protected override string OnInit()
		{
			_navMeshAgent = blackboard.GetVariable<NavMeshAgent>("myNavMeshAgent").value;
			
			return null;
		}

		protected override string info {
			get { return "Distance" + OperationTools.GetCompareString(checkType) + distance + " to " + _player; }
		}

		protected override bool OnCheck()
		{
			var value = OperationTools.Compare(Vector3.Distance(agent.position, _player.value.transform.position),
				distance.value, checkType, floatingPoint);
			
			
			if(value)
			{
				_navMeshAgent.ResetPath();
			}
			
			return value;
		}

		public override void OnDrawGizmosSelected() {
			if ( agent != null ) {
				Gizmos.DrawWireSphere(agent.position, distance.value);
			}
		}
	}
}