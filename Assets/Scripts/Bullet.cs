using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletFlick {

    public class Bullet : MonoBehaviour {
         
        [SerializeField] private float bulletSpeed = 40f;
        /*using animationcurve as a curve for multiplyer
         * x represents original bulletcurve y represents multipyer */
        [SerializeField] private AnimationCurve curveMultiplyer;
        [SerializeField] private float bulletLifeLength = 3f;
        [SerializeField] private float maxCurve = 1f;
        [SerializeField] private float raycastLength = 0.2f;

        [SerializeField] private int bodyDamage = 50;
        [SerializeField] private int headDamage = 100;
        private Vector3 bulletCurve;
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
            bulletCurve.x = Mathf.Sign(bulletCurve.x) * curveMultiplyer.Evaluate(Mathf.Abs(bulletCurve.x)) * maxCurve;
            bulletCurve.y = Mathf.Sign(bulletCurve.y) * curveMultiplyer.Evaluate(Mathf.Abs(bulletCurve.y)) * maxCurve;
            bulletCurve.z = 0;
            this.bulletCurve = bulletCurve;
            this.isDamageBullet = isDamageBullet;
            startTime = Time.time;

            trailRenderer.Clear();
        }

        // Update is called once per frame
        void Update () {
            
        }

        void OnCollisionEnter (Collision collision) {
            if (!collision.gameObject.CompareTag("Gun")) {
                Hit(collision.gameObject);
            }
        }

        void FixedUpdate () {
            //TODO: bulletmultiplyer curve
            if (Time.time >= startTime + bulletLifeLength) {
                gameObject.SetActive(false);
            } else {
                bulletRigidbody.velocity = transform.forward * bulletSpeed;
                transform.Rotate(bulletCurve);
            }

            RaycastHit hit;

            //TODO: replace magic number
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, raycastLength)) {
                Hit(hit.transform.gameObject);
            }
        }

        private void Hit (GameObject other) {
            GameObject root = other.transform.root.gameObject;
            if (isDamageBullet && root.CompareTag("Player")) {
                if (other.CompareTag("Head")) {
                    root.GetComponent<PhotonView>().RPC("Damage", PhotonTargets.All, headDamage);
                } else {
                    root.GetComponent<PhotonView>().RPC("Damage", PhotonTargets.All, bodyDamage);
                }

            }
            gameObject.SetActive(false);
        }
    }
}