using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        public NavMeshAgent agent { get; private set; } // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // the character we are controlling
        public Transform target; // target to aim for

        public event Action<GameObject> OnPositionReached;

        // Use this for initialization
        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

	        agent.updateRotation = false;
	        agent.updatePosition = true;
        }


        // Update is called once per frame
        private void Update()
        {
            if (target != null)
            {
                agent.SetDestination(target.position);

                // use the values to move the character
                character.Move(agent.desiredVelocity, false, false);

                // check position reached
                if(IsWithinBoundaries(character.transform.position,target.position,.5f))
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
                character.Move(Vector3.zero, false, false);
            }

        }

        private bool IsWithinBoundaries(Vector3 obj, Vector3 target, float boundarySize)
        {
            return Vector3.Distance(obj, target) <= boundarySize;
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}
