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

	void UpdateMaterial (bool isSelected) {
		if (isSelected) {
			GetComponent<Renderer>().material = selectedStateMaterial;
		} else {
			GetComponent<Renderer>().material = normalStateMaterial;
		}
	}

	void ResetOffset () {
		screenPoint = Camera.main.WorldToScreenPoint (gameObject.transform.position);
		print (gameObject.transform.position);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 
		                                                                                    Input.mousePosition.y,
		                                                                                    screenPoint.z));
	}

	void OnMouseDown () {
		print ("Down.");
		myLine = gameObject.AddComponent <LineRenderer>();
		ResetOffset ();
		Vector3 curScreenPoint = new Vector3 (Input.mousePosition.x,  
		                                      Input.mousePosition.y,
		                                      screenPoint.z);
		Vector3 startPosition = Camera.main.ScreenToWorldPoint (curScreenPoint) + offset;


		myLine.SetPosition(0, startPosition);
		myLine.SetPosition(1, startPosition);
		myLine.SetWidth (0.05f, 0.05f);
		myLine.material = lineColor;
		GameObject g = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		g.transform.position = startPosition;
	}

	void OnMouseDrag() {
		print ("Dragging...");
		Vector3 curScreenPoint = new Vector3 (Input.mousePosition.x,  
		                                      Input.mousePosition.y,
		                                      screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenPoint);// + offset;
		//transform.position = curPosition;

		myLine.SetPosition(1, curPosition);
	}

	void OnMouseUp () {
		print ("Released.");
		if (gameObject.GetComponent<LineRenderer> ()) {
			Destroy(gameObject.GetComponent<LineRenderer>());
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 100)) {
			print ("Hit!");
			if(hit.collider.gameObject.CompareTag("Ring")) {
				print ("Hit ring!");
			}
			Debug.DrawLine (ray.origin, hit.point);
		}
	}
}
