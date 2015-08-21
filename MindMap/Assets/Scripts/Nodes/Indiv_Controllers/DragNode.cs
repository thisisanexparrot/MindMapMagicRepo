using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

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

	public delegate void UpdateName (string newName);
	public static event UpdateName NameWasUpdated;

	/***** Node Properties *****/
	public string title;
	public string description;
	public int idNumber;

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

	/***** Materials *****/
	public Material normalMaterial;
	public Material hoverMaterial;
	public Material selectedMaterial;
	public Material previousMaterial;

	/***** Lerp variables *****/
	public float originalScale = 1.0f;
	public float hoverScale = 1.2f;
	public bool movementIsLocked = false;
	public Camera mainCamera;

	/***** UI elements *****/
	public Wormhole myWormhole;

	/***** Parenting *****/
	public DragNode newPotentialParent;

	/*****************/
	/*   FUNCTIONS   */
	/*****************/

	/***** INITIALIZATIONS *****/
	/* Initialization based on whether the button was clicked or dragged */
	public void InitializeNode (NodeCreator creator, bool isNew) {
		previousMaterial = normalMaterial;
		if (isNew) {
			StartMoving();
		} 
		else {
			StopMoving();
		}
		ResetOffset ();
		mainCamera = Camera.main;
	}
	
	void OnEnable () {
		MouseOrbitImproved.completeMove += SetMovementLock;
		NodeCreator.CurrentlyFocusedNodeUpdated += FocusUpdate;

		myWormhole = GetComponentInChildren<Wormhole> ();
		myWormhole.gameObject.SetActive (false);
	}
	
	void OnDisable () {
		MouseOrbitImproved.completeMove -= SetMovementLock;
		NodeCreator.CurrentlyFocusedNodeUpdated -= FocusUpdate;
	}

	/* Update based on whether or not node is moving/clicked */
	void Update () {
		if (isBeingMoved) {
			DragNodeInSpace();
			CheckMoveZSpace ();
		}
//		if (Input.GetMouseButtonUp (0) && mySerialization.isSelected) {
//			StopMoving();
//		}
		if (mainCamera) {
			transform.LookAt (transform.position + mainCamera.transform.rotation * Vector3.forward,
			                 mainCamera.transform.rotation * Vector3.up);
		}
	}

	/***** GET & SET NODE PROPERTIES *****/
	/* Update the node's ID Number */
	public void SetIDNumber (int newIDNumber) {
		idNumber = newIDNumber;
	}

	/* Update the node's title */
	public void SetName (string newTitle, bool firstLoad) {
		title = newTitle;
		NameWasUpdated (newTitle);
		if (!firstLoad) {
			DatabaseAccess db = NodeCreator.creator.GrandDatabase;
			db.SetObjectInTable (DatabaseAccess.tn_node, idNumber, DatabaseAccess.node_name, newTitle);
		} 
	}

	/* Update the node's description */
	public void SetDescription (string newDescription, bool firstLoad) {
		description = newDescription;
		if (!firstLoad) {
			DatabaseAccess db = NodeCreator.creator.GrandDatabase;
			db.SetObjectInTable (DatabaseAccess.tn_node, idNumber, DatabaseAccess.node_desc, newDescription);
		}
	}


	void SetMovementLock (bool newIsLocked) {
		movementIsLocked = newIsLocked;
	}
	
	/* Check if the current node is being selected, and update its status if so */	
	void FocusUpdate (DragNode newFocusedNode) {
		if (newFocusedNode && (newFocusedNode.idNumber == idNumber)) {
			transform.localScale = new Vector3 (1.5f, 1.5f, 1.5f);
			myWormhole.gameObject.SetActive (true);
		} else {
			transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			myWormhole.gameObject.SetActive (false);
		}
	}

	/***** Delete node functions *****/
	public void RemoveNode(){
		NodeCreator.creator.RemoveNode (gameObject.GetComponent<DragNode> ());
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
		NodeCreator.creator.UpdateCurrentlySelectedNode (this);
		if ((Time.time - clickTimer) < doubleClickLimit) {
			doubleClicked = true;
			SetMovementLock (true);
			Camera.main.GetComponent<MouseOrbitImproved>().SetTarget(gameObject, false);
			doubleClicked = false;
		} else {
			ResetOffset ();
			StartMoving ();
		}
		clickTimer = Time.time;
	}

	void OnMouseUp () {
		Debug.Log ("regular click");
		StopMoving ();
		if (newPotentialParent) {
			DropIntoWormhole ();
		}
	}

	void OnMouseDrag () {
		StartMoving ();
	}

	public void OnMouseEnter () {
		SetMaterial (hoverMaterial);
	}
	
	public void OnMouseExit () {
		SetMaterial (normalMaterial);
	}

	public void SetMaterial (Material nextMaterial) {
		previousMaterial = GetComponent<Renderer> ().material;
		GetComponent<Renderer> ().material = nextMaterial;
	}

	/***** Reset movement information *****/
	public void StopMoving () {
		isBeingMoved = false;
		NodeSelectionUpdate (false, this);
		NodeCreator.creator.GrandDatabase.SetNodePosition (idNumber, transform.position.x, transform.position.y, transform.position.z);
	}

	public void StartMoving() {
		if (!movementIsLocked && !doubleClicked) {
			isBeingMoved = true;
			NodeSelectionUpdate (true, this);
			SetMaterial (selectedMaterial);
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

	/*****************************/
	/*  PARENTING INFORMATION    */
	/*   - Set potential parent  */
	/*   - Set parent in db      */
	/*****************************/

	/***** If there is a drag hover over another node, save that node as a potential parent *****/
	public void SetNewPotentialParent (DragNode parentCandidate) {
		DragNode focusNode = NodeCreator.creator.currentlyFocusedNode;
		if (focusNode && focusNode.idNumber == idNumber) {
			newPotentialParent = parentCandidate;
		}
	}

	/***** If the node is hovering over another node, visually indicate that it is about to be parented *****/
	public void TriggerPotentialParentDropVisuals (bool isOn) {
		DragNode focusNode = NodeCreator.creator.currentlyFocusedNode;
		if (focusNode && focusNode.idNumber == idNumber) {
			if (isOn) {
				transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
			} else {
				transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			}
		}
	}

	public void DropIntoWormhole () {
		//set parent 
		//destroy gameobject
		NodeCreator.creator.GrandDatabase.UpdateNodeParent (this, newPotentialParent);
	}


}















