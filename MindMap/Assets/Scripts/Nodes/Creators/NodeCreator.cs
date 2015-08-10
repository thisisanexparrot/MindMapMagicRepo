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

	public List<DragNode> allNodes;
	public static Dictionary<int, DragNode> allNodesDictionary;
	public DragNode currentlyFocusedNode;

	public DatabaseAccess GrandDatabase;

	public delegate void FocusedNodeUpdate (DragNode node);
	public static event FocusedNodeUpdate CurrentlyFocusedNodeUpdated;

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

		GrandDatabase.ReadNodesFromDatabase ();
		GrandDatabase.ReadConnectionsFromDatabase ();
		connectionCentralHub.InitializeLoadedConnections ();
	}

	public void CreateBaseTables () {
		GrandDatabase.CreateNewTable (DatabaseAccess.TableType.Node);
		GrandDatabase.CreateNewTable (DatabaseAccess.TableType.Connection);
		GrandDatabase.CreateNewTable (DatabaseAccess.TableType.NodeConIdentifier);
		GrandDatabase.CreateNewTable (DatabaseAccess.TableType.Meta);
		GrandDatabase.CreateNewTable (DatabaseAccess.TableType.NodeParent);
	}
	

	/********* CREATE & DESTROY **********/
	/* Create an individual node and add it to the database */
	public void SpawnNewNode (bool wasDragged) {
		GrandDatabase.IncrementIDCounter (DatabaseAccess.TableType.Node);
		int newNodeID = GrandDatabase.GetCurrentIDCount (DatabaseAccess.TableType.Node); //Increment counter, get next ID Number

		Vector3 mousePosition = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10);
		DragNode newNode = Instantiate (blankNodeTemplate, Camera.main.ScreenToWorldPoint(mousePosition), Quaternion.identity) as DragNode;
		allNodes.Add (newNode);
		allNodesDictionary.Add (newNodeID, newNode);

		GrandDatabase.AddNewNodeToDatabase (newNodeID);

		newNode.SetIDNumber (newNodeID);
		newNode.SetName (defaultName, false);
		newNode.SetDescription (defaultDesc, false);
		newNode.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
		creator.GrandDatabase.SetNodePosition (newNodeID, transform.position.x, transform.position.y, transform.position.z);
		newNode.mainCamera = Camera.main;

		newNode.GetComponent<DragNode> ().InitializeNode (this, true);
	}

	/* Load a node's data from the database */
	public void LoadNewNode (int _idNumber, string _name, string _description, float _posX, float _posY, float _posZ) {
		Vector3 mousePosition = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10);
		DragNode newNode = Instantiate (blankNodeTemplate, Camera.main.ScreenToWorldPoint(mousePosition), Quaternion.identity) as DragNode;
		allNodes.Add (newNode);
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

}









