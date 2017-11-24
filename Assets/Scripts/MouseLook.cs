using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//place this class on the player root
public class MouseLook : MonoBehaviour {

	public Transform playerCamera;

	public float sensitivityX = 5f;
	public float sensitivityY = 2f;

	public float maximumX = 360f;
	public float minimumX = -360f;

	public float maximumY = 60f;
	public float minimumY = -60f;

	private float rotationX = 0f;
	private float rotationY = 0f;

	//Use body for x and camera for y
	private Quaternion curBodyRotation;
	private Quaternion curCameraRotation;

	void Start () {
		curBodyRotation = transform.localRotation;
		curCameraRotation = playerCamera.localRotation;
	}
		
	void Update () {
		rotationX += Input.GetAxis("Mouse X") * sensitivityX;
		rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

		rotationX = ClampAngle (rotationX, minimumX, maximumX);
		rotationY = ClampAngle (rotationY, minimumY, maximumY);

		Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
		Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, Vector3.left);

		transform.localRotation = curBodyRotation * xQuaternion;	
		playerCamera.localRotation = curCameraRotation * yQuaternion;
	}

	public static float ClampAngle (float angle, float min, float max) {
		if (angle < -360f) {
			angle += 360f;
		}
		if (angle > 360f) {
			angle -= 360f;
		}
		return Mathf.Clamp (angle, min, max);
	}
}
