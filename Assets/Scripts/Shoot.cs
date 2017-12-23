using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletFlick {
    public class Shoot : MonoBehaviour {

        [SerializeField]
        private GameObject bulletPrefab;
        [SerializeField]
        private Transform exitPoint;
        private List<GameObject> bulletPool;

        private Quaternion lastGunRotation;


        void Awake () {
            bulletPool = new List<GameObject>();

        }

        void Update () {
            if (Input.GetMouseButtonDown(0)) {
                GameObject bullet = GetBullet();
                //TODO: calculate bulletVel
                Bullet bulletComponent = bullet.GetComponent<Bullet>();
                bulletComponent.Init(euleruAnglesDelta(transform.eulerAngles, lastGunRotation.eulerAngles) * Time.deltaTime);
            }

            //remember transform.rotation is global space rotation;
            lastGunRotation = transform.rotation;
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

        private Vector3 euleruAnglesDelta (Vector3 cur, Vector3 past) {
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
