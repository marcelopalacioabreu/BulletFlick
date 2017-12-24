using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletFlick {
    public class Shoot : Photon.MonoBehaviour {

        [SerializeField] private GameObject arm;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform exitPoint;

        private List<GameObject> bulletPool;
        private Quaternion lastGunRotation;

        private float cooldownTime = 1f;
        private float currentCoolTime;


        void Awake () {
            bulletPool = new List<GameObject>();
        }

        void Update () {
            if (!photonView.isMine) {
                return;
            }

            currentCoolTime -= Time.deltaTime;
            if (Input.GetMouseButtonDown(0) && currentCoolTime <= 0) {
                //have bullet be shot but dont do hit detection on it
                photonView.RPC("ShootVisualBullet", PhotonTargets.Others);
                //spawn local bullet that does damage
                ShootDamageBullet();
            }

            lastGunRotation = arm.transform.localRotation;
        }

        private void ShootDamageBullet() {
            GameObject bullet = GetBullet();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            bulletComponent.Init(EulerAnglesDelta(arm.transform.localEulerAngles, lastGunRotation.eulerAngles) * Time.deltaTime, true);
            currentCoolTime = cooldownTime;
        }
        
        [PunRPC]
        private void ShootVisualBullet() {
            GameObject bullet = GetBullet();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            bulletComponent.Init(EulerAnglesDelta(arm.transform.localEulerAngles, lastGunRotation.eulerAngles) * Time.deltaTime, false);
            currentCoolTime = cooldownTime;
        }

        private GameObject GetBullet () {

            foreach (GameObject bullet in bulletPool) {
                if (!bullet.activeInHierarchy) {
                    bullet.transform.position = exitPoint.position;
                    bullet.transform.rotation = exitPoint.rotation;
                    bullet.SetActive(true);
                    return bullet;
                }
            }
            GameObject newBullet = Instantiate(bulletPrefab, exitPoint.position, exitPoint.rotation);
            bulletPool.Add(newBullet);
            return newBullet;
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
