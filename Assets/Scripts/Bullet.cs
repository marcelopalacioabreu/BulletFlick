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
        [SerializeField]
        private float raycastLength = 0.2f;

        public Vector3 bulletCurve;

        private Rigidbody bulletRigidbody;

        private TrailRenderer trailRenderer;

        private float startTime;

        private bool isDamageBullet;

        public void Init (Vector3 bulletCurve, bool isDamageBullet) {
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
            this.isDamageBullet = isDamageBullet;
            startTime = Time.time;

            trailRenderer.Clear();
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
                Hit(collision.transform.root.gameObject);
            }
        }

        void FixedUpdate () {
            RaycastHit hit;

            //TODO: replace magic number
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, raycastLength)) {
                Hit(hit.transform.root.gameObject);
            }
        }

        private void Hit (GameObject other) {
            Debug.Log("Hello");
            if (isDamageBullet && other.CompareTag("Player")) {
                other.GetComponent<PhotonView>().RPC("Damage", PhotonTargets.All, 50);
                Debug.Log("Damage");
            }
            gameObject.SetActive(false);
        }
    }
}