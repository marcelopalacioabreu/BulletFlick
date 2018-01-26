using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletFlick {
    public class Shoot : Photon.MonoBehaviour {
        
        [SerializeField] private float cooldownTime = 1f;
        [SerializeField] private float maxCurveTime = .3f;

        [SerializeField] private GameObject arm;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform exitPoint;

        private Queue<GameObject> bulletPool;
        private Quaternion startGunRotation;

        
        private float currentCoolTime;
        private float startBulletTime;

        private bool hasBeenPressedDown = false;

        void Awake () {
            bulletPool = new Queue<GameObject>();
        }

        void LateUpdate () {
            if (!photonView.isMine) {
                return;
            }

            currentCoolTime -= Time.deltaTime;
            if (Input.GetMouseButtonDown(0)) {
                startGunRotation = arm.transform.localRotation;
                startBulletTime = Time.time;
                hasBeenPressedDown = true;
            }
            
            if ((Input.GetMouseButtonUp(0) || (Time.time-startBulletTime) >= maxCurveTime) 
                && hasBeenPressedDown && currentCoolTime <= 0) {

                Vector3 bulletCurve = (EulerAnglesDelta(arm.transform.localEulerAngles, startGunRotation.eulerAngles) / (Time.time-startBulletTime));
                //have bullet be shot on other players but no hit detection
                photonView.RPC("ShootVisualBullet", PhotonTargets.Others, bulletCurve, exitPoint.transform.position, exitPoint.transform.rotation);
                //spawn local bullet that does damage
                ShootDamageBullet(bulletCurve);
                hasBeenPressedDown = false;
                currentCoolTime = cooldownTime;
            }
        }

        private void ShootDamageBullet(Vector3 bulletCurve) {
            GameObject bullet = GetBullet();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            bulletComponent.Init(bulletCurve, true, gameObject);            
        }
        
        [PunRPC]
        private void ShootVisualBullet(Vector3 bulletCurve, Vector3 bulletPosition, Quaternion bulletRotation) {
            GameObject bullet = GetBullet();
            bullet.transform.position = bulletPosition;
            bullet.transform.rotation = bulletRotation;
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            bulletComponent.Init(bulletCurve, false, null);
        }

        private GameObject GetBullet () {
            if (bulletPool.Count > 0) {
                GameObject bullet = bulletPool.Dequeue();
                bullet.transform.position = exitPoint.position;
                bullet.transform.rotation = exitPoint.rotation;
                bullet.SetActive(true);
                return bullet;
            }
   
            GameObject newBullet = Instantiate(bulletPrefab, exitPoint.position, exitPoint.rotation);
            return newBullet;
        }

        public void AddBulletToPool(GameObject bullet) {
            bulletPool.Enqueue(bullet);
        }

        private Vector3 EulerAnglesDelta (Vector3 cur, Vector3 past) {
            Vector3 deltaAngles = cur - past;
            for (int i = 0; i < 3; i++) {
                if (deltaAngles[i] > 180) {
                    deltaAngles[i] -= 360;
                } else if (deltaAngles[i] < -180) {
                    deltaAngles[i] += 360;
                }
            }
            return deltaAngles;
        }
    }
}
