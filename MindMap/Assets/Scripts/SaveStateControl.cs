using UnityEngine;
using System.Collections;

public class SaveStateControl : MonoBehaviour {
	public static SaveStateControl control;

	public float health;
	public float xp;

	void Awake () 
	{
		if (control == null) 
		{
			DontDestroyOnLoad (gameObject);
			control = this;
		} 
		else if (control != this) 
		{
			Destroy(gameObject);
		}
	}
	
}
