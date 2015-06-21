using UnityEngine;
using System.Collections;

//[AddComponentMenu("Camera-Control/Mouse drag Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour
{
	public delegate void RepositionComplete (bool isLocked);
	public static event RepositionComplete completeMove;

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
	public Vector3 nextCenter;
	public float minDistance;
	public float maxDistance;
	public float scrollMultiplier;

	public float panThreshold = 1.0f;

	
	void Start()
	{
		Vector3 angles = transform.eulerAngles;
		rotationYAxis = angles.y;
		rotationXAxis = angles.x;

		target = CameraCenter.transform;
		nextCenter = CameraCenter.transform.position;
	}

	public void SetTarget (GameObject newTarget) {
		//CameraCenter.transform.position = newTarget.position;
		nextCenter = newTarget.transform.position;
	}

	void SmoothPan () {
		float mag = Vector3.Magnitude (CameraCenter.transform.position - nextCenter);
		float distance_modifier = mag * 0.5f;
		CameraCenter.transform.position = Vector3.Lerp (CameraCenter.transform.position, 
		                                                nextCenter, 
		                                                1.5f * distance_modifier * Time.deltaTime);
		if(mag < panThreshold) {
			print ("NOW unlocked");
			completeMove (false);
		}
	}

	void FixedUpdate () {
		float mag = Vector3.Magnitude (CameraCenter.transform.position - nextCenter);
		if (mag > 0.1f) {
			SmoothPan ();
		}
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