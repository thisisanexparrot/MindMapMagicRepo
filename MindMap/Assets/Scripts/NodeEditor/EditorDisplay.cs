using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorDisplay : MonoBehaviour {
	public InputField title;
	public InputField description;

	public DragNode currentlyEditedNode;
	public DragConnection currentlyEditedConnection;

	/***** Enable/Disable event listeners *****/
	void OnEnable () {
		DragNode.NodeSelectionUpdate += DisplayNodeInfoInEditor;
		DragConnection.ConnectionSelectedUpdate += DisplayConnectionInfoEditor;
	}

	void OnDisable () {
		DragNode.NodeSelectionUpdate -= DisplayNodeInfoInEditor;
		DragConnection.ConnectionSelectedUpdate -= DisplayConnectionInfoEditor;
	}

	/***** Update which node/connection is currently being edited *****/
	void DisplayConnectionInfoEditor (DragConnection thisConnection) {
		print ("Display update to connection!");
		title.text = thisConnection.mySerialization.label;
		description.text = "No Description";

		currentlyEditedNode = null;
		currentlyEditedConnection = thisConnection;
	}

	void DisplayNodeInfoInEditor (bool isOn, DragNode thisNode) {
		title.text = thisNode.mySerialization.titleName;
		description.text = thisNode.mySerialization.description;

		currentlyEditedConnection = null;
		currentlyEditedNode = thisNode;
	}

	/***** Edit text in Editor window *****/
	public void EditorCompleteTitleEdit () {
		if (currentlyEditedNode != null) {
			currentlyEditedNode.mySerialization.titleName = title.text;
			currentlyEditedNode.theCreator.Save ();
		} else if (currentlyEditedConnection != null) {

		}
	}

	public void EditorCompleteDescriptionEdit () {
		if (currentlyEditedNode != null) {
			currentlyEditedNode.mySerialization.description = description.text;
			currentlyEditedNode.theCreator.Save ();
		} else if (currentlyEditedConnection != null) {

		}
	}

}
