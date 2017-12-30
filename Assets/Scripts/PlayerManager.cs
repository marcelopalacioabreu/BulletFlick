using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Hashtable = ExitGames.Client.Photon.Hashtable;
namespace BulletFlick {
    [System.Serializable]
    public class ToggleEvent : UnityEvent<bool> { }

    public class PlayerManager : Photon.MonoBehaviour {

        [SerializeField] private ToggleEvent onToggleLocal;

        [SerializeField] private GameObject playerBody;

        private GameObject defaultCamera;

        private GameManager gameManager;

        private PlayerUIController playerUIController;

        void Start () {
            gameManager = GameManager.Instance();

            if (photonView.isMine) {
                defaultCamera = Camera.main.gameObject;
                playerBody.layer = LayerMask.NameToLayer("My Player");
            } else {
                playerBody.layer = LayerMask.NameToLayer("Other Player");
            }

            EnablePlayer();
            gameManager.AddPlayer(photonView.ownerId, gameObject);
            playerUIController = GetComponent<PlayerUIController>();
        }

        void FixedUpdate () {
        }

        public void Die (int killerId) {
            if (photonView.isMine) {
                Hashtable deadProperties = PhotonNetwork.player.CustomProperties;
                int deaths = 0;
                if (deadProperties.ContainsKey("deaths")) {
                    deaths = (int)deadProperties["deaths"];
                }
                deadProperties["deaths"] = deaths + 1;
                PhotonNetwork.player.SetCustomProperties(deadProperties);

                if (killerId != -1 && killerId != PhotonNetwork.player.ID) {
                    PhotonPlayer killer = PhotonPlayer.Find(killerId);
                    Hashtable killerProperties = killer.CustomProperties;
                    killerProperties["kills"] = ((int)killerProperties["kills"]) + 1;
                    killer.SetCustomProperties(killerProperties);
                }
                gameManager.Respawn();
            }
            gameManager.RemovePlayer(photonView.ownerId);
            DestroyPlayer();
        }

        public void HitOtherPlayer () {
            playerUIController.ShowHitmarker();
        }

        public void EnablePlayer () {
            if (photonView.isMine) {
                defaultCamera.SetActive(false);
                onToggleLocal.Invoke(true);
            }
        }

        public void DisablePlayer () {
            if (photonView.isMine) {
                onToggleLocal.Invoke(false);
            }
        }

        private void DestroyPlayer () {
            if (photonView.isMine) {
                defaultCamera.SetActive(true);
                PhotonNetwork.Destroy(photonView);
            }
        }
    }
}