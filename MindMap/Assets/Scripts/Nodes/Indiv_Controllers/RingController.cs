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
		NodeCreator.CurrentlyFocusedNodeUpdated += UpdateMaterial;
	}

	void OnDisable () {
		NodeCreator.CurrentlyFocusedNodeUpdated -= UpdateMaterial;
	}

	void UpdateMaterial (DragNode node) {
		if (node && (node.GetInstanceID() == Utilities.GetParentNode (gameObject).GetInstanceID())) {
			GetComponent<Renderer> ().material = normalStateMaterial;
		} else {
			GetComponent<Renderer> ().material = selectedStateMaterial;
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
