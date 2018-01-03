using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BulletFlick {
    public class Launcher : Photon.PunBehaviour {

        [SerializeField] private byte maxPlayers = 4;

        private string gameVersion = "1";

        void Awake () {
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = true;
        }

        // Use this for initialization
        void Start () {
            Connect();
        }

        // Update is called once per frame
        void Update () {

        }

        public void Connect () {
            if (!PhotonNetwork.connected) { 
                PhotonNetwork.ConnectUsingSettings(gameVersion);
            }
        }

        public void Play() {
            if (PhotonNetwork.connectedAndReady) {
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnJoinedRoom () {
            SceneManager.LoadScene(1);
        }

        public override void OnPhotonRandomJoinFailed (object[] codeAndMsg) {
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = maxPlayers }, null);
        }
    }
}