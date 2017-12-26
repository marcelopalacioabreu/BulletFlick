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
            scoreboard.SetActive(true);
        } else if(Input.GetKeyUp(KeyCode.Tab)) {
            scoreboard.SetActive(false);
        }
            
        if(Input.GetKeyDown(KeyCode.Escape)) {
            showMenu = !showMenu;
        }

        UpdateScoreboard();
	}

    public void UpdateScoreboard() {
        StringBuilder text = new StringBuilder();
        foreach(PhotonPlayer player in PhotonNetwork.playerList) {
            text.Append(player.NickName + " ");
            text.Append(player.CustomProperties["kills"] + "/" + player.CustomProperties["deaths"]+"\n");
        }
        scoreboardText.text = text.ToString();
    }
}
