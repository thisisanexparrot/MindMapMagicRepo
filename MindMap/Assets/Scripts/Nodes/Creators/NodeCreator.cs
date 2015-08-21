#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class NodeCreator : MonoBehaviour {

	public static NodeCreator creator;
	public static ConnectionHub connectionCentralHub;

	public DragNode blankNodeTemplate;

	public string defaultName = "New Node";
	public string defaultDesc = "New description";

	public static Dictionary<int, DragNode> allNodesDictionary;
	public DragNode currentlyFocusedNode;
	public List<DragNode> currentlyVisibleNodes;

	public DatabaseAccess GrandDatabase;

	public delegate void FocusedNodeUpdate (DragNode node);
	public static event FocusedNodeUpdate CurrentlyFocusedNodeUpdated;

	public DragNode visibleCoreParent;

	/********* INIT  **********/
	/* Wake-up load functions */
	void Awake () {
		if (creator == null)
		{
			DontDestroyOnLoad(gameObject);
			creator = this;
		}
		else if (creator != this)
		{
			Destroy(gameObject);
		}
		if (connectionCentralHub == null) {
			connectionCentralHub = GetComponent<ConnectionHub>();
			DontDestroyOnLoad(connectionCentralHub);
		}
		allNodesDictionary = new Dictionary<int, DragNode> ();

		GrandDatabase = new DatabaseAccess ();
		GrandDatabase.OpenDatabase (DatabaseAccess.dbn_MainDatabase, this);
		CreateBaseTables ();

		connectionCentralHub.InitializeConnectionHub ();

		GetVisibleNodesWithParent (visibleCoreParent); //todo

//		GrandDatabase.ReadNodesFromDatabase (visibleCoreParent);
		GrandDatabase.ReadConnectionsFromDatabase ();
		connectionCentralHub.InitializeLoadedConnections ();

	}

	/*public void TEMPSetupParentTable () {
		foreach (KeyValuePair<int, DragNode> kvp in allNodesDictionary) {
			GrandDatabase.AddNodeToParentTable(kvp.Value, visibleCoreParent);
		}
	}*/

	public void CreateBaseTables () {
		GrandDatabase.CreateNewTable (DatabaseAccess.TableType.Node);
		GrandDatabase.CreateNewTable (DatabaseAccess.TableType.Connection);
		GrandDatabase.CreateNewTable (DatabaseAccess.TableType.NodeConIdentifier);
		GrandDatabase.CreateNewTable (DatabaseAccess.TableType.Meta);
		GrandDatabase.CreateNewTable (DatabaseAccess.TableType.NodeParent);
		GrandDatabase.CreateInitialCounterRow ();
	}
	

	/********* CREATE & DESTROY **********/
	/* Create an individual node and add it to the database */
	public void SpawnNewNode (bool wasDragged) {
		GrandDatabase.IncrementIDCounter (DatabaseAccess.TableType.Node);
		int newNodeID = GrandDatabase.GetCurrentIDCount (DatabaseAccess.TableType.Node); //Increment counter, get next ID Number

		Vector3 mousePosition = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10);
		DragNode newNode = Instantiate (blankNodeTemplate, Camera.main.ScreenToWorldPoint(mousePosition), Quaternion.identity) as DragNode;
		allNodesDictionary.Add (newNodeID, newNode);

		GrandDatabase.AddNewNodeToDatabase (newNodeID);

		newNode.SetIDNumber (newNodeID);
		newNode.SetName (defaultName, false);
		newNode.SetDescription (defaultDesc, false);
		newNode.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
		creator.GrandDatabase.SetNodePosition (newNodeID, transform.position.x, transform.position.y, transform.position.z);
		newNode.mainCamera = Camera.main;

		newNode.GetComponent<DragNode> ().InitializeNode (this, true);

		GrandDatabase.AddNodeToParentTable(newNode, visibleCoreParent);
	}

	/* Load a node's data from the database */
	public void LoadNewNode (int _idNumber, string _name, string _description, float _posX, float _posY, float _posZ) {
		Vector3 mousePosition = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10);
		DragNode newNode = Instantiate (blankNodeTemplate, Camera.main.ScreenToWorldPoint(mousePosition), Quaternion.identity) as DragNode;
		allNodesDictionary.Add (_idNumber, newNode);

		newNode.SetName (_name, true);
		newNode.SetDescription (_description, true);
		newNode.SetIDNumber (_idNumber);
		newNode.transform.position = new Vector3 (_posX, _posY, _posZ);
		newNode.mainCamera = Camera.main;
	}

	/* Destroy a node and remove it from the database */
	public void RemoveNode (DragNode destroyThis) {
		destroyThis.DestroyThisNode ();
		GrandDatabase.RemoveNodeFromDatabase (destroyThis.idNumber);
		allNodesDictionary.Remove (destroyThis.idNumber);
		Destroy (destroyThis.gameObject);
	}

	/* Update the local pointer to the focused node and sends a global event */
	public void UpdateCurrentlySelectedNode (DragNode newNode) {
		currentlyFocusedNode = newNode;
		CurrentlyFocusedNodeUpdated (newNode);
	}

	/********* WORMHOLE HANDLERS **********/
	/* */
	public void EnterWormhole (DragNode wormholeNode) {
		visibleCoreParent = wormholeNode;
		print ("New wormhole: " + visibleCoreParent.title);
		GetVisibleNodesWithParent (visibleCoreParent);
	}

	/* */
	public void ExitWormhole () {
		//Todo: connect to a button 
		if (visibleCoreParent) {
			//get parent of visibleCoreParent
			//GetVisibleNodesWithParent for new visibleCoreParent
		}
	}

	public void GetVisibleNodesWithParent (DragNode parentNode) {
		//THIS NEXT: CALL THE DATABASE THING AND DESTROY THE NODES!!!!!!!
		//TODO: [900] 
		//Populate parent table
		//If node.parent = parentNode, it is visible
		//Otherwise, it is disabled
		//Set currentlyVisibleNodes to whatever this new list is
		if (allNodesDictionary != null) {
			foreach (KeyValuePair<int, DragNode> kvp in allNodesDictionary) {
				DragNode n = kvp.Value;
				Debug.Log(n.title);
				Destroy(n.gameObject);
			}
		}

		GrandDatabase.ReadNodesFromDatabase (visibleCoreParent);

	}
}









