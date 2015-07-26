using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitleHandler : MonoBehaviour {
	public string title;
	public DragNode parentNode;


	void OnEnable () {
		NodeCreator.LoadCompleted += LoadTitleFromSave;
		if (parentNode == null) {
			SetParentNode(Utilities.GetParentNode(gameObject));
		}
	}

	void OnDisable () {
		NodeCreator.LoadCompleted -= LoadTitleFromSave;
	}

	public void LoadTitleFromSave () {
		if (parentNode) {
//			string savedText = parentNode.mySerialization.titleName;
//			gameObject.GetComponent<InputField>().text = savedText;
		}
	}

	public void DoneEditing () {
		UpdateTitle (gameObject.GetComponent<InputField> ().textComponent.text);
	}

	public void SetParentNode(DragNode pNode) {
		parentNode = pNode;
		if (parentNode.theCreator != null) {
//			parentNode.theCreator.Save ();
		}
	}

	public void UpdateTitle(string newTitle) {
		title = newTitle;
//		parentNode.mySerialization.titleName = title;
		parentNode.SetName (title);
//		parentNode.theCreator.Save ();
	}

}
