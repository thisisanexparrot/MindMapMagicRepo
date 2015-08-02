﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConnectionHub : MonoBehaviour {

	public static List<DragConnection> allConnections;
	public static DragConnection connectionTemplate;
	public NodeCreator theCreator;

	public DragConnection template;
	

	public void InitializeConnectionHub () {
		theCreator = GetComponent<NodeCreator> ();
		if (connectionTemplate == null) {
			connectionTemplate = template;
		}
		CheckAllConnectionsExists ();
	}

	void CheckAllConnectionsExists () {
		if (allConnections == null) {
			allConnections = new List<DragConnection>();
		}
	}

	/***** Add and remove new connections *****/
	public static void AddNewConnection (DragNode origin, DragNode endpoint) {
		Vector3 newPosition = new Vector3 (0, 0, 0);
		if (connectionTemplate == null) {
			Debug.Log("null");
		}
		DragConnection newConnection = Instantiate (connectionTemplate, newPosition, Quaternion.identity) as DragConnection;
		newConnection.InitializeConnection (origin, endpoint);
//		newConnection.CreateMySerialization (origin, endpoint);

		/* Add the new connection (idNumber, default values) to database */
		DatabaseAccess db = NodeCreator.creator.GrandDatabase;
		db.IncrementIDCounter (DatabaseAccess.TableType.Connection);
		int newConnectionID = db.GetCurrentIDCount (DatabaseAccess.TableType.Connection); //Increment counter, get next ID Number
		db.AddNewConnectionToDatabase (newConnectionID);

		db.AddMidConnectionToDatabalse (origin.idNumber, newConnectionID);
		db.AddMidConnectionToDatabalse (endpoint.idNumber, newConnectionID);

		newConnection.SetIDNumber (newConnectionID);
		newConnection.SetLabel ("This is a default label");
		newConnection.SetThickness (0.8f);
		newConnection.SetVisibility (true);

		allConnections.Add (newConnection); //todo: is this still necessary?

		/* Add two new mid table entries for the connection with each of its nodes */
		//todo


		/* Add new connection to database here */ 
	}

	public static void RemoveConnection () {
		//TO-DO
	}

	public void LoadNewConnection () {
		//To-do
	}


	/***** Save and Load *****/
//	public List<ConnectionSerialized> CreateSaveList () {
//		List<ConnectionSerialized> returnList = new List<ConnectionSerialized> ();
//		foreach (DragConnection connection in allConnections) {
//			ConnectionSerialized c = connection.mySerialization;
//			returnList.Add(c);
//		}
//		return returnList;
//	}
//
//	public void LoadConnectionsFromFile (List<ConnectionSerialized> serializedList) {
//		foreach (ConnectionSerialized cs in serializedList) {
//			NodeSerialized n1 = cs.nodes[0];
//			NodeSerialized n2 = cs.nodes[1];
//
//			DragNode origin = null;
//			DragNode endpoint = null;
//
//			for(int i = 0; i < theCreator.allNodes.Count; i++) {
//				DragNode nextNode = theCreator.allNodes[i];
//				if(nextNode.mySerialization.idNumber == n1.idNumber) {
//					origin = nextNode;
//				}
//				if(nextNode.mySerialization.idNumber == n2.idNumber) {
//					endpoint = nextNode;
//				}
//			}
//
//			Vector3 newPosition = new Vector3 (0, 0, 0);
//			DragConnection newConnection = Instantiate (connectionTemplate, newPosition, Quaternion.identity) as DragConnection;
//			newConnection.SetMySerialization(cs);
//			newConnection.InitializeConnection (origin, endpoint); 
//			allConnections.Add(newConnection);
//		}
//		theCreator.Save ();
//	}
}


























