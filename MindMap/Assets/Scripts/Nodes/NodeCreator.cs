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

	public delegate void InitLoad ();
	public static event InitLoad LoadCompleted;

	public DragNode blankNodeTemplate;
	public NodeListAndSavedData saveNodesList;

	//public int localNodeCounter;
	public List<NodeSerialized> localNodeList;
	public List<ConnectionSerialized> tempConnectionList;
	public List<DragNode> allNodes;

	string playerPath = "/playerInfo12.dat";

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
	}

	/* Broadcasts event of initial load */
	void OnEnable () {
		Load ();
		if (LoadCompleted != null) {
			LoadCompleted ();
		}
	}


	/********* CREATE **********/
	/* Create & Remove individual nodes for storage */
	public void SpawnNewNode () {
		Vector3 mousePosition = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10);
		DragNode newNode = Instantiate (blankNodeTemplate, Camera.main.ScreenToWorldPoint(mousePosition), Quaternion.identity) as DragNode;
		NodeSerialized newSerialized = CreateNewSerializeNode ();
		
		newNode.GetComponent<DragNode> ().InitializeNode (newSerialized, this, true);
		saveNodesList.nodeCounter += 1;


		Save ();
	}

	NodeSerialized CreateNewSerializeNode () {
		NodeSerialized newNode = new NodeSerialized ();
		newNode.titleName = "This is a new name";
		newNode.isSelected = false;
		newNode.idNumber = saveNodesList.nodeCounter;
		localNodeList.Add (newNode);

		print ("NEW NODE! Number: " + newNode.idNumber);
		
		return newNode;
	}

	public void RemoveNode (DragNode destroyThis) {
		NodeSerialized destroyID = destroyThis.mySerialization;
		destroyThis.DestroyThisNode ();
		localNodeList.Remove (destroyID);
		Destroy (destroyThis.gameObject);
		print ("***SAVE***** (removenode)");
		Save ();
	}

	/********* SAVE **********/
	/* Save and Load Methods */
	public void Save () {
		print ("INITIALIZE SAVE!");
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + playerPath);

		if (saveNodesList == null) {
			print ("SAVED LIST WAS NULL ALL ALONG");
			saveNodesList = new NodeListAndSavedData ();
			saveNodesList.nodeCounter = 0;
			saveNodesList.connectionList = new List<ConnectionSerialized> ();
		} else {
			print ("~~~~~~ NAH, it's cool.");
			saveNodesList.connectionList = connectionCentralHub.CreateSaveList (); // <---- Here's the problem: the list is getting saved over before it can be loaded because "save" is called before load can finish.

		}
		saveNodesList.nodeList = localNodeList;
		//NodeListAndSavedData data = new NodeListAndSavedData ();
		//data.nodeCounter = 0;
		//if(
		//data.nodeList = localNodeList;
		//data.connectionList = connectionCentralHub.CreateSaveList();
		//print ("Saved with " + data.connectionList.Count + " total connections.");
		print ("Saved with " + saveNodesList.connectionList.Count + " total Connections");
		//data.nodeCounter = localNodeCounter;


		//bf.Serialize (file, data);
		bf.Serialize (file, saveNodesList);
		file.Close ();
		//print ("Saved!");
		/* Reminder: This probably happens a lot more than it needs to; come fix it later. */
	}

	public void Load () {
		print (" ### BEING ### MASTER Loading...");
		if (File.Exists (Application.persistentDataPath + playerPath))
		{
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + playerPath, FileMode.Open);
			NodeListAndSavedData data = (NodeListAndSavedData)bf.Deserialize (file);
			file.Close ();

			saveNodesList = data;
			localNodeList = data.nodeList;
			tempConnectionList = data.connectionList;

			print("<<<ORIGINAL LENGTH of connection list>>>>>: " + data.connectionList.Count);
			//print ("CURRENT FUCKING NUMBER: " + saveNodesList.nodeCounter);
			//localNodeCounter = data.nodeCounter;

			LoadNodesFromSerialized ();
			print("<<<ORIGINAL LENGTH of connection list 2>>>>>: " + data.connectionList.Count);

			connectionCentralHub.LoadConnectionsFromFile(tempConnectionList);


		}
		else
		{
			print ("File with name not found.");
		}
		print ("Master load");
	}
	
	void LoadNodesFromSerialized () {
		print ("<<<<<1.1 CHECK: " + saveNodesList.connectionList.Count);

		foreach (NodeSerialized nextNode in localNodeList) {
			Vector3 nextPosition = FloatsToVector3(nextNode);
			DragNode newNode = Instantiate (blankNodeTemplate, nextPosition, Quaternion.identity) as DragNode;
			print ("<<<<<1.2 CHECK: " + saveNodesList.connectionList.Count);
			newNode.GetComponent<DragNode> ().InitializeNode (nextNode, this, false);
			allNodes.Add(newNode);
			print ("<<<<<1.3 CHECK: " + saveNodesList.connectionList.Count);

		}
	}

	/********* UTILS **********/
	/* Converters for Vector3 Serialization in NodeSerialized */
	public void Vector3ToFloats (NodeSerialized n, Vector3 input) {
		n.locationX = input.x;
		n.locationY = input.y;
		n.locationZ = input.z;
	}

	public Vector3 FloatsToVector3 (NodeSerialized n) {
		Vector3 newVector = new Vector3 (n.locationX, n.locationY, n.locationZ);
		return newVector;
	}


}









