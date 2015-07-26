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

//	public void LoadTitleFromSave (){//string newTitle) {
//		Debug.Log ("loading from save...");
//		if (parentNode) {
//			Debug.Log("Parent node exists: " + parentNode.name);
////			string savedText = parentNode.mySerialization.titleName;
//			gameObject.GetComponent<InputField>().text = parentNode.name;
//		}
//	}

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
