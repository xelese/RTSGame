using System;
using UnityEngine;
using UnityEngine.AI;

namespace Script.Player {
    public class ClickToMove : MonoBehaviour {
        // navigation assistant.
        private NavMeshAgent m_NavMeshAgent;

        // agent speed
        public float speed = 8f;

        // only move if selected.
        public bool isSelected;

        // Start is called before the first frame update
        private void Start() {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        private void Update() {
            if (UnityEngine.Camera.main != null) {
                if (!isSelected) return;
                // get the vector of mouse position.
                var ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

                // if right click is pressed.
                if (!Input.GetMouseButtonUp(1)) return;
                // position to transform to.
                if (!Physics.Raycast(ray, out var hit, 1000)) return;
                ChangeDirection();
                MovePawn(hit);
            }
            else {
                throw new Exception("No main camera");
            }
        }

        private void MovePawn(RaycastHit hit) {
            m_NavMeshAgent.destination = hit.point;
            m_NavMeshAgent.speed = speed;
        }

        private void ChangeDirection() {
            if (m_NavMeshAgent.velocity.sqrMagnitude > Mathf.Epsilon) {
                transform.rotation = Quaternion.LookRotation(m_NavMeshAgent.velocity.normalized);
            }
        }
    }
}