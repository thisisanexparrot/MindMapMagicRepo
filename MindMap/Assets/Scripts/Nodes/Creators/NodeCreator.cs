#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class NodeCreator : MonoBehaviour {

	public static NodeCreator creator;
	public static ConnectionHub connectionCentralHub;
	public static string[] baseNodeTableColumnNames = new string[6] {"idNumber","Name","Description","locationX","locationY","locationZ"}; 
	public static string[] baseNodeTableColumnTypes = new string[6] {"int","text","text","float","float","float"}; 
	public string defaultName = "New Node";
	public string defaultDesc = "New description";

	public delegate void InitLoad ();
	public static event InitLoad LoadCompleted;

	public DragNode blankNodeTemplate;
//	public NodeListAndSavedData saveNodesList;

	//public int localNodeCounter;
//	public List<NodeSerialized> localNodeList;
//	public List<ConnectionSerialized> tempConnectionList;
	public List<DragNode> allNodes;

//	public dbAccess graphDatabase;

	DatabaseAccess GrandDatabase;

	public string dbn_MainDatabase = "CentralGraphDatabase.sqdb";
	public string tn_node = "NodeTable";
	public string tn_connection = "ConnectionTable";
	public string tn_mid = "MidTable";

	string playerPath = "/playerInfo26.dat";

	/********* INIT  **********/
	/* Wake-up load functions */
	void Awake () {
		if (creator == null)
		{
			DontDestroyOnLoad(gameObject);
			creator = this;
		}
		else if (creator != this)
		{
			Destroy(gameObject);
		}
		if (connectionCentralHub == null) {
			connectionCentralHub = GetComponent<ConnectionHub>();
			DontDestroyOnLoad(connectionCentralHub);
		}

		GrandDatabase = new DatabaseAccess ();
		GrandDatabase.OpenDatabase (dbn_MainDatabase, this);
		GrandDatabase.ReadNodesFromDatabase ();


//		graphDatabase = new dbAccess();
//		DatabaseUtils.OpenDatabase_DB (graphDatabase, graphDatabaseName);
		string[] columnNames = new string[6] {"idNumber","Name","Description","locationX","locationY","locationZ"};
		string[] columnTypes = new string[6] {"int","text","text","float","float","float"};

		string[] columnNamesConnects = new string[4] {"idNumber", "Label", "Thickness", "IsVisible"};
		string[] columnTypesConnects = new string[4] {"int", "text", "float", "bool"};

		string[] columnNamesMidTable = new string[2] {"NodeIDNumber", "ConnectionIDNumber"};
		string[] columnTypesMidTable = new string[2] {"int", "int"};

		LoadFromDatabase ();

		
//		DatabaseUtils.CreateTable_DB (graphDatabase, nodeTableName, columnNames, columnTypes);
//		DatabaseUtils.CreateTable_DB (graphDatabase, connectionTableName, columnNamesConnects, columnTypesConnects);
//		DatabaseUtils.CreateTable_DB (graphDatabase, midTableName, columnNamesMidTable, columnTypesMidTable);
		//graphDatabase.CreateTable(nodeTableName,columnNames,columnValues);
		print ("Awake!");

	}

	/* Broadcasts event of initial load */
	void OnEnable () {
		connectionCentralHub.InitializeConnectionHub ();
		//Load ();
//		DatabaseUtils.LoadNodeTable_DB (graphDatabase, nodeTableName);
		LoadFromDatabase ();
		if (LoadCompleted != null) {
			LoadCompleted ();
		}
	}

	/*void TransferToDatabase () {
		foreach (NodeSerialized nextNode in localNodeList) {
			int myIDNumber = nextNode.idNumber;
			string myIDString = myIDNumber.ToString();
			graphDatabase.UpdateColumn(nodeTableName, "Name", nextNode.titleName, "string", "idNumber", "=", myIDString);
			graphDatabase.UpdateColumn(nodeTableName, "Description", nextNode.description, "string", "idNumber", "=", myIDString);
			graphDatabase.UpdateColumn(nodeTableName, "locationX", nextNode.locationX.ToString(), "float", "idNumber", "=", myIDString);
			graphDatabase.UpdateColumn(nodeTableName, "locationY", nextNode.locationY.ToString(), "float", "idNumber", "=", myIDString);
			graphDatabase.UpdateColumn(nodeTableName, "locationZ", nextNode.locationZ.ToString(), "float", "idNumber", "=", myIDString);
		}
	}*/


	/********* CREATE **********/
	/* Create & Remove individual nodes for storage */
	public void SpawnNewNode () {
		string defaultPos = 0.0f.ToString ();
//		string[] firstValues = new string[] {saveNodesList.nodeCounter.ToString(), defaultName, defaultDesc, defaultPos, defaultPos, defaultPos};
		print ("SPAWNING!");
//		DatabaseUtils.InsertIntoSpecific_DB(graphDatabase, 
//		                                    nodeTableName, 
//		                                    baseNodeTableColumnNames, 
//		                                    firstValues, 
//		                                    baseNodeTableColumnTypes);
		print ("DONE SPAWNING!");

		Vector3 mousePosition = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10);
		DragNode newNode = Instantiate (blankNodeTemplate, Camera.main.ScreenToWorldPoint(mousePosition), Quaternion.identity) as DragNode;
//		NodeSerialized newSerialized = CreateNewSerializeNode ();
		
//		newNode.GetComponent<DragNode> ().InitializeNode (newSerialized, this, true);
//		saveNodesList.nodeCounter += 1;

//		Save (); //*** SERI_MARK
	}

	public void LoadNewNode (int _idNumber, string _name, string _description, float _posX, float _posY, float _posZ) {
		Vector3 mousePosition = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10);
		DragNode newNode = Instantiate (blankNodeTemplate, Camera.main.ScreenToWorldPoint(mousePosition), Quaternion.identity) as DragNode;
		allNodes.Add (newNode);

		newNode.SetName (_name);
		newNode.SetDescription (_description);
		newNode.SetIDNumber (_idNumber);
		newNode.transform.position = new Vector3 (_posX, _posY, _posZ);
	}


//	NodeSerialized CreateNewSerializeNode () {
//		NodeSerialized newNode = new NodeSerialized ();
//		newNode.titleName = "This is a new name";
//		newNode.isSelected = false;
//		newNode.idNumber = saveNodesList.nodeCounter;
//		localNodeList.Add (newNode);
//
//		return newNode;
//	}

	public void RemoveNode (DragNode destroyThis) {
//		NodeSerialized destroyID = destroyThis.mySerialization;
//		DatabaseUtils.DeleteFromTableWhere_DB (graphDatabase, nodeTableName, "idNumber", "=", destroyThis.mySerialization.idNumber.ToString());
		destroyThis.DestroyThisNode ();
//		localNodeList.Remove (destroyID);
		Destroy (destroyThis.gameObject);
//		Save ();
	}

	/********* SAVE **********/
	/* Save and Load Methods */ /* By the end of the day, these two methods should become obselete */
//	public void Save () {
//		BinaryFormatter bf = new BinaryFormatter ();
//		FileStream file = File.Create (Application.persistentDataPath + playerPath);
//
//		if (saveNodesList == null) {
//			saveNodesList = new NodeListAndSavedData ();
//			saveNodesList.nodeCounter = 0;
//			saveNodesList.connectionList = new List<ConnectionSerialized> ();
//		} else {
//			saveNodesList.connectionList = connectionCentralHub.CreateSaveList (); // <---- Here's the problem: the list is getting saved over before it can be loaded because "save" is called before load can finish.
//		}
//		saveNodesList.nodeList = localNodeList;
//
//		bf.Serialize (file, saveNodesList);
//		file.Close ();
//		/* Reminder: This probably happens a lot more than it needs to; come fix it later. */
//	}
//
//	public void Load () {
//		if (File.Exists (Application.persistentDataPath + playerPath))
//		{
//			BinaryFormatter bf = new BinaryFormatter ();
//			FileStream file = File.Open (Application.persistentDataPath + playerPath, FileMode.Open);
//			NodeListAndSavedData data = (NodeListAndSavedData)bf.Deserialize (file);
//			file.Close ();
//
//			saveNodesList = data;
//			localNodeList = data.nodeList;
//			tempConnectionList = data.connectionList;
//
//			LoadNodesFromSerialized ();
//			connectionCentralHub.LoadConnectionsFromFile(tempConnectionList);
//			//TransferToDatabase();
//		}
//		else
//		{
//			print ("File with name not found.");
//		}
//	}

	public void LoadFromDatabase () {
//		DatabaseUtils.ReadFullTable_DB (graphDatabase, nodeTableName);
	}
	
//	void LoadNodesFromSerialized () {
//		foreach (NodeSerialized nextNode in localNodeList) {
//			Vector3 nextPosition = FloatsToVector3(nextNode);
//			DragNode newNode = Instantiate (blankNodeTemplate, nextPosition, Quaternion.identity) as DragNode;
//			newNode.GetComponent<DragNode> ().InitializeNode (nextNode, this, false);
//			allNodes.Add(newNode);
//		}
//	}

	/********* UTILS **********/
	/* Converters for Vector3 Serialization in NodeSerialized */
//	public void Vector3ToFloats (NodeSerialized n, Vector3 input) {
//		n.locationX = input.x;
//		n.locationY = input.y;
//		n.locationZ = input.z;
//	}
//
//	public Vector3 FloatsToVector3 (NodeSerialized n) {
//		Vector3 newVector = new Vector3 (n.locationX, n.locationY, n.locationZ);
//		return newVector;
//	}



}









