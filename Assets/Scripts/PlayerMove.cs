using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour {

	public float speed = 3.0f;

	private CharacterController controller;
	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () {
		//get player input and make it into global vector
		Vector3 moveVec = transform.TransformDirection(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		controller.SimpleMove (moveVec * speed);
	}
}
