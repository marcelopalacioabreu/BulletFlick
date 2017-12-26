using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour {

    [SerializeField] private GameObject scoreboard;
    [SerializeField] private GameObject menu;

    private bool showMenu;
    private Text scoreboardText;

	// Use this for initialization
	void Awake () {
        scoreboardText = scoreboard.GetComponentInChildren<Text>();
        showMenu = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Tab)) {
            UpdateScoreboard();
            scoreboard.SetActive(true);
        } else if(Input.GetKeyUp(KeyCode.Tab)) {
            scoreboard.SetActive(false);
        }
            
        if(Input.GetKeyDown(KeyCode.Escape)) {
            showMenu = !showMenu;
        }
	}

    public void UpdateScoreboard() {
        StringBuilder text = new StringBuilder();
        text.Append(PhotonNetwork.playerName + " ");
        text.Append(PhotonNetwork.player.CustomProperties["kills"] + "/");
        text.Append(PhotonNetwork.player.CustomProperties["deaths"] + "\n");
        foreach (PhotonPlayer player in PhotonNetwork.playerList) {
            if (player.ID != PhotonNetwork.player.ID) {
                text.Append(player.NickName + " ");
                text.Append(player.CustomProperties["kills"] + "/");
                text.Append(player.CustomProperties["deaths"] + "\n");
            }
        }
        scoreboardText.text = text.ToString();
    }
}
