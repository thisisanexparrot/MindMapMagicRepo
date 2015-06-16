using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragNode : MonoBehaviour 
{
	public delegate void NodeSelected (bool isOn, DragNode node);
	public static event NodeSelected NodeSelectionUpdate;

	public delegate void NodeDestroyed (DragNode node);
	public static event NodeDestroyed NodeDestroyedUpdate;

	public float scrollMultiplier;
	public NodeSerialized mySerialization;
	public NodeCreator theCreator;

	private bool initIsClick;
	private Vector3 screenPoint;
	private Vector3 offset;
	private bool isBeingMoved;

	/* Initialization based on whether the button was clicked or dragged */
	public void InitializeNode (NodeSerialized newSerialize, NodeCreator creator, bool isNew) {
		theCreator = creator;
		mySerialization = newSerialize;
		if (isNew) {
			StartMoving();
		} 
		else {
			StopMoving();
		}
		ResetOffset ();
	}

	/* Update functions */
	void Update () {
		if (isBeingMoved) {
			DragNodeInSpace();
			CheckMoveZSpace ();
		}
		if (Input.GetMouseButtonUp (0)) {
			StopMoving();
		}
	}

	public void DestroyThisNode () {
		DragNode n = this;
		if (NodeDestroyedUpdate != null) {
			NodeDestroyedUpdate (n);
		}
	}

	/* Movement functions */
	public void RemoveNode(){
		theCreator.RemoveNode (gameObject.GetComponent<DragNode> ());
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
		StartMoving ();
	}

	void OnMouseUp () {
		StopMoving ();
	}

	void OnMouseDrag () {
		StartMoving ();
	}

	void OnClick () {
		StopMoving ();
	}

	/* Reset movement information*/
	void StopMoving () {
		isBeingMoved = false;
		theCreator.Vector3ToFloats (mySerialization, transform.position);
		mySerialization.isSelected = false;

		theCreator.Save ();
		NodeSelectionUpdate (false, this);
	}

	void StartMoving() {
		isBeingMoved = true;
		mySerialization.isSelected = true;
		NodeSelectionUpdate (true, this);
	}

	/* Scroll events */
	void CheckMoveZSpace () {
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















