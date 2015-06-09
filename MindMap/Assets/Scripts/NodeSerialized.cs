using UnityEngine;
using System.Collections;

[System.Serializable]
public class NodeSerialized : ScriptableObject {
	public string titleName = "New Node";
	public Vector3 location;
	public bool isSelected = false;
}
