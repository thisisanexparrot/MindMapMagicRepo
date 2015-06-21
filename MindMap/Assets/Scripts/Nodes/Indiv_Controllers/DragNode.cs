using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragNode : MonoBehaviour 
{
	/********************/
	/* Global Variables */
	/********************/

	/***** Delegates: if node is selected or destroyed *****/
	public delegate void NodeSelected (bool isOn, DragNode node);
	public static event NodeSelected NodeSelectionUpdate;

	public delegate void NodeDestroyed (DragNode node);
	public static event NodeDestroyed NodeDestroyedUpdate;

	/***** Pointers to controllers and save information *****/
	public NodeSerialized mySerialization;
	public NodeCreator theCreator;

	/***** Dragging variables *****/
	public float scrollMultiplier;
	private bool initIsClick;
	private Vector3 screenPoint;
	private Vector3 offset;
	private bool isBeingMoved;

	/***** Double click variables *****/
	private bool oneClick;
	private bool doubleClicked;
	private float clickTimer;
	private float doubleClickLimit = 0.5f;


	/*****************/
	/*   FUNCTIONS   */
	/*****************/

	/***** Initialization based on whether the button was clicked or dragged *****/
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

	/***** Update based on whether or not node is moving/clicked *****/
	void Update () {
		if (isBeingMoved) {
			DragNodeInSpace();
			CheckMoveZSpace ();
		}
		if (Input.GetMouseButtonUp (0) && mySerialization.isSelected) {
			StopMoving();
		}
	}

	/***** Delete node functions *****/
	public void RemoveNode(){
		theCreator.RemoveNode (gameObject.GetComponent<DragNode> ());
	}

	public void DestroyThisNode () {
		DragNode n = this;
		if (NodeDestroyedUpdate != null) {
			NodeDestroyedUpdate (n);
		}
	}

	/***** Movement functions *****/
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

	/***** Mouse change states *****/
	void OnMouseDown () {
		if ((Time.time - clickTimer) < doubleClickLimit) {
			print ("*** DOUBLE clicked");
			doubleClicked = true;
			Camera.main.GetComponent<MouseOrbitImproved>().SetTarget(gameObject);
			doubleClicked = false;
		} else {
			print ("Single click");
			ResetOffset ();
			StartMoving ();
		}
		clickTimer = Time.time;
	}

	void OnMouseUp () {
		StopMoving ();
	}

	void OnMouseDrag () {
		StartMoving ();
	}

	/***** Reset movement information *****/
	void StopMoving () {
		print ("Stop moving");
		isBeingMoved = false;
		theCreator.Vector3ToFloats (mySerialization, transform.position);
		mySerialization.isSelected = false;

		theCreator.Save ();
		NodeSelectionUpdate (false, this);
	}

	void StartMoving() {
		if (!doubleClicked) {
			print ("Start moving");
			isBeingMoved = true;
			mySerialization.isSelected = true;
			NodeSelectionUpdate (true, this);
		}
	}

	/***** Scroll events *****/
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















