using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ToggleEvent : UnityEvent<bool> {}


public class PlayerManager : Photon.MonoBehaviour {

    [SerializeField]
    private ToggleEvent onToggleLocal;

    private GameObject defaultCamera;

	// Use this for initialization
	void Start () {
        if (photonView.isMine) {
            defaultCamera = Camera.main.gameObject;
        }
        EnablePlayer();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void EnablePlayer () {
        if (photonView.isMine) {
            defaultCamera.SetActive(false);
            onToggleLocal.Invoke(true);
        }
    }
}
