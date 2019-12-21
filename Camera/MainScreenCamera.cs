using System;
using UnityEngine;

namespace Script.Camera {
    public class MainScreenCamera : MonoBehaviour {
        // speed
        public float panSpeed = 10f;
        public float zoomSpeed = 4f;
        public float rotationSpeed = 10f;

        // border to scroll on.
        private const float ScreenBorder = 30f;

        // current position
        private float m_PosX;
        private float m_PosY;
        private float m_PosZ;

        // enable movement.
        public bool enableMovement = true;

        // enable zoom.
        public bool enableZoom = true;

        // enable combined key and mouse movement.
        public bool enableCombinedMovement = true;

        // enable rotation.
        public bool enableRotation = true;

        // camera
        public new UnityEngine.Camera camera;

        // zoom
        public float minZoom = 7;
        public float maxZoom = 21;

        // private variables;
        bool m_LockPositionOfRotationObj;
        private Vector3 m_Position;
        private Vector3 m_LookPos;
        private float m_LocalRotationSpeed;

        private void Awake() {
            m_LockPositionOfRotationObj = false;
            m_LocalRotationSpeed = 0f;
        }

        // Update is called once per frame
        private void Update() {
            // calculate movement.
            MoveDirection();

            // calculate zoom.
            Zoom();

            // rotation.
            Rotate();

            // apply move and zoom.
            Movement(m_PosX, m_PosY, m_PosZ);
        }

        // calculate movement.
        private void MoveDirection() {
            if (enableMovement) {
                // For mouse movement.
                var mPos = Input.mousePosition;
                // for width.
                if (mPos.x < ScreenBorder) {
                    m_PosX = -1;
                }
                else if (mPos.x >= Screen.width - ScreenBorder) {
                    m_PosX = 1;
                }
                else {
                    m_PosX = 0;
                }

                // for height.
                if (mPos.y < ScreenBorder) {
                    m_PosZ = -1;
                }
                else if (mPos.y >= Screen.height - ScreenBorder) {
                    m_PosZ = 1;
                }
                else {
                    m_PosZ = 0;
                }

                // For Keyboard movement
                var keyHMovement = Input.GetAxis("Horizontal");
                var keyVMovement = Input.GetAxis("Vertical");

                // horizontal movement calculation.
                if (Math.Abs(keyHMovement) > 0.1f) {
                    if (enableCombinedMovement) {
                        m_PosX += keyHMovement;
                    }
                    else {
                        m_PosX = keyHMovement;
                    }
                }

                // Vertical movement calculation.
                if (!(Math.Abs(keyVMovement) > 0.1f)) return;
                if (enableCombinedMovement) {
                    m_PosZ += keyVMovement;
                }
                else {
                    m_PosZ = keyVMovement;
                }
            }
            else {
                m_PosX = 0;
                m_PosZ = 0;
            }
        }

        // move based on calculation.
        private void Movement(float posX, float posY, float posZ) {
            //vector to move to.
            posX *= panSpeed;
            posZ *= panSpeed;

            var moveVector = new Vector3(posX, posY, posZ);
            moveVector *= Time.deltaTime;

            // move to this location.
            transform.Translate(moveVector);
        }

        // zoom calculation.
        private void Zoom() {
            if (enableZoom) {
                float fov = camera.fieldOfView;

                if (Input.GetAxis("Mouse ScrollWheel") < 0) {
                    if (fov + zoomSpeed < maxZoom) {
                        fov += zoomSpeed;
                    }
                }
                else if (Input.GetAxis("Mouse ScrollWheel") > 0) {
                    if (fov - zoomSpeed > minZoom) {
                        fov += -zoomSpeed;
                    }
                }
                else {
                    m_PosY = 0;
                }

                camera.fieldOfView = fov;
            }
        }

        // Rotation Calculations.
        private void Rotate() {
            if (!enableRotation) return;
            if (!Input.GetKey(KeyCode.Mouse2)) {
                m_LockPositionOfRotationObj = false;
                enableMovement = true;
                m_LocalRotationSpeed = 0f;
                return;
            }

            // rotate around its locked point axis.
            if (m_LockPositionOfRotationObj == false) {
                MouseRay();
            }

            // lock the rotation position.
            m_LockPositionOfRotationObj = true;
            // lock movement.
            enableMovement = false;

            var mouseDistance = Input.GetAxis("Mouse X");

            // setting rotatable to a number but never to 0f.
            if (mouseDistance > 0f) {
                m_LocalRotationSpeed = rotationSpeed;
            }
            else if (mouseDistance < 0f) {
                m_LocalRotationSpeed = -rotationSpeed;
            }

            if (Mathf.Abs(m_LocalRotationSpeed) > 0f) {
                transform.RotateAround(m_LookPos, Vector3.up,
                    m_LocalRotationSpeed * Time.deltaTime);
            }
        }

        // creates a ray and finds a point to rotate around.
        private void MouseRay() {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 10000)) return;
            m_Position = hit.point;
            m_LookPos = new Vector3(m_Position.x, transform.position.y,
                m_Position.z);
        }
    }
}