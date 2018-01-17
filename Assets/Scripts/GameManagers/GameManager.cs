using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
namespace BulletFlick {
    public class GameManager : Photon.PunBehaviour {

        private static GameManager instance;

        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject spawnPointHolder;
        [SerializeField] private GameUIController gameUIController;

        private List<GameObject> spawnPoints;

        private Dictionary<int, GameObject> players;

        private float xSensitivity;
        private float ySensitivity;

        public static GameManager Instance () {
            return instance;
        }

        void Awake () {
            Application.runInBackground = true;
            if (instance != null && instance != this) {
                Destroy(gameObject);
            } else {
                instance = this;
            }

            spawnPoints = new List<GameObject>();
            players = new Dictionary<int, GameObject>();

            if (!PlayerPrefs.HasKey("X Sensitivity")) {
                PlayerPrefs.SetFloat("X Sensitivity", 5);
                xSensitivity = 5;
            }
            if (!PlayerPrefs.HasKey("Y Sensitivity")) {
                PlayerPrefs.SetFloat("Y Sensitivity", 2);
                ySensitivity = 2;
            }

            xSensitivity = PlayerPrefs.GetFloat("X Sensitivity");
            ySensitivity = PlayerPrefs.GetFloat("Y Sensitivity");
        }

        // Use this for initialization
        void Start () {
            foreach (Transform child in spawnPointHolder.transform) {
                spawnPoints.Add(child.gameObject);
            }

            Vector3 spawnPoint = FindBestSpawnPoint().transform.position;
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity, 0);

            Hashtable playerProperties = new Hashtable();
            playerProperties["kills"] = 0;
            playerProperties["deaths"] = 0;
            PhotonNetwork.player.SetCustomProperties(playerProperties);
            PhotonNetwork.OnEventCall += OnEventRaised;
            //TODO get all players for playerList
        }

        public void OnEventRaised (byte eventcode, object content, int senderid) {
            //gameover
            if (eventcode == 0) {
                Debug.Log("Game over");
                GameOver((string)content);
            }
        }

        public void GameOver (string winner) {
            gameUIController.Win(winner);
            Time.timeScale = 0;
            StartCoroutine(EndGame());
        }

        public void AddPlayer (int id, GameObject player) {
            players[id] = player;
        }

        public void RemovePlayer (int id) {
            players.Remove(id);
        }

        public void OnLeftRoom () {
            SceneManager.LoadScene("Launcher");
        }

        //Used by exit button
        public void LeaveRoom () {
            PhotonNetwork.LeaveRoom();
        }

        public void Respawn () {
            StartCoroutine(RespawnCoroutine());
        }

        public void EnableLocalPlayer () {
            if (players.ContainsKey(PhotonNetwork.player.ID)) {
                players[PhotonNetwork.player.ID].GetComponent<PlayerManager>().EnablePlayer();
            }
        }

        public void DisableLocalPlayer () {
            if (players.ContainsKey(PhotonNetwork.player.ID)) {
                players[PhotonNetwork.player.ID].GetComponent<PlayerManager>().DisablePlayer();
            }
        }

        private void OnPhotonPlayerDisconnected (PhotonPlayer otherPlayer) {
            RemovePlayer(otherPlayer.ID);
        }

        private void OnPhotonPlayerPropertiesChanged (object[] playerAndUpdatedProps) {
            if (!PhotonNetwork.isMasterClient) {
                return;
            }
            Hashtable updatedProperties = (Hashtable)playerAndUpdatedProps[1];
            if (updatedProperties.ContainsKey("kills") && (int)updatedProperties["kills"] >= 30) {
                RaiseEventOptions options = RaiseEventOptions.Default;
                options.CachingOption = EventCaching.AddToRoomCacheGlobal;
                options.Receivers = ReceiverGroup.All;
                PhotonNetwork.RaiseEvent(0, ((PhotonPlayer)playerAndUpdatedProps[0]).NickName, true, options);
            }
        }

        private GameObject FindBestSpawnPoint () {
            if (players.Count == 0) {
                return spawnPoints[Random.Range(0, spawnPoints.Count)];
            }

            float maxAvgDistance = 0;
            GameObject bestSpawnPoint = null;
            foreach (GameObject spawnPoint in spawnPoints) {
                float sum = 0;
                foreach (KeyValuePair<int, GameObject> player in players) {
                    sum += Vector3.Distance(player.Value.transform.position, spawnPoint.transform.position);
                }
                float avg = sum / players.Count;
                if (avg >= maxAvgDistance) {
                    maxAvgDistance = avg;
                    bestSpawnPoint = spawnPoint;
                }
            }
            return bestSpawnPoint;
        }

        private IEnumerator RespawnCoroutine () {
            yield return new WaitForSeconds(2f);
            Debug.Log("Spawn");
            Vector3 spawnPoint = FindBestSpawnPoint().transform.position;
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity, 0);
            AddPlayer(PhotonNetwork.player.ID, player);
        }

        private IEnumerator EndGame () {
            yield return new WaitForSecondsRealtime(4f);
            PhotonNetwork.room.IsOpen = false;
            Time.timeScale = 1;
            PhotonNetwork.LeaveRoom();
        }

        public float XSensitivity {
            get {
                return xSensitivity;
            }

            set {
                xSensitivity = value;
            }
        }

        public float YSensitivity {
            get {
                return ySensitivity;
            }

            set {
                ySensitivity = value;
            }
        }
    }
}