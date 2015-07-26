using UnityEngine;
using System.Collections;

public class Utilities : MonoBehaviour {

	public static DragNode GetParentNode (GameObject thisObject) {
		DragNode parentNode = null;
		Transform nextParent = thisObject.transform.parent;

		while (nextParent != null) {
			if(nextParent.CompareTag("Node")) {
				parentNode = nextParent.gameObject.GetComponent<DragNode>();
				break;
			}
			nextParent = nextParent.transform.parent;
		}
			
		return parentNode;
	}
}
