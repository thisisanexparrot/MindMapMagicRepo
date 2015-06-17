using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragConnection : MonoBehaviour {
	public DragNode node1;
	public DragNode node2;

	public ConnectionSerialized mySerialization;
	public LineRenderer myLine;

	public void InitializeConnection (DragNode n1, DragNode n2) {
		node1 = n1;
		node2 = n2;

		myLine = GetComponent<LineRenderer> ();
			
		myLine.SetPosition(0, node1.gameObject.transform.position);
		myLine.SetPosition(1, node2.gameObject.transform.position);
		myLine.SetWidth (0.05f, 0.05f);
	}

	public void SetMySerialization (ConnectionSerialized cs) {
		mySerialization = cs;
	}

	public void CreateMySerialization (DragNode origin, DragNode endpoint) {
		mySerialization = new ConnectionSerialized ();
		mySerialization.nodes = new List<NodeSerialized> ();
		mySerialization.nodes.Add (origin.mySerialization);
		mySerialization.nodes.Add (endpoint.mySerialization);
		mySerialization.thickness = 0.05f;
		mySerialization.isVisible = true;
		mySerialization.isBold = false;
		mySerialization.label = "New Connection";
	}

	void OnEnable () {
		DragNode.NodeSelectionUpdate += UpdatePosition;
		DragNode.NodeDestroyedUpdate += DestroyConnection;
	}

	void OnDisable () {
		DragNode.NodeSelectionUpdate -= UpdatePosition;
		DragNode.NodeDestroyedUpdate -= DestroyConnection;
	}

	public void UpdatePosition (bool isSelected, DragNode n) {
		if ((n.GetInstanceID () == node1.GetInstanceID ())) {
			myLine.SetPosition (0, n.gameObject.transform.position);
		} else if ((n.GetInstanceID () == node2.GetInstanceID ())) {
			myLine.SetPosition (1, n.gameObject.transform.position);
		}
	}

	public void DestroyConnection (DragNode n) {
		if ((n.GetInstanceID () == node1.GetInstanceID ()) || (n.GetInstanceID () == node2.GetInstanceID ())) {
			ConnectionHub.allConnections.Remove(this);
			Destroy(this.gameObject);
		}
	}

}
