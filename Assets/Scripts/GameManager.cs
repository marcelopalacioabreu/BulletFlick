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

        private List<GameObject> spawnPoints;

        private Dictionary<int, GameObject> players;

        public static GameManager Instance () {
            return instance;
        }

        void Awake () {
            if(instance != null && instance != this) {
                Destroy(gameObject);
            } else {
                instance = this;
            }

            spawnPoints = new List<GameObject>();
            players = new Dictionary<int, GameObject>();
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
            PhotonNetwork.player.NickName = "Player " + PhotonNetwork.playerList.Length;
            //TODO get all players
        }

        // Update is called once per frame
        void Update () {

        }

        public void AddPlayer (int id, GameObject player) {
            players[id] = player;
        }

        public void RemovePlayer (int id) {
            players.Remove(id);
        }
        //TODO add on disconnect listener
        public void OnLeftRoom () {
            SceneManager.LoadScene("Launcher");
        }

        public void LeaveRoom () {
            PhotonNetwork.LeaveRoom();
        }

        public void Respawn () {
            StartCoroutine(RespawnCoroutine());
        }

        private void OnPhotonPlayerDisconnected (PhotonPlayer otherPlayer) {
            RemovePlayer(otherPlayer.ID);
        }

        private void OnPhotonPlayerPropertiesChanged (object[] playerAndUpdatedProps) {
            if (!PhotonNetwork.isMasterClient) {
                return;
            }
            Hashtable updatedProperties = (Hashtable)playerAndUpdatedProps[1];
            if (updatedProperties.ContainsKey("kills")) {
                if ((int)updatedProperties["kills"] >= 30) {
                    Debug.Log("Winner is " + ((PhotonPlayer)playerAndUpdatedProps[1]).NickName);
                }
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
                Debug.Log(avg);
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
    }
}