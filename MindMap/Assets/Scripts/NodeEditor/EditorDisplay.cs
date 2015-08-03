using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorDisplay : MonoBehaviour {
	public InputField title;
	public InputField description;

	public DragNode currentlyEditedNode;
	public DragConnection currentlyEditedConnection;

	public string s_priority = "priority";
	public string s_isComplete = "isComplete";

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
		title.text = thisConnection.label;
		description.text = "Description editing is not available for connections at this time";

		currentlyEditedNode = null;
		currentlyEditedConnection = thisConnection;
	}

	void DisplayNodeInfoInEditor (bool isOn, DragNode thisNode) {
		title.text = thisNode.title;
		description.text = thisNode.description;

		currentlyEditedConnection = null;
		currentlyEditedNode = thisNode;
//		currentlyEditedNode.DisplayIngredientsOnSelect ();
	}

	/***** Edit text in Editor window *****/
	public void EditorCompleteTitleEdit () {
		if (currentlyEditedNode != null) {
			DatabaseAccess db = NodeCreator.creator.GrandDatabase;
			db.SetObjectInTable(DatabaseAccess.tn_node, currentlyEditedNode.idNumber, DatabaseAccess.node_name, title.text);

			currentlyEditedNode.title = title.text;

			InputField[] fields = currentlyEditedNode.GetComponentsInChildren<InputField>();
			foreach(InputField field in fields) {
				field.text = title.text;
			}
		} else if (currentlyEditedConnection != null) {
			currentlyEditedConnection.SetLabel(title.text, false);
		}
	}

	public void EditorCompleteDescriptionEdit () {
		Debug.Log ("Just edited the description");
		if (currentlyEditedNode != null) {
			DatabaseAccess db = NodeCreator.creator.GrandDatabase;
			db.SetObjectInTable(DatabaseAccess.tn_node, currentlyEditedNode.idNumber, DatabaseAccess.node_desc, description.text);
			currentlyEditedNode.description = description.text;
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

	/***** Add new ingredients to selected *****/
	public void AddIngredientOfTypeToSelectedNode (string newType) {
		if (currentlyEditedNode) {
			if(string.Equals(newType, s_priority)) {
				print (">>> Prio pressed...");
//				currentlyEditedNode.AddNewIngredientOfType(Ingr_Type.Priority);
			}
			else if(string.Equals(newType, s_isComplete)) {
				print (">>> Completed pressed...");
//				currentlyEditedNode.AddNewIngredientOfType(Ingr_Type.IsComplete);
			}
		}
	}

}
