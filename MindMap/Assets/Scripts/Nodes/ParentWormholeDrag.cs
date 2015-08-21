using UnityEngine;
using System.Collections;

public class ParentWormholeDrag : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("Node")) {
			DragNode dn = other.gameObject.GetComponent<DragNode>();
			dn.SetNewPotentialParent(Utilities.GetParentNode(this.gameObject));
			dn.TriggerPotentialParentDropVisuals(true);
		}
	}

	void OnTriggerExit (Collider other) {
		if (other.CompareTag ("Node")) {
			DragNode dn = other.gameObject.GetComponent<DragNode>();
			dn.SetNewPotentialParent(null);
			dn.TriggerPotentialParentDropVisuals(false);
		}
	}
}
