using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour {

    [SerializeField]
    private GameObject playerPrefab;

	// Use this for initialization
	void Start () {
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0,10,0), Quaternion.identity, 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnLeftRoom() {
        SceneManager.LoadScene("Launcher");
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }
}
