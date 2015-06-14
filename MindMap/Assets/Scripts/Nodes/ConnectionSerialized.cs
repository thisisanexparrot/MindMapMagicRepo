using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ConnectionSerialized {
	public string label;
	public int thickness;
	public bool isBold;
	public bool isVisible;

	public int rValue;
	public int gValue;
	public int bValue;

	public List<NodeSerialized> nodes;
}
