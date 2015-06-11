﻿using UnityEngine;
using System.Collections;

//[AddComponentMenu("Camera-Control/Mouse drag Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour
{
	public Transform target;
	public float distance = 5.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;
	
	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;
	
	public float distanceMin = .5f;
	public float distanceMax = 15f;
	
	public float smoothTime = 2f;
	
	float rotationYAxis = 0.0f;
	float rotationXAxis = 0.0f;
	
	float velocityX = 0.0f;
	float velocityY = 0.0f;

	public GameObject CameraCenter;
	public float minDistance;
	public float maxDistance;
	public float scrollMultiplier;

	
	void Start()
	{
		Vector3 angles = transform.eulerAngles;
		rotationYAxis = angles.y;
		rotationXAxis = angles.x;
	}
	
	void LateUpdate()
	{
		if (target)
		{
			if ((Input.GetKey (KeyCode.LeftAlt) || (Input.GetKey (KeyCode.RightAlt))) && (Input.GetMouseButton(0))) {
				velocityX += xSpeed * Input.GetAxis("Mouse X") * 0.02f;
				velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
			}
			
			rotationYAxis += velocityX;
			rotationXAxis -= velocityY;
			
			rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
			
//			Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
			Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
			Quaternion rotation = toRotation;


			
			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = rotation * negDistance + target.position;
			
			transform.rotation = rotation;
			transform.position = position;
			
			velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
			velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
		}

		CheckZoom ();
		
	}
	
	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}



	void CheckZoom () {
		if ((Input.GetAxis ("Mouse ScrollWheel") != 0) && !Input.GetMouseButton(0)) {
			Vector3 vectorToCenter = new Vector3(0, 0, 0);
			if(Input.GetAxis ("Mouse ScrollWheel") < 0) {
				vectorToCenter = transform.position - target.transform.position;
			}
			else if(Input.GetAxis ("Mouse ScrollWheel") > 0) {
				vectorToCenter =  target.transform.position - transform.position;
			}
			Vector3 normalizedVect = vectorToCenter.normalized;
			float scrollInput = Input.GetAxis("Mouse ScrollWheel") * scrollMultiplier;
			Vector3 newPosition = new Vector3(transform.position.x + normalizedVect.x + scrollInput, 
			                                  transform.position.y + normalizedVect.y + scrollInput, 
			                                  transform.position.z + normalizedVect.z + scrollInput);
			distance = Vector3.Distance(newPosition, target.transform.position);

		}
	}
}