using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BulletFlick {
    public class PlayerUIController : MonoBehaviour {
        [SerializeField] private GameObject crosshairPrefab;
        [SerializeField] private GameObject hitmarkerPrefab;

        private GameObject canvas;
        private RawImage crosshair;
        private RawImage hitmarker;
        // Use this for initialization
        void Start () {
            canvas = GameObject.FindGameObjectWithTag("UI");

            crosshair = Instantiate(crosshairPrefab, canvas.transform).GetComponent<RawImage>();
            hitmarker = Instantiate(hitmarkerPrefab, canvas.transform).GetComponent<RawImage>();

            hitmarker.enabled = false;
            crosshair.enabled = true;
        }

        // Update is called once per frame
        void Update () {

        }

        void OnDisable () {
            hitmarker.enabled = false;
            crosshair.enabled = false;
        }

        public void ShowHitmarker () {
            StartCoroutine(ShowHitmarkerTemp());
        }

        IEnumerator ShowHitmarkerTemp () {
            hitmarker.enabled = true;
            hitmarker.CrossFadeAlpha(0.0f, 0.3f, false);
            yield return new WaitForSeconds(0.3f);
            hitmarker.enabled = false;
        }
    }
}