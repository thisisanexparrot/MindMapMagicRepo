using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitleHandler : MonoBehaviour {
	public string title;
	public DragNode parentNode;


	void Start () {
		//UpdateTitle ("TESTING");
	}

	void OnEnable () {
		if (parentNode == null) {
			print ("No parent Node yet");
			Transform nextParent = transform.parent;
			while (!nextParent.CompareTag("Node")) {
				nextParent = nextParent.transform.parent;
			}
			SetParentNode (nextParent.GetComponent<DragNode> ());
		}
	}

	public void SetParentNode(DragNode pNode) {
		parentNode = pNode;
		if (parentNode.theCreator != null) {
			parentNode.theCreator.Save ();
		}
	}

	public void UpdateTitle(string newTitle) {
		title = newTitle;
		gameObject.GetComponent<Text> ().text = title;
		parentNode.mySerialization.titleName = title;
		parentNode.theCreator.Save ();
	}

}
