using UnityEngine;
using System.Collections;

public class ConnectionCollide : MonoBehaviour {
	DragConnection myParent;
	bool webIsUpdating;

	/***** Initialize the collider for the connection *****/
	public void InitializeCollider (DragNode n1, DragNode n2, DragConnection p) {
		myParent = p;
		transform.parent = p.transform;
		
		CapsuleCollider col = GetComponent<CapsuleCollider> ();
		col.radius = 0.2f;
		UpdateColliderTransform (n1, n2);
	}

	/***** Update collider position/rotation when node position changes *****/
	public void UpdateColliderTransform (DragNode n1, DragNode n2) {
		transform.position = (n1.gameObject.transform.position + n2.gameObject.transform.position) / 2;

		CapsuleCollider col = GetComponent<CapsuleCollider> ();
		col.height = Vector3.Magnitude (n1.gameObject.transform.position - n2.gameObject.transform.position);
		
		Vector3 relativePos = n1.gameObject.transform.position - n2.gameObject.transform.position;
		Quaternion rotation = Quaternion.LookRotation(relativePos);
		transform.rotation = rotation;
	}

	/***** Listen for player input *****/
	public void OnMouseEnter () {
		myParent.myLine.SetWidth (0.3f, 0.3f);
	}

	public void OnMouseExit () {
		myParent.myLine.SetWidth (0.05f, 0.05f);
	}


}
