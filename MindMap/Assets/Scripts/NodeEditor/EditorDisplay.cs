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
		title.text = thisConnection.mySerialization.label;
		description.text = "Description editing is not available for connections at this time";

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
			InputField[] fields = currentlyEditedNode.GetComponentsInChildren<InputField>();
			foreach(InputField field in fields) {
				field.text = title.text;
			}
			currentlyEditedNode.theCreator.Save ();
		} else if (currentlyEditedConnection != null) {
			currentlyEditedConnection.mySerialization.label = title.text;
			currentlyEditedConnection.node1.theCreator.Save();
		}
	}

	public void EditorCompleteDescriptionEdit () {
		if (currentlyEditedNode != null) {
			currentlyEditedNode.mySerialization.description = description.text;
			currentlyEditedNode.theCreator.Save ();
		} else if (currentlyEditedConnection != null) {
			// To Do
		}
	}

	/***** Delete node/connection from the editor *****/
	public void DeletePressed () {
		if (currentlyEditedNode != null) {
			currentlyEditedNode.RemoveNode();
		} else if (currentlyEditedConnection != null) {
			currentlyEditedConnection.RemoveThisConnection();
		} else {
			print ("No delete");
		}
	}

}
