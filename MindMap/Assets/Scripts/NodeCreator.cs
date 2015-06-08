using UnityEngine;
using System.Collections;

public class NodeCreator : MonoBehaviour {
	public Node blankNodeTemplate;

	private bool isBeingDragged;

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


}
