using UnityEngine;
using System.Collections;

public class TestURL : MonoBehaviour {

	public void OpenThisURL () {
		print ("Open URL");
		Application.OpenURL ("www.marlenaabraham.com");
	}
}
