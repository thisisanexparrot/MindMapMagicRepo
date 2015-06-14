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

	void OnEnable () {
		DragNode.NodeSelectionUpdate += UpdatePosition;
		DragNode.NodeDestroyedUpdate += DestroyConnection;
	}

	void OnDisable () {
		DragNode.NodeSelectionUpdate -= UpdatePosition;
		DragNode.NodeDestroyedUpdate -= DestroyConnection;
	}

	public void UpdatePosition (bool isSelected, DragNode n) {
		DragNode followNode = null;

		if ((n.GetInstanceID () == node1.GetInstanceID ())) {
			myLine.SetPosition (0, n.gameObject.transform.position);
		} else if ((n.GetInstanceID () == node2.GetInstanceID ())) {
			myLine.SetPosition (1, n.gameObject.transform.position);
		}
	}

	public void DestroyConnection (DragNode n) {
		if ((n.GetInstanceID () == node1.GetInstanceID ()) || (n.GetInstanceID () == node2.GetInstanceID ())) {
			Destroy(this.gameObject);
		}
	}

	/* NEED MORE FOR THIS */
	public void CreateConnectionSerialization () {
		ConnectionSerialized c = new ConnectionSerialized ();


		mySerialization = c;
	}
}
