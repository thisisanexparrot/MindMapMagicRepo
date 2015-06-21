using UnityEngine;
using System.Collections;

public class ConnectionCollide : MonoBehaviour {

	DragConnection myParent;

	public void InitializeCollider (DragNode n1, DragNode n2, DragConnection p) {
		myParent = p;
		transform.parent = p.transform;
		transform.position = (n1.gameObject.transform.position + n2.gameObject.transform.position) / 2;
		
		CapsuleCollider col = GetComponent<CapsuleCollider> ();
		col.radius = 0.2f;
		col.height = Vector3.Magnitude (n1.gameObject.transform.position - n2.gameObject.transform.position);
		
		Vector3 relativePos = n1.gameObject.transform.position - n2.gameObject.transform.position;
		Quaternion rotation = Quaternion.LookRotation(relativePos);
		transform.rotation = rotation;
	}

	public void OnMouseEnter () {
		myParent.myLine.SetWidth (0.3f, 0.3f);
		print ("Entered!");
	}

	public void OnMouseExit () {
		myParent.myLine.SetWidth (0.05f, 0.05f);
		print ("Exited.");
	}
}
