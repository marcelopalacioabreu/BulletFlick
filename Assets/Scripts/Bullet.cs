using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletFlick {

    public class Bullet : MonoBehaviour {
         
        [SerializeField] private float bulletSpeed = 40f;
        /*using animationcurve as a curve for multiplyer
         * x represents original bulletcurve y represents multipyer */
        [SerializeField] private float maxInitBulletCurve = 500f;
        [SerializeField] private AnimationCurve curveMultiplyer;
        [SerializeField] private float bulletLifeLength = 3f;
        [SerializeField] private float maxCurve = 1f;
        [SerializeField] private float raycastLength = 0.95f;

        [SerializeField] private int bodyDamage = 50;
        [SerializeField] private int headDamage = 100;
        private Vector3 bulletCurve;
        private Rigidbody bulletRigidbody;
        private TrailRenderer trailRenderer;

        private GameObject playerOwner; 

        private float startTime;
        private bool isDamageBullet;

        private FXManager fXManager;

        public void Init (Vector3 bulletCurve, bool isDamageBullet, GameObject owner) {
            if (!bulletRigidbody) {
                bulletRigidbody = GetComponent<Rigidbody>();
            }
            if (!trailRenderer) {
                trailRenderer = GetComponent<TrailRenderer>();
            }
            playerOwner = owner;
            bulletRigidbody.velocity = Vector3.zero;
            bulletRigidbody.angularVelocity = Vector3.zero;
            //put initial bulletCurve in 0 to 1 range
            bulletCurve.x = Mathf.Sign(bulletCurve.x) * Mathf.Min(Mathf.Abs(bulletCurve.x), maxInitBulletCurve) / maxInitBulletCurve;
            bulletCurve.y = Mathf.Sign(bulletCurve.y) * Mathf.Min(Mathf.Abs(bulletCurve.y), maxInitBulletCurve) / maxInitBulletCurve;
            bulletCurve.x = Mathf.Sign(bulletCurve.x) * curveMultiplyer.Evaluate(Mathf.Abs(bulletCurve.x)) * maxCurve;
            bulletCurve.y = Mathf.Sign(bulletCurve.y) * curveMultiplyer.Evaluate(Mathf.Abs(bulletCurve.y)) * maxCurve;
            bulletCurve.z = 0;
            this.bulletCurve = bulletCurve;
            this.isDamageBullet = isDamageBullet;
            startTime = Time.time;

            trailRenderer.Clear();
        }

        void Start () {
            fXManager = FXManager.Instance();     
        }

        // Update is called once per frame
        void Update () {
            
        }

        void OnCollisionEnter (Collision collision) {
            if (!collision.gameObject.CompareTag("Gun")) {
                ContactPoint contact = collision.contacts[0];
                Hit(collision.gameObject, contact.point, contact.normal);
            }
        }

        void FixedUpdate () {
            if (Time.time >= startTime + bulletLifeLength) {
                playerOwner.GetComponent<Shoot>().AddBulletToPool(gameObject);
                gameObject.SetActive(false);
            } else {
                bulletRigidbody.velocity = transform.forward * bulletSpeed;
                transform.Rotate(bulletCurve);
            }

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward),
                out hit, raycastLength)) {
                Hit(hit.transform.gameObject, hit.point, hit.normal);
            }
        }

        private void Hit (GameObject other, Vector3 position, Vector3 normal) {
            GameObject root = other.transform.root.gameObject;
            if (isDamageBullet && root.CompareTag("Player")) {
                if (other.CompareTag("Head")) {
                    root.GetComponent<PhotonView>()
                        .RPC("Damage", PhotonTargets.All, headDamage, PhotonNetwork.player.ID);
                } else {
                    root.GetComponent<PhotonView>()
                        .RPC("Damage", PhotonTargets.All, bodyDamage, PhotonNetwork.player.ID);
                }
                playerOwner.GetComponent<PlayerManager>().HitOtherPlayer();
            }
            fXManager.SpawnBulletSplash(position, normal);
            playerOwner.GetComponent<Shoot>().AddBulletToPool(gameObject);
            gameObject.SetActive(false);
        }
    }
}