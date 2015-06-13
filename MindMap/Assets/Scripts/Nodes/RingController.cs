using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class RingController : MonoBehaviour {
	public Material normalStateMaterial;
	public Material selectedStateMaterial;
	public Material lineColor;

	public LineRenderer myLine;

	private Vector3 screenPoint;
	private Vector3 offset;


	void OnEnable () {
		DragNode.NodeSelectionUpdate += UpdateMaterial;
	}

	void OnDisable () {
		DragNode.NodeSelectionUpdate -= UpdateMaterial;
	}

	void UpdateMaterial (bool isSelected, DragNode node) {
		if (node.GetInstanceID() == Utilities.GetParentNode (gameObject).GetInstanceID()) {
			if (isSelected) {
				GetComponent<Renderer> ().material = selectedStateMaterial;
			} else {
				GetComponent<Renderer> ().material = normalStateMaterial;
			}
		}
	}

	void ResetOffset () {
		screenPoint = Camera.main.WorldToScreenPoint (gameObject.transform.position);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 
		                                                                                    Input.mousePosition.y,
		                                                                                    screenPoint.z));
	}

	void OnMouseDown () {
		myLine = gameObject.AddComponent <LineRenderer>();
		ResetOffset ();
		Vector3 curScreenPoint = new Vector3 (Input.mousePosition.x,  
		                                      Input.mousePosition.y,
		                                      screenPoint.z);
		Vector3 startPosition = Camera.main.ScreenToWorldPoint (curScreenPoint) + offset;


		myLine.SetPosition(0, startPosition);
		myLine.SetPosition(1, startPosition);
		myLine.SetWidth (0.08f, 0.08f);
		myLine.material = lineColor;
	}

	void OnMouseDrag() {
		Vector3 curScreenPoint = new Vector3 (Input.mousePosition.x,  
		                                      Input.mousePosition.y,
		                                      screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenPoint);

		myLine.SetPosition(1, curPosition);
	}

	void OnMouseUp () {
		if (gameObject.GetComponent<LineRenderer> ()) {
			Destroy(gameObject.GetComponent<LineRenderer>());
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 100)) {
			if(hit.collider.gameObject.CompareTag("Ring")) {
				DragNode myParent = Utilities.GetParentNode(gameObject);
				DragNode newConnection = Utilities.GetParentNode(hit.collider.gameObject);
				ConnectionHub.AddNewConnection(myParent, newConnection);
			}
			Debug.DrawLine (ray.origin, hit.point);
		}
	}
}
