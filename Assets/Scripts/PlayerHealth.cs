using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Photon.MonoBehaviour {

    private float initialHealth = 100;
    private float currentHealth;

    private PlayerManager playerManager;

    // Use this for initialization
    void Awake () {
        currentHealth = initialHealth;
    }

    void Start () {
        playerManager = GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update () {

    }

    [PunRPC]
    public void Damage (int damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) {
            playerManager.Die();
        }
    }
}
