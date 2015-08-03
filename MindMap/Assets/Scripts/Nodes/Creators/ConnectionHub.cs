using UnityEngine;
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
		newConnection.SetLabel ("This is a default label", false);
		newConnection.SetThickness (0.8f, false);
		newConnection.SetVisibility (false, false);

		allConnections.Add (newConnection); 
	}

	public static void RemoveConnection () {
		//TO-DO
	}

	public void LoadNewConnection (int loadIDNumber, string loadLabel, float loadThickness, bool loadVisibility) {
		Vector3 newPosition = new Vector3 (0, 0, 0);
		DragConnection newConnection = Instantiate (connectionTemplate, newPosition, Quaternion.identity) as DragConnection;

		newConnection.SetIDNumber (loadIDNumber);
		newConnection.SetLabel (loadLabel, true);
		newConnection.SetThickness (loadThickness, true);
		newConnection.SetVisibility (loadVisibility, true);

		allConnections.Add (newConnection);
	}

	public void InitializeLoadedConnections () {
		DatabaseAccess db = NodeCreator.creator.GrandDatabase;

		foreach (DragConnection nextConnection in allConnections) {
			List<DragNode> connectionEndpoints = db.FindNodesForConnectionID(nextConnection.idNumber);
			nextConnection.InitializeConnection(connectionEndpoints[0], connectionEndpoints[1]);
		}
	}
}


























