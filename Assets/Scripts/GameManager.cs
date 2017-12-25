using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour {

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject spawnPointHolder;

    private List<GameObject> spawnPoints;

    private Dictionary<int,GameObject> players;

    void Awake () {
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
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddPlayer(int id, GameObject player) {
        players[id] = player;
    }

    public void RemovePlayer(int id) {
        players.Remove(id);
    }

    public void OnLeftRoom() {
        SceneManager.LoadScene("Launcher");
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    public void Respawn() {
        StartCoroutine(RespawnCoroutine());
    }

    private GameObject FindBestSpawnPoint() {
        if(players.Count == 0) {
            return spawnPoints[Random.Range(0, spawnPoints.Count)];
        }

        float minAvgDistance = float.MaxValue;
        GameObject bestSpawnPoint = null;
        foreach (GameObject spawnPoint in spawnPoints) {
            float sum = 0;
            foreach(KeyValuePair<int,GameObject> player in players) {
                sum += Vector3.Distance(player.Value.transform.position, spawnPoint.transform.position);
            }
            float avg = sum / players.Count;
            Debug.Log(avg);
            if(avg <= minAvgDistance) {
                minAvgDistance = avg;
                bestSpawnPoint = spawnPoint;
            }
        }
        return bestSpawnPoint;
    }

    private IEnumerator RespawnCoroutine () {
        Debug.Log("Spawn Start");
        yield return new WaitForSeconds(1f);
        Debug.Log("Spawn");
        Vector3 spawnPoint = FindBestSpawnPoint().transform.position;
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity, 0);
    }
}
