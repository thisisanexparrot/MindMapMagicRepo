using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class NodeCreator : MonoBehaviour {
	public Node blankNodeTemplate;
	public NodeList saveNodesList;

	/* Wake-up load functions */
	void OnEnable () {
		saveNodesList = AssetDatabase.LoadAssetAtPath ("Assets/NodeList.asset", typeof(NodeList)) as NodeList;
		LoadNodesFromSerialized ();
	}

	void LoadNodesFromSerialized () {
		foreach (NodeSerialized nextNode in saveNodesList.nodeList) {
			print ("loading!");
			Vector3 nextPosition = nextNode.location;
			Node newNode = Instantiate (blankNodeTemplate, nextPosition, Quaternion.identity) as Node;
			newNode.GetComponent<DragNode> ().InitializeNode (nextNode, this, false);
		}
	}


	/* Create & Remove individual nodes for storage */
	public void SpawnNewNode () {
		Vector3 mousePosition = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10);
		Node newNode = Instantiate (blankNodeTemplate, Camera.main.ScreenToWorldPoint(mousePosition), Quaternion.identity) as Node;
		NodeSerialized newSerialized = CreateNewSerializeNode ();
		
		newNode.GetComponent<DragNode> ().InitializeNode (newSerialized, this, true);
		saveNodesList.nodeCounter++;
	}

	NodeSerialized CreateNewSerializeNode () {
		NodeSerialized newNode = new NodeSerialized ();
		newNode.titleName = "This is a new name";
		newNode.isSelected = false;
		newNode.idNumber = saveNodesList.nodeCounter;
		saveNodesList.nodeList.Add (newNode);

		AssetDatabase.SaveAssets ();
		
		return newNode;
	}

	public void RemoveNode (DragNode destroyThis) {
		NodeSerialized destroyID = destroyThis.mySerialization;
		saveNodesList.nodeList.Remove(destroyID);
		Destroy (destroyThis.gameObject);
	}


}
