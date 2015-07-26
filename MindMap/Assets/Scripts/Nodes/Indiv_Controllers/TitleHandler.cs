using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitleHandler : MonoBehaviour {
	public string title;
	public DragNode parentNode;
	
	void OnEnable () {
		//NodeCreator.LoadCompleted += LoadTitleFromSave;
		DragNode.NameWasUpdated += UpdateNameField;
		if (parentNode == null) {
			SetParentNode(Utilities.GetParentNode(gameObject));
		}
	}

	void OnDisable () {
		DragNode.NameWasUpdated -= UpdateNameField;
		//NodeCreator.LoadCompleted -= LoadTitleFromSave;
	}

	public void UpdateNameField (string newName) {
		if (parentNode) {
			gameObject.GetComponent<InputField>().text = parentNode.title;
		}
	}

	public void DoneEditing () {
		UpdateTitle (gameObject.GetComponent<InputField> ().textComponent.text);
	}
	
	public void SetParentNode(DragNode pNode) {
		parentNode = pNode;
		if (parentNode.theCreator != null) {
		}
	}

	public void UpdateTitle(string newTitle) {
		title = newTitle;
		parentNode.SetName (title, false);
	}

}
