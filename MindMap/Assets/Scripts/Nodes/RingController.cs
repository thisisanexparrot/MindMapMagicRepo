using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class RingController : MonoBehaviour {
	public Material normalStateMaterial;
	public Material selectedStateMaterial;

	void OnEnable () {
		DragNode.NodeSelectionUpdate += UpdateMaterial;
	}

	void OnDisable () {
		DragNode.NodeSelectionUpdate -= UpdateMaterial;
	}

	void UpdateMaterial (bool isSelected) {
		if (isSelected) {
			GetComponent<Renderer>().material = selectedStateMaterial;
		} else {
			GetComponent<Renderer>().material = normalStateMaterial;
		}
	}

	void OnMouseDown () {
		print ("Clicked on ring");
	}

	void OnMouseUp () {
		print ("Clicked on ring");
	}
}
