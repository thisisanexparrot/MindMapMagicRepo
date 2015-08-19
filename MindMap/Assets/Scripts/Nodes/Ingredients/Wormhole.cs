using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Wormhole : MonoBehaviour {

	void OnMouseEnter () {
		transform.localScale = new Vector3 (1.2f, 1.2f, 1.2f);
	}

	void OnMouseExit () {
		transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
	}

	void OnMouseUp () {
		Debug.Log ("Clicked");
		Camera.main.GetComponent<MouseOrbitImproved>().SetTarget(gameObject, true);
		NodeCreator.creator.EnterWormhole(Utilities.GetParentNode(this.gameObject));
	}
}
