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
			string savedText = parentNode.mySerialization.titleName;
			print (savedText);
			gameObject.GetComponent<InputField>().text = savedText;
		}
	}

	public void DoneEditing () {
		UpdateTitle (gameObject.GetComponent<InputField> ().textComponent.text);
	}

	public void SetParentNode(DragNode pNode) {
		print ("***SAVE***** (setparentnode)");
		parentNode = pNode;
		if (parentNode.theCreator != null) {
			parentNode.theCreator.Save ();
		}
	}

	public void UpdateTitle(string newTitle) {
		print ("***SAVE***** (updatetitle)");
		title = newTitle;
		parentNode.mySerialization.titleName = title;
		parentNode.theCreator.Save ();
	}

}
