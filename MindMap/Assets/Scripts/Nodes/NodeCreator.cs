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

	public DragNode blankNodeTemplate;
	public NodeListAndSavedData saveNodesList;

	public int localNodeCounter;
	public List<NodeSerialized> localNodeList;

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
	}

	void OnEnable () {
		Load ();
	}



	/* Create & Remove individual nodes for storage */
	public void SpawnNewNode () {
		Vector3 mousePosition = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10);
		DragNode newNode = Instantiate (blankNodeTemplate, Camera.main.ScreenToWorldPoint(mousePosition), Quaternion.identity) as DragNode;
		NodeSerialized newSerialized = CreateNewSerializeNode ();
		
		newNode.GetComponent<DragNode> ().InitializeNode (newSerialized, this, true);
		localNodeCounter++;

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
		localNodeList.Remove (destroyID);
		Destroy (destroyThis.gameObject);
		Save ();
	}

	/* Save and Load Methods */
	public void Save () {
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/playerInfo.dat");

		NodeListAndSavedData data = new NodeListAndSavedData ();
		data.nodeList = localNodeList;
		data.nodeCounter = localNodeCounter;

		bf.Serialize (file, data);
		file.Close ();
	}

	public void Load () {
		if (File.Exists (Application.persistentDataPath + "/playerInfo.dat"))
		{
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			NodeListAndSavedData data = (NodeListAndSavedData)bf.Deserialize (file);
			file.Close ();

			localNodeList = data.nodeList;
			localNodeCounter = data.nodeCounter;

			LoadNodesFromSerialized ();
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
		}
	}

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









