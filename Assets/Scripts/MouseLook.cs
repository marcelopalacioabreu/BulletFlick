using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletFlick {
    //place this class on the player root
    public class MouseLook : MonoBehaviour {


        [SerializeField] private float sensitivityX = 5f;
        [SerializeField] private float sensitivityY = 2f;

        [SerializeField] private float maximumX = 360f;
        [SerializeField] private float minimumX = -360f;

        [SerializeField] private float maximumY = 60f;
        [SerializeField] private float minimumY = -60f;

        [SerializeField] private CursorLockMode cursorLockMode = CursorLockMode.Locked;

        [SerializeField] private Transform playerCamera;

        private float curRotationX = 0f;
        private float curRotationY = 0f;

        //Use body for x and camera for y
        private Quaternion curBodyRotation;
        private Quaternion curCameraRotation;

        void Start () {
            curBodyRotation = transform.localRotation;
            curCameraRotation = playerCamera.localRotation;
        }

        void Update () {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                cursorLockMode = CursorLockMode.None;
                Cursor.visible = true;
            } else if (Input.GetMouseButtonDown(0)) {
                //TODO change to constants
                cursorLockMode = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            Cursor.lockState = cursorLockMode;

            curRotationX += Input.GetAxis("Mouse X") * sensitivityX;
            curRotationY += Input.GetAxis("Mouse Y") * sensitivityY;

            curRotationX = ClampAngle(curRotationX, minimumX, maximumX);
            curRotationY = ClampAngle(curRotationY, minimumY, maximumY);

            Quaternion xQuaternion = Quaternion.AngleAxis(curRotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(curRotationY, Vector3.left);
            
            playerCamera.localRotation = curCameraRotation * xQuaternion * yQuaternion;
        }

        public static float ClampAngle (float angle, float min, float max) {
            if (angle < -360f) {
                angle += 360f;
            }
            if (angle > 360f) {
                angle -= 360f;
            }
            return Mathf.Clamp(angle, min, max);
        }
    }
}