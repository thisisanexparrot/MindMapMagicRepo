using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitleHandler : MonoBehaviour {
	public string title;
	public DragNode parentNode;


	void OnEnable () {
		NodeCreator.LoadCompleted += LoadTitleFromSave;
		if (parentNode == null) {
			Transform nextParent = transform.parent;
			while (!nextParent.CompareTag("Node")) {
				nextParent = nextParent.transform.parent;
			}
			SetParentNode (nextParent.GetComponent<DragNode> ());
		}
	}

	void OnDisable () {
		NodeCreator.LoadCompleted -= LoadTitleFromSave;
	}

	public void LoadTitleFromSave () {
		if (parentNode) {
			string savedText = parentNode.mySerialization.titleName;
			print (savedText);
			gameObject.GetComponent<InputField>().text = savedText;
		}
	}

	public void DoneEditing () {
		UpdateTitle (gameObject.GetComponent<InputField> ().textComponent.text);
	}

	public void SetParentNode(DragNode pNode) {
		parentNode = pNode;
		if (parentNode.theCreator != null) {
			parentNode.theCreator.Save ();
		}
	}

	public void UpdateTitle(string newTitle) {
		title = newTitle;
		parentNode.mySerialization.titleName = title;
		parentNode.theCreator.Save ();
	}

}
