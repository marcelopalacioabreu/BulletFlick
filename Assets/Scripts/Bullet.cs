using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletFlick {

    public class Bullet : MonoBehaviour {

        [SerializeField]
        private float bulletSpeed = .01f;
        [SerializeField]
        private float bulletCurveMultiplyer = 10f;
        [SerializeField]
        private float bulletLifeLength = 3f;
        [SerializeField]
        private float maxCurve = 1f;

        public Vector3 bulletCurve;

        private Rigidbody bulletRigidbody;

        private TrailRenderer trailRenderer;

        private float startTime;
        void Start () {

        }

        // Update is called once per frame
        void Update () {
            if (Time.time >= startTime + bulletLifeLength) {
                gameObject.SetActive(false);
            } else {
                bulletRigidbody.velocity = transform.forward * bulletSpeed;
                transform.Rotate(bulletCurve * bulletCurveMultiplyer);
            }
        }

        void OnCollisionEnter (Collision collision) {
            if (!collision.gameObject.CompareTag("Gun")) {
                gameObject.SetActive(false);
            }
        }

        void FixedUpdate () {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), .2f)) {
                Debug.Log("Hit");
                gameObject.SetActive(false);
            }
        }

        public void Init (Vector3 bulletCurve) {
            if (!bulletRigidbody) {
                bulletRigidbody = GetComponent<Rigidbody>();
            }
            if (!trailRenderer) {
                trailRenderer = GetComponent<TrailRenderer>();
            }
            bulletRigidbody.velocity = Vector3.zero;
            bulletRigidbody.angularVelocity = Vector3.zero;

            bulletCurve.x = Mathf.Sign(bulletCurve.x) * Mathf.Min(maxCurve, Mathf.Abs(bulletCurve.x));
            bulletCurve.y = Mathf.Sign(bulletCurve.y) * Mathf.Min(maxCurve, Mathf.Abs(bulletCurve.y));
            bulletCurve.z = 0;
            this.bulletCurve = bulletCurve;
            startTime = Time.time;

            trailRenderer.Clear();
        }
    }
}