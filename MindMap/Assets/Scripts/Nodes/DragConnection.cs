using UnityEngine;
using System.Collections;

public class DragConnection : MonoBehaviour {
	public DragNode node1;
	public DragNode node2;

	public void InitializeConnection (DragNode n1, DragNode n2) {
		node1 = n1;
		node2 = n2;

		print ("Initialized connection.");

		LineRenderer myLine = GetComponent<LineRenderer> ();
			
		myLine.SetPosition(0, n1.gameObject.transform.position);
		myLine.SetPosition(1, n2.gameObject.transform.position);
		myLine.SetWidth (0.05f, 0.05f);
	}
}
