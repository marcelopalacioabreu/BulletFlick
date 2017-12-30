using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
namespace BulletFlick {
    public class PlayerHealth : Photon.MonoBehaviour {

        private float initialHealth = 100;
        private float currentHealth;

        private PlayerManager playerManager;

        // Use this for initialization
        void Awake () {
            currentHealth = initialHealth;
        }

        void Start () {
            playerManager = GetComponent<PlayerManager>();
        }

        // Update is called once per frame
        void FixedUpdate () {
            if (transform.position.y < 0) {
                photonView.RPC("Damage", PhotonTargets.All, 100, -1);
            }
        }

        [PunRPC]
        public void Damage (int damage, int playerId) {
            currentHealth -= damage;
            if (currentHealth <= 0) {
                playerManager.Die(playerId);
            }
        }
    }
}