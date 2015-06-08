using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragNode : MonoBehaviour
{
	public float scrollMultiplier;

	private bool initIsClick;
	private Vector3 screenPoint;
	private Vector3 offset;
	private bool isBeingMoved;

	/* Initialization based on whether the button was clicked or dragged */
	public void Initialize (bool initClick) {
		initIsClick = initClick;
		isBeingMoved = true;
		ResetOffset ();
	}

	/* Update functions */
	void Update () {
		if (isBeingMoved) {
			DragNodeInSpace();
			CheckMoveZSpace ();
		}
		if (Input.GetMouseButtonUp (0)) {
			isBeingMoved = false;
		}
	}

	void ResetOffset () {
		screenPoint = Camera.main.WorldToScreenPoint (gameObject.transform.position);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 
		                                                                                    Input.mousePosition.y,
		                                                                                    screenPoint.z));
	}

	void DragNodeInSpace () {
		Vector3 curScreenPoint = new Vector3 (Input.mousePosition.x,  
		                                      Input.mousePosition.y,
		                                      screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenPoint) + offset;
		transform.position = curPosition;
	}

	/* Mouse change states */
	void OnMouseDown () {
		ResetOffset ();
		isBeingMoved = true;
	}

	void OnMouseUp () {
		isBeingMoved = false;
	}

	void OnMouseDrag () {
		isBeingMoved = true;
	}

	void OnClick () {
		isBeingMoved = false;
	}

	/* Scroll events */
	void CheckMoveZSpace () {
		//if ((Input.GetKey (KeyCode.LeftAlt) || (Input.GetKey (KeyCode.RightAlt)))
		// && (Input.GetAxis ("Mouse ScrollWheel") != 0)) {
		if(Input.GetAxis("Mouse ScrollWheel") != 0) {
			ResetOffset();
			Vector3 curPosition = new Vector3(screenPoint.x, 
			        	                      screenPoint.y, 
			            	                  screenPoint.z + (Input.GetAxis("Mouse ScrollWheel") * scrollMultiplier));
			curPosition = Camera.main.ScreenToWorldPoint(curPosition);
			transform.position = curPosition;
			ResetOffset();
		}
	}




}















