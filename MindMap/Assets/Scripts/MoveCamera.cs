using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour {
	public GameObject CameraCenter;
	public float minDistance;
	public float maxDistance;
	public float scrollMultiplier;

	void Update () {
		//transform.LookAt (CameraCenter.transform.position);
		CheckZoom ();
	}

	void CheckZoom () {
		if ((Input.GetAxis ("Mouse ScrollWheel") != 0) && !Input.GetMouseButton(0)) {
			if(Input.GetAxis ("Mouse ScrollWheel") < 0) {

				Vector3 vectorToCenter = transform.position - CameraCenter.transform.position;
				Vector3 normalizedVect = vectorToCenter.normalized;
				float scrollInput = Input.GetAxis("Mouse ScrollWheel") * scrollMultiplier;
				Vector3 newPosition = new Vector3(transform.position.x + normalizedVect.x + scrollInput, 
			   	                               transform.position.y + normalizedVect.y + scrollInput, 
			    	                              transform.position.z + normalizedVect.z + scrollInput);
				transform.position = newPosition;
			}
			else if(Input.GetAxis ("Mouse ScrollWheel") > 0) {
				
				Vector3 vectorToCenter =  CameraCenter.transform.position - transform.position;
				Vector3 normalizedVect = vectorToCenter.normalized;
				float scrollInput = Input.GetAxis("Mouse ScrollWheel") * scrollMultiplier;
				Vector3 newPosition = new Vector3(transform.position.x + normalizedVect.x + scrollInput, 
				                                  transform.position.y + normalizedVect.y + scrollInput, 
				                                  transform.position.z + normalizedVect.z + scrollInput);
				transform.position = newPosition;
			}
		}
	}
}
