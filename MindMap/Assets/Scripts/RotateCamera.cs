using UnityEngine;
using System.Collections;

public class RotateCamera : MonoBehaviour {
	private Vector3 screenPoint;
	private Vector3 offset;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			print ("Down");
			//screenPoint = Camera.main.WorldToScreenPoint (gameObject.transform.position);
/*			offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 
			                                                                                    Input.mousePosition.y,
			                                                                                    screenPoint.z));*/
		}
		if ((Input.GetKey (KeyCode.LeftAlt) || (Input.GetKey (KeyCode.RightAlt))) && (Input.GetMouseButton(0))) {
			Vector3 curPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);// + offset;
			transform.LookAt(curPosition);
			print ("Spinning");

			/*flow_target.transform.position = Vector3.Lerp( flow_target.transform.position, target.position, flow_speed);
			this.transform.LookAt ( flow_target.transform );
*/
		}
	}
}

/*
 * 
 * using UnityEngine;
 using UnityEngine;
 using System.Collections;
 
 [AddComponentMenu("Camera-Control/Mouse drag Orbit with zoom")]
 public class DragMouseOrbit : MonoBehaviour
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
 
     // Use this for initialization
     void Start()
     {
         Vector3 angles = transform.eulerAngles;
         rotationYAxis = angles.y;
         rotationXAxis = angles.x;
 
         // Make the rigid body not change rotation
         if (rigidbody)
         {
             rigidbody.freezeRotation = true;
         }
     }
 
     void LateUpdate()
     {
         if (target)
         {
             if (Input.GetMouseButton(0))
             {
                 velocityX += xSpeed * Input.GetAxis("Mouse X") * distance * 0.02f;
                 velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
             }
 
             rotationYAxis += velocityX;
             rotationXAxis -= velocityY;
 
             rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
 
             Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
             Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
             Quaternion rotation = toRotation;
             
             distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
 
             RaycastHit hit;
             if (Physics.Linecast(target.position, transform.position, out hit))
             {
                 distance -= hit.distance;
             }
             Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
             Vector3 position = rotation * negDistance + target.position;
             
             transform.rotation = rotation;
             transform.position = position;
 
             velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
             velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
         }
 
     }
 
     public static float ClampAngle(float angle, float min, float max)
     {
         if (angle < -360F)
             angle += 360F;
         if (angle > 360F)
             angle -= 360F;
         return Mathf.Clamp(angle, min, max);
     }
 }

*/