using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class NodeCreator : MonoBehaviour {
	public Node blankNodeTemplate;
	public NodeList saveNodesList;

	void OnEnable () {
		saveNodesList = AssetDatabase.LoadAssetAtPath ("Assets/NodeList.asset", typeof(NodeList)) as NodeList;
	}

	public void SpawnNewNode () {
		Vector3 mousePosition = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10);
		Node newNode = Instantiate (blankNodeTemplate, Camera.main.ScreenToWorldPoint(mousePosition), Quaternion.identity) as Node;
		NodeSerialized newSerialized = CreateSerializeNode ();

		newNode.GetComponent<DragNode> ().InitializeNode (newSerialized, this);
		saveNodesList.nodeCounter++;
	}

	/* Create a node for storage */
	NodeSerialized CreateSerializeNode () {
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
