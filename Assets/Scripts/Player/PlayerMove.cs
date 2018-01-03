using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletFlick {
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMove : MonoBehaviour {

        private const float Gravity = 9.8f;

        [SerializeField] public float speed = 3.0f;
        [SerializeField] public float jumpSpeed = 5.0f;

        [SerializeField] private Transform playerCamera;

        private CharacterController controller;
        private Vector3 moveDirection;

        private bool hasDoubleJumped;

        void Start () {
            controller = GetComponent<CharacterController>();
            hasDoubleJumped = false;
            moveDirection = Vector3.zero;
        }
        
        void Update () {
            if (controller.isGrounded) {
                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                moveDirection = playerCamera.transform.TransformDirection(moveDirection);
                moveDirection *= speed;
                hasDoubleJumped = false;
            }
            if (Input.GetButtonDown("Jump") && (!hasDoubleJumped || controller.isGrounded)) {
                Debug.Log("Jump");
                moveDirection.y = jumpSpeed;
                if (!controller.isGrounded) {
                    Debug.Log("Double");
                    hasDoubleJumped = true;
                }
            }
            moveDirection.y -= Gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
        }
    }
}
