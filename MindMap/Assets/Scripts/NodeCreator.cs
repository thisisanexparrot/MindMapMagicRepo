using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class NodeCreator : MonoBehaviour {
	public Node blankNodeTemplate;
	public NodeList saveNodesList;

	private bool isBeingDragged;

	void OnEnable () {
		saveNodesList = AssetDatabase.LoadAssetAtPath ("Assets/NodeList.asset", typeof(NodeList)) as NodeList;
	}

	public void SpawnNewNodeClick () {
		isBeingDragged = false;
		SpawnNewNode ();
	}

	public void SpawnNewNodeDrag () {
		isBeingDragged = true;
		SpawnNewNode ();
	}

	public void SpawnNewNode () {
		Vector3 mousePosition = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10);
		Node newNode = Instantiate (blankNodeTemplate, Camera.main.ScreenToWorldPoint(mousePosition), Quaternion.identity) as Node;
		newNode.GetComponent<DragNode> ().Initialize (isBeingDragged);
	}

	void SerializeNode () {
		NodeSerialized newNode = new NodeSerialized ();
		newNode.name = "This is a new name";

	}


}
