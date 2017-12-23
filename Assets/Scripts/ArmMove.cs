using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BulletFlick {
    public class ArmMove : MonoBehaviour {

        public float armSpeed;

        [SerializeField]
        private Transform playerCamera;

        // Use this for initialization
        void Start () {

        }

        // Update is called once per frame
        void Update () {
            transform.rotation = Quaternion.Lerp(transform.rotation, playerCamera.rotation, armSpeed * Time.deltaTime);
        }
    }
}
