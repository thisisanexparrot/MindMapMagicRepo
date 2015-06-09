using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragNode : MonoBehaviour 
{
	public float scrollMultiplier;
	public NodeSerialized mySerialization;
	public NodeCreator theCreator;

	private bool initIsClick;
	private Vector3 screenPoint;
	private Vector3 offset;
	private bool isBeingMoved;

	/* Initialization based on whether the button was clicked or dragged */
	public void InitializeNode (NodeSerialized newSerialize, NodeCreator creator) {
		theCreator = creator;
		mySerialization = newSerialize;
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

	public void RemoveNode(){
		print ("Clicked white space");
		print ("remove time: " + theCreator.saveNodesList.nodeList.Count);
		//theCreator.saveNodesList.nodeList.RemoveAt(theCreator.saveNodesList.nodeList.Count-1);
		theCreator.RemoveNode (gameObject.GetComponent<DragNode> ());

		print ("remove COMPLETE: " + theCreator.saveNodesList.nodeList.Count);
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
		//((Input.GetKey (KeyCode.LeftAlt) || (Input.GetKey (KeyCode.RightAlt)))
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















