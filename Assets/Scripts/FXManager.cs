using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BulletFlick {
    public class FXManager : MonoBehaviour {
        private static FXManager instance;
        [SerializeField] private GameObject bulletSplashPrefab;

        private Queue<GameObject> bulletSplashPool;

        public static FXManager Instance () {
            return instance;
        }

        // Use this for initialization
        void Awake () {
            if (instance != null && instance != this) {
                Destroy(gameObject);
            } else {
                instance = this;
            }
            bulletSplashPool = new Queue<GameObject>();
        }

        // Update is called once per frame
        void Update () {

        }

        public void SpawnBulletSplash (Vector3 position, Vector3 normal) {
            Quaternion rotation = Quaternion.LookRotation(-normal);
            position += normal * .01f;
            if (bulletSplashPool.Count > 0) {
                GameObject bulletSplash = bulletSplashPool.Dequeue();
                bulletSplash.transform.position = position;
                bulletSplash.transform.rotation = rotation;
                StartCoroutine(TempActiveBulletSplash(bulletSplash));
            } else {
                GameObject bulletSplash = Instantiate(bulletSplashPrefab, position, rotation);
                StartCoroutine(TempActiveBulletSplash(bulletSplash));
            }

        }

        IEnumerator TempActiveBulletSplash(GameObject bulletSplash) {
            bulletSplash.SetActive(true);
            yield return new WaitForSeconds(3f);
            bulletSplashPool.Enqueue(bulletSplash);
            bulletSplash.SetActive(false);
        }
    }
}