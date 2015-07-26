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
//	public NodeSerialized mySerialization;
	public NodeCreator theCreator;

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
	Camera mainCamera;

	/*****************/
	/*   FUNCTIONS   */
	/*****************/

	/***** Initialization based on whether the button was clicked or dragged *****/
//	public void InitializeNode (NodeSerialized newSerialize, NodeCreator creator, bool isNew) {
//		previousMaterial = normalMaterial;
//		theCreator = creator;
//		mySerialization = newSerialize;
//		if (isNew) {
//			StartMoving();
//		} 
//		else {
//			StopMoving();
//		}
//		ResetOffset ();
//		mainCamera = Camera.main;
//		CreateIngredientsList ();
//
//		foreach (Ingredient i in mySerialization.ingredients) {
//			InitializeIngredientDisplayOfType(i.myType);
//			print (">>>> " + i.myType);
//		}
//	}

//	void CreateIngredientsList () {
//		if (mySerialization.ingredients == null) {
//			print ("No list yet");
//			mySerialization.ingredients = new List<Ingredient>();
//		}
//		//print ("Ingredients list added successfully");
//	}

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

		if (Input.GetKeyDown (KeyCode.T)) {
			print ("HERE ARE YOUR INGREDIENTS: ");
//			foreach(Ingredient ingr in mySerialization.ingredients) {
//				print (ingr.myType);
//			}
		}
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

	public void SetIDNumber (int newIDNumber) {
		idNumber = newIDNumber;
	}

	public void SetName (string newTitle) {
		title = newTitle;
//		DatabaseUtils.UpdateColumn_DB(theCreator.graphDatabase, theCreator.nodeTableName, "Name", title, "string", "idNumber", "=", mySerialization.idNumber.ToString());
	}

	public void SetDescription (string newDescription) {
		description = newDescription;
		//		DatabaseUtils.UpdateColumn_DB(theCreator.graphDatabase, theCreator.nodeTableName, "Name", title, "string", "idNumber", "=", mySerialization.idNumber.ToString());
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
			doubleClicked = true;
			SetMovementLock (true);
			Camera.main.GetComponent<MouseOrbitImproved>().SetTarget(gameObject);
			doubleClicked = false;
		} else {
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
//		if (!mySerialization.isSelected) {
//			SetMaterial (normalMaterial);
//		}
	}

	public void SetMaterial (Material nextMaterial) {
		previousMaterial = GetComponent<Renderer> ().material;
		GetComponent<Renderer> ().material = nextMaterial;
	}

	/***** Reset movement information *****/
	void StopMoving () {
		isBeingMoved = false;
//		theCreator.Vector3ToFloats (mySerialization, transform.position);
//		mySerialization.isSelected = false;

//		theCreator.Save ();
		NodeSelectionUpdate (false, this);
//		if (!mySerialization.isSelected) {
//			SetMaterial (previousMaterial);
//		}
		print ("Stopped moving");
//		DatabaseUtils.UpdateColumn_DB(theCreator.graphDatabase, theCreator.nodeTableName, "locationX", transform.position.x.ToString(), "float", "idNumber", "=", mySerialization.idNumber.ToString());
//		DatabaseUtils.UpdateColumn_DB(theCreator.graphDatabase, theCreator.nodeTableName, "locationY", transform.position.y.ToString(), "float", "idNumber", "=", mySerialization.idNumber.ToString());
//		DatabaseUtils.UpdateColumn_DB(theCreator.graphDatabase, theCreator.nodeTableName, "locationZ", transform.position.z.ToString(), "float", "idNumber", "=", mySerialization.idNumber.ToString());
	}

	void StartMoving() {
		if (!movementIsLocked && !doubleClicked) {
			isBeingMoved = true;
//			mySerialization.isSelected = true;
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

	/*****  Add an ingredient *****/
//	public void AddNewIngredientOfType (Ingr_Type newIngrType) {
//		if(!IngredientExists(newIngrType)) {
//			if (newIngrType == Ingr_Type.IsComplete) {
//				print ("New type of IsComplete");
//				Ingr_IsCompleted my_IC = new Ingr_IsCompleted();
//				my_IC.myType = Ingr_Type.IsComplete;
//				print ("TYPE: " + my_IC.myType);
//				mySerialization.ingredients.Add(my_IC);
//				mySerialization.ingredients[0].myType = Ingr_Type.IsComplete;
//				print ("SERI TYPE: " + mySerialization.ingredients[0].myType);
//
//				InitializeIngredientDisplayOfType(Ingr_Type.IsComplete);
//				//initialize the objct of type and display
//			} 
//
//			else if (newIngrType == Ingr_Type.Priority) {
//				print ("New type of Priority");
//				Ingr_Priority my_Prio = new Ingr_Priority();
//				my_Prio.myType = Ingr_Type.Priority;
//				print ("TYPE: " + my_Prio.myType);
//				mySerialization.ingredients.Add(my_Prio);
//
//				InitializeIngredientDisplayOfType(Ingr_Type.Priority);
//			}
////			theCreator.Save();
//			print ("## SAVED ##");
////			theCreator.Load();
//			print ("## LOADED ##");
//		}
//
//	}

	void InitializeIngredientDisplayOfType (Ingr_Type thisIngrType) {
		print ("Begin display...");
		switch (thisIngrType) {
		case Ingr_Type.Priority: 
			GameObject newSuper_Prio = new GameObject("d_Ingr_Prio", typeof(SupervisePriority));
			newSuper_Prio.transform.parent = this.transform;

			newSuper_Prio.GetComponent<SupervisePriority>().SelectedNodeDisplay(this); //Delete this soon

			break;
		case Ingr_Type.IsComplete:
			GameObject newSuper_Comp = new GameObject("d_Ingr_Comp", typeof(SuperviseCompleted));
			newSuper_Comp.gameObject.transform.parent = this.transform;

			newSuper_Comp.GetComponent<SuperviseCompleted>().SelectedNodeDisplay(this); //Delete this soon

			break;
		default:
			break;
		}
	}

//	bool IngredientExists (Ingr_Type thisIngrType) {
//		foreach (Ingredient ingNext in mySerialization.ingredients) {
//			if(ingNext.myType == thisIngrType) {
//				print ("Nope, already exists for this node.");
//				return true;
//			}
//		}
//		return false;
//	}

//	public void DisplayIngredientsOnSelect () {
//		if (mySerialization.ingredients != null) {
//			foreach(Ingredient i in mySerialization.ingredients) {
//				/* We should create a new instance of each type of ingredient from the new classes and parent it to the node if there isn't one there already. we should then run
//				 the display method to show some visible effect. this visible effect should be edited via that instance. These instances should perhaps be kept in a list here on this node; 
//				 a separate list, remember, from the one in the serialized object. */
//			}
//		}
//	}

//	public Ingredient GetIngredientFromSerialized (Ingr_Type searchType) {
//		foreach (Ingredient i in mySerialization.ingredients) {
//			if(i.myType == searchType) {
//				print ("Found the type yo'ure looking for");
//				return i;
//			}
//		}
//		print ("This is not the type you are looking for.");
//		return null;
//	}




}















