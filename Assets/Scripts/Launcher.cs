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
            if (PhotonNetwork.connected) {
                PhotonNetwork.JoinRandomRoom();
            } else {
                PhotonNetwork.ConnectUsingSettings(gameVersion);
            }
        }

        public override void OnConnectedToMaster () {
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnDisconnectedFromPhoton () {
            
        }

        public override void OnJoinedRoom () {
            SceneManager.LoadScene("Test");
        }

        public override void OnPhotonRandomJoinFailed (object[] codeAndMsg) {
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = maxPlayers }, null);
        }

       
    }
}