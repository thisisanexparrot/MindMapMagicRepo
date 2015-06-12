using UnityEngine;
using System.Collections;

public enum NodeState{Active, Inactive, Completed};

[System.Serializable]
public class NodeSerialized {
	/* Add an enum for state*/
	public string titleName = "New Node";
	public string description = "New description";
	public bool isSelected = false;
	public int idNumber;
	public int priority;

	public NodeState currentState;

	public float locationX;
	public float locationY;
	public float locationZ;
}
