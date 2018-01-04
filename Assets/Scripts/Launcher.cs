using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace BulletFlick {
    public class Launcher : Photon.PunBehaviour {

        [SerializeField] private byte maxPlayers = 4;

        [SerializeField] private GameObject mapContent;

        [SerializeField] private GameObject mapOptionPrefab;

        private string gameVersion = "1";

        private int mapIndex;

        void Awake () {
            PhotonNetwork.automaticallySyncScene = true;
        }

        // Use this for initialization
        void Start () {
            //start at 1 to skip launcher
            for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++) {
                GameObject mapOption = Instantiate(mapOptionPrefab);
                mapOption.transform.SetParent(mapContent.transform, true);
                mapOption.GetComponent<RectTransform>().anchoredPosition 
                    = new Vector2(0, -50 * (i - 1));

                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = scenePath.Substring(scenePath.LastIndexOf("/") + 1,
                    scenePath.LastIndexOf(".")-(scenePath.LastIndexOf("/") + 1));

                mapOption.GetComponentInChildren<Text>().text = sceneName;
                //need indexCopy for delegate, otherwise runs last i value
                int indexCopy = i;
                mapOption.GetComponentInChildren<Button>().onClick.AddListener(delegate { Play(indexCopy); });
            }
            Connect();
        }

        public void Connect () {
            if (!PhotonNetwork.connected) { 
                PhotonNetwork.ConnectUsingSettings(gameVersion);
            }
        }

        public void Play(int index) {
            mapIndex = index;
            if (PhotonNetwork.connectedAndReady) {
                Hashtable roomProperties = new Hashtable() { { "map", mapIndex } };
                PhotonNetwork.JoinRandomRoom(roomProperties, 0);
            }
        }

        public override void OnJoinedRoom () {
            SceneManager.LoadScene(mapIndex);
        }

        public override void OnPhotonRandomJoinFailed (object[] codeAndMsg) {
            string[] propForLobby = { "map" };
            RoomOptions roomOptions = new RoomOptions {
                MaxPlayers = maxPlayers,
                CustomRoomPropertiesForLobby = propForLobby,
                CustomRoomProperties = new Hashtable() { {"map", mapIndex} }
            };
            PhotonNetwork.CreateRoom(null, roomOptions, null);
        }
    }
}