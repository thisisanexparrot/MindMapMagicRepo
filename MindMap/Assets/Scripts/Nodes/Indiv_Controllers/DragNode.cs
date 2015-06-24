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

	/***** Materials *****/
	public Material normalMaterial;
	public Material hoverMaterial;
	public Material selectedMaterial;
	public Material previousMaterial;

	/***** Lerp variables *****/
	public float originalScale = 1.0f;
	public float hoverScale = 1.2f;
	public bool movementIsLocked = false;
	Camera mainCamera;

	/*****************/
	/*   FUNCTIONS   */
	/*****************/

	/***** Initialization based on whether the button was clicked or dragged *****/
	public void InitializeNode (NodeSerialized newSerialize, NodeCreator creator, bool isNew) {
		previousMaterial = normalMaterial;
		theCreator = creator;
		mySerialization = newSerialize;
		if (isNew) {
			StartMoving();
		} 
		else {
			StopMoving();
		}
		ResetOffset ();
		mainCamera = Camera.main;
		CreateIngredientsList ();
	}

	void CreateIngredientsList () {
		if (mySerialization.ingredients == null) {
			print ("No list yet");
			mySerialization.ingredients = new List<Ingredient>();
			//Ingr_Priority newP = new Ingr_Priority();
			//newP.myPriority = 3;
			//mySerialization.ingredients.Add(newP);
		}
		print ("Ingredients list added successfully");
	}

	/***** Update based on whether or not node is moving/clicked *****/
	void Update () {
		/*if(mySerialization.ingredients != null) {
			if (mySerialization.ingredients.Count > 0) {
				Ingr_Priority nextP = (Ingr_Priority)mySerialization.ingredients [0];
				print (nextP.myPriority + " is my priority");
			}

			if (mySerialization.ingredients.Count > 1) {
				Ingr_IsCompleted nextC = (Ingr_IsCompleted)mySerialization.ingredients [1];
				print (nextC.isComplete + " status");
			}
		}*/
		if (isBeingMoved) {
			DragNodeInSpace();
			CheckMoveZSpace ();
		}
		if (Input.GetMouseButtonUp (0) && mySerialization.isSelected) {
			StopMoving();
		}
		if (mainCamera) {
			transform.LookAt (transform.position + mainCamera.transform.rotation * Vector3.forward,
			                 mainCamera.transform.rotation * Vector3.up);
		}
	}

	void SetMovementLock (bool newIsLocked) {
		movementIsLocked = newIsLocked;
	}

	void OnEnable () {
		MouseOrbitImproved.completeMove += SetMovementLock;
	}

	void OnDisable () {
		MouseOrbitImproved.completeMove -= SetMovementLock;
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
			//print ("*** DOUBLE clicked");
			doubleClicked = true;
			SetMovementLock (true);
			Camera.main.GetComponent<MouseOrbitImproved>().SetTarget(gameObject);
			doubleClicked = false;
		} else {
			//print ("Single click");
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

	public void OnMouseEnter () {
		SetMaterial (hoverMaterial);
	}
	
	public void OnMouseExit () {
		if (!mySerialization.isSelected) {
			SetMaterial (normalMaterial);
		}
	}

	public void SetMaterial (Material nextMaterial) {
		previousMaterial = GetComponent<Renderer> ().material;
		GetComponent<Renderer> ().material = nextMaterial;
	}

	/***** Reset movement information *****/
	void StopMoving () {
		//print ("Stop moving");
		isBeingMoved = false;
		theCreator.Vector3ToFloats (mySerialization, transform.position);
		mySerialization.isSelected = false;

		theCreator.Save ();
		NodeSelectionUpdate (false, this);
		if (!mySerialization.isSelected) {
			SetMaterial (previousMaterial);
		}
	}

	void StartMoving() {
		if (!movementIsLocked && !doubleClicked) {
			//print ("Start moving");
			isBeingMoved = true;
			mySerialization.isSelected = true;
			NodeSelectionUpdate (true, this);
			SetMaterial (selectedMaterial);

			/*print ("TEMP: ADDING");
			Ingr_IsCompleted newI = new Ingr_IsCompleted();
			newI.isComplete = true;
			if(mySerialization.ingredients != null) {
				mySerialization.ingredients.Add(newI);
				print ("ADDED COMPLETE");
			}*/
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

	/*****  Add an ingredient *****/
	public void AddNewIngredientOfType (Ingr_Type newIngrType) {
		if(!IngredientExists(newIngrType)) {
			if (newIngrType == Ingr_Type.IsComplete) {
				print ("New type of IsComplete");
				Ingr_IsCompleted my_IC = new Ingr_IsCompleted();
				my_IC.myType = Ingr_Type.IsComplete;
				mySerialization.ingredients.Add(my_IC);
			} 

			else if (newIngrType == Ingr_Type.Priority) {
				print ("New type of Priority");
				Ingr_Priority my_Prio = new Ingr_Priority();
				my_Prio.myType = Ingr_Type.Priority;
				mySerialization.ingredients.Add(my_Prio);
			}
		}

	}

	bool IngredientExists (Ingr_Type thisIngrType) {
		foreach (Ingredient ingNext in mySerialization.ingredients) {
			if(ingNext.myType == thisIngrType) {
				print ("Nope, already exists for this node.");
				return true;
			}
		}
		return false;
	}

	public void DisplayIngredientsOnSelect () {

	}




}















