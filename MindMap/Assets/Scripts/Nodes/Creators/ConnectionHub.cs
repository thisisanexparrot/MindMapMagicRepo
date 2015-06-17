using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConnectionHub : MonoBehaviour {

	public static List<DragConnection> allConnections;
	public static DragConnection connectionTemplate;
	public NodeCreator theCreator;

	public DragConnection template;

	void OnEnable () {
		theCreator = GetComponent<NodeCreator> ();
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
		/* NEED TO ADDRESS: Get a serialization in there! */
		newConnection.CreateMySerialization (origin, endpoint);

		allConnections.Add (newConnection);
	}

	/***** Save and Load *****/
	public List<ConnectionSerialized> CreateSaveList () {
		List<ConnectionSerialized> returnList = new List<ConnectionSerialized> ();
		foreach (DragConnection connection in allConnections) {
			ConnectionSerialized c = connection.mySerialization;
			returnList.Add(c);
		}
		return returnList;
	}

	public void LoadConnectionsFromFile (List<ConnectionSerialized> serializedList) {
		foreach (ConnectionSerialized cs in serializedList) {
			NodeSerialized n1 = cs.nodes[0];
			NodeSerialized n2 = cs.nodes[1];

			DragNode origin = null;
			DragNode endpoint = null;

			//foreach (NodeSerialized n in cs.nodes) {
			for(int i = 0; i < theCreator.allNodes.Count; i++) {
				DragNode nextNode = theCreator.allNodes[i];
				if(nextNode.mySerialization.idNumber == n1.idNumber) {
					origin = nextNode;
				}
				if(nextNode.mySerialization.idNumber == n2.idNumber) {
					endpoint = nextNode;
				}
			}

			//}

			Vector3 newPosition = new Vector3 (0, 0, 0);
			DragConnection newConnection = Instantiate (connectionTemplate, newPosition, Quaternion.identity) as DragConnection;
			newConnection.SetMySerialization(cs);
			newConnection.InitializeConnection (origin, endpoint); /* NEED THIS SOON TO SPAWN THE LINE IN SPACE */
			allConnections.Add(newConnection);
		}
		theCreator.Save ();
	}
}


























