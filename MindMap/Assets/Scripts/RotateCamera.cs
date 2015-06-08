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
