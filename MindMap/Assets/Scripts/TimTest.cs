using UnityEngine;
using System.Collections;

public class TimTest : MonoBehaviour {
	public AudioSource source1;
	public AudioSource source2;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.L)) {
			print ("Pressed!");
			if(source1.isPlaying) {
				source1.Stop();
			}
			else {
				source1.Play();
			}
		}
	}
}
