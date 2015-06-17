using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorDisplay : MonoBehaviour {
	public InputField title;
	public InputField description;

	void OnEnable () {
		DragNode.NodeSelectionUpdate += DisplayNodeInfoInEditor;
	}

	void OnDisable () {
		DragNode.NodeSelectionUpdate -= DisplayNodeInfoInEditor;
	}

	void DisplayNodeInfoInEditor (bool isOn, DragNode thisNode) {
		title.text = thisNode.mySerialization.titleName;
		description.text = thisNode.mySerialization.description;
	}
}
