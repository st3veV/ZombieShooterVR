using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        public NavMeshAgent Agent { get; private set; } // the navmesh agent required for the path finding
        public ThirdPersonCharacter Character { get; private set; } // the character we are controlling
        public Transform Target; // target to aim for

        public event Action<GameObject> OnPositionReached;

        // Use this for initialization
        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            Agent = GetComponentInChildren<NavMeshAgent>();
            Character = GetComponent<ThirdPersonCharacter>();

	        Agent.updateRotation = false;
	        Agent.updatePosition = true;
        }


        // Update is called once per frame
        private void Update()
        {
            if (Target != null)
            {
                Agent.SetDestination(Target.position);

                // use the values to move the character
                Character.Move(Agent.desiredVelocity, false, false);

                // check position reached
                if(IsWithinBoundaries(Character.transform.position,Target.position,1f))
                {
                    if (OnPositionReached != null)
                    {
                        OnPositionReached(gameObject);
                    }
                }
            }
            else
            {
                // We still need to call the character's move function, but we send zeroed input as the move param.
                Character.Move(Vector3.zero, false, false);
            }

        }

        private bool IsWithinBoundaries(Vector3 obj, Vector3 target, float boundarySize)
        {
            return Vector3.Distance(obj, target) <= boundarySize;
        }

        public void SetTarget(Transform target)
        {
            this.Target = target;
        }
    }
}
