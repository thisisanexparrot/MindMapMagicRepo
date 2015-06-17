using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorDisplay : MonoBehaviour {
	public InputField title;
	public InputField description;

	public DragNode currentlyEditedNode;

	void OnEnable () {
		DragNode.NodeSelectionUpdate += DisplayNodeInfoInEditor;
	}

	void OnDisable () {
		DragNode.NodeSelectionUpdate -= DisplayNodeInfoInEditor;
	}

	void DisplayNodeInfoInEditor (bool isOn, DragNode thisNode) {
		title.text = thisNode.mySerialization.titleName;
		description.text = thisNode.mySerialization.description;
		currentlyEditedNode = thisNode;
	}

	public void EditorCompleteTitleEdit () {
		if (currentlyEditedNode != null) {
			currentlyEditedNode.mySerialization.titleName = title.text;
			currentlyEditedNode.theCreator.Save ();
		}
	}

	public void EditorCompleteDescriptionEdit () {
		if (currentlyEditedNode != null) {
			currentlyEditedNode.mySerialization.description = description.text;
			currentlyEditedNode.theCreator.Save ();
		}
	}

}
