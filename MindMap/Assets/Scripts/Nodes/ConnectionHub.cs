using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConnectionHub : MonoBehaviour {

	public static List<DragConnection> allConnections;
	public static DragConnection connectionTemplate;

	public DragConnection template;

	void OnEnable () {
		if (connectionTemplate == null) {
			connectionTemplate = template;
		}
		if (allConnections == null) {
			allConnections = new List<DragConnection>();
		}
	}

	public static void AddNewConnection (DragNode origin, DragNode endpoint) {
		Vector3 newPosition = new Vector3 (0, 0, 0);
		DragConnection newConnection = Instantiate (connectionTemplate, newPosition, Quaternion.identity) as DragConnection;
		newConnection.InitializeConnection (origin, endpoint);

		allConnections.Add (newConnection);
		print ("Added the connection: " + allConnections.Count);
	}
}
