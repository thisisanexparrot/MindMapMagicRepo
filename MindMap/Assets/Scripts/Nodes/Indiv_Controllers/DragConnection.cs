using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class DragConnection : MonoBehaviour {
	public delegate void ConnectionSelected (DragConnection con);
	public static event ConnectionSelected ConnectionSelectedUpdate;

	public ConnectionCollide connectionColliderTemplate;
	public ConnectionCollide myColliderObject;

	public DragNode node1;
	public DragNode node2;

	public int idNumber;
	public string label;
	public float thickness;
	public bool isVisible;

	public LineRenderer myLine;


	public void InitializeConnection (DragNode n1, DragNode n2) {
		node1 = n1;
		node2 = n2;

		myLine = GetComponent<LineRenderer> ();
			
		myLine.SetPosition(0, node1.gameObject.transform.position);
		myLine.SetPosition(1, node2.gameObject.transform.position);
		myLine.SetWidth (0.05f, 0.05f);

		myColliderObject = Instantiate (connectionColliderTemplate);
		myColliderObject.InitializeCollider (node1, node2, this);
	}

	public void TriggerConnectionEdit () {
		ConnectionSelectedUpdate (this);
		NodeCreator.creator.UpdateCurrentlySelectedNode (null);
	}

	public void SetIDNumber (int newID) {
		idNumber = newID;
	}

	public void SetLabel (string newLabel, bool isFirstLoad) {
		label = newLabel;
		if (!isFirstLoad) {
			DatabaseAccess db = NodeCreator.creator.GrandDatabase;
			db.SetObjectInTable (DatabaseAccess.tn_connection, idNumber, DatabaseAccess.con_label, newLabel);
		}
	}

	public void SetThickness (float newThickness, bool isFirstLoad) {
		thickness = newThickness;
		if (!isFirstLoad) {
			DatabaseAccess db = NodeCreator.creator.GrandDatabase;
			db.SetObjectInTable (DatabaseAccess.tn_connection, idNumber, DatabaseAccess.con_thickness, newThickness);
		}
	}

	public void SetVisibility (bool newVisibility, bool isFirstLoad) {
		isVisible = newVisibility;
		if (!isFirstLoad) {
			DatabaseAccess db = NodeCreator.creator.GrandDatabase;
			db.SetObjectInTable (DatabaseAccess.tn_connection, idNumber, DatabaseAccess.con_isVisible, newVisibility);
		}
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
			myColliderObject.UpdateColliderTransform(node1, node2);
		} else if ((n.GetInstanceID () == node2.GetInstanceID ())) {
			myLine.SetPosition (1, n.gameObject.transform.position);
			myColliderObject.UpdateColliderTransform(node1, node2);
		}
	}

	/***** Destroy connection *****/
	public void DestroyConnection (DragNode n) {
		if ((n.GetInstanceID () == node1.GetInstanceID ()) || (n.GetInstanceID () == node2.GetInstanceID ())) {
			RemoveThisConnection();
		}
	}

	public void RemoveThisConnection () {
		ConnectionHub.allConnections.Remove(this);
		Destroy(this.gameObject);
	}

}
