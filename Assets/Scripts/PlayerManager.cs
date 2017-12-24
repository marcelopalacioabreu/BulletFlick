using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ToggleEvent : UnityEvent<bool> { }


public class PlayerManager : Photon.MonoBehaviour {

    [SerializeField] private ToggleEvent onToggleLocal;

    private GameObject defaultCamera;

    private GameManager gameManager;

    // Use this for initialization
    void Start () {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (photonView.isMine) {
            defaultCamera = Camera.main.gameObject;
        }
        EnablePlayer();
    }

    // Update is called once per frame
    void Update () {

    }

    public void Die () {
        if (photonView.isMine) {
            gameManager.Respawn();
        }
        DisablePlayer();
    }

    private void EnablePlayer () {
        if (photonView.isMine) {
            defaultCamera.SetActive(false);
            onToggleLocal.Invoke(true);
        }
    }

    private void DisablePlayer () {
        if (photonView.isMine) {
            defaultCamera.SetActive(true);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
