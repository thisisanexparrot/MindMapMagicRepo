﻿#if UNITY_EDITOR
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

	string playerPath = "/playerInfo21.dat";

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
		connectionCentralHub.InitializeConnectionHub ();
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

		return newNode;
	}

	public void RemoveNode (DragNode destroyThis) {
		NodeSerialized destroyID = destroyThis.mySerialization;
		destroyThis.DestroyThisNode ();
		localNodeList.Remove (destroyID);
		Destroy (destroyThis.gameObject);
		Save ();
	}

	/********* SAVE **********/
	/* Save and Load Methods */
	public void Save () {
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + playerPath);

		if (saveNodesList == null) {
			saveNodesList = new NodeListAndSavedData ();
			saveNodesList.nodeCounter = 0;
			saveNodesList.connectionList = new List<ConnectionSerialized> ();
		} else {
			saveNodesList.connectionList = connectionCentralHub.CreateSaveList (); // <---- Here's the problem: the list is getting saved over before it can be loaded because "save" is called before load can finish.
		}
		saveNodesList.nodeList = localNodeList;

		bf.Serialize (file, saveNodesList);
		file.Close ();
		/* Reminder: This probably happens a lot more than it needs to; come fix it later. */
	}

	public void Load () {
		if (File.Exists (Application.persistentDataPath + playerPath))
		{
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + playerPath, FileMode.Open);
			NodeListAndSavedData data = (NodeListAndSavedData)bf.Deserialize (file);
			file.Close ();

			saveNodesList = data;
			localNodeList = data.nodeList;
			tempConnectionList = data.connectionList;

			LoadNodesFromSerialized ();
			connectionCentralHub.LoadConnectionsFromFile(tempConnectionList);
		}
		else
		{
			print ("File with name not found.");
		}
	}
	
	void LoadNodesFromSerialized () {
		foreach (NodeSerialized nextNode in localNodeList) {
			Vector3 nextPosition = FloatsToVector3(nextNode);
			DragNode newNode = Instantiate (blankNodeTemplate, nextPosition, Quaternion.identity) as DragNode;
			newNode.GetComponent<DragNode> ().InitializeNode (nextNode, this, false);
			allNodes.Add(newNode);
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








