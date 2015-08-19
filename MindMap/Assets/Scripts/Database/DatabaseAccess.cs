using UnityEngine;
using System.Collections;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;
using System;

public class DatabaseAccess {

	/************************/
	/*  Database Variables  */
	/************************/

	public enum TableType {Node, Connection, NodeConIdentifier, Meta, NodeParent};

	/***** Database operators *****/
	private string _constr;
	private IDbConnection _dbConnection;
	private IDbCommand _dbCommand;
	private IDataReader _dbReader;

	/***** Database and table names *****/
	public const string dbn_MainDatabase = "CentralGraphDatabase.sqdb";
	public const string tn_meta = "MetaTable";
	public const string tn_node = "NodeTable";
	public const string tn_connection = "ConnectionTable";
	public const string tn_mid = "MidTable";
	public const string tn_nodeparent = "NodeParentTable";

	public const string node_idNumber = "idNumber";
	public const string node_name = "Name";
	public const string node_desc = "Description";
	public const string node_locX = "locationX";
	public const string node_locY = "locationY";
	public const string node_locZ = "locationZ";
	public const string node_isVisible = "IsVisible";

	public const string con_idNumber = "idNumber";
	public const string con_label = "Label";
	public const string con_thickness = "Thickness";
	public const string con_isVisible = "IsVisible";

	public const string mid_nodeID = "NodeIDNumber";
	public const string mid_conID = "ConnectionIDNumber";

	public const string meta_nodeIDCounter = "NodeIDCounter";
	public const string meta_conIDCounter = "ConnectionIDCounter";

	public const string nodeparent_parentID = "ParentNodeIDNumber";
	public const string nodeparent_childID = "ChildNodeIDNumber";
	public const string nodeparent_childLocalX = "ChildLocalX";
	public const string nodeparent_childLocalY = "ChildLocalY";
	public const string nodeparent_childLocalZ = "ChildLocalZ";


	/***** Table column names and types *****/
	public static string[] colNames_Nodes = new string[7] {node_idNumber,node_name,node_desc,node_locX,node_locY,node_locZ, node_isVisible}; 
	public static string[] colTypes_Nodes = new string[7] {"int","text","text","float","float","float", "bool"}; 

	public string[] colNames_Connections = new string[4] {con_idNumber, con_label, con_thickness, con_isVisible};
	public string[] colTypes_Connections = new string[4] {"int", "text", "float", "bool"};
	
	public string[] colNames_Mid = new string[2] {mid_nodeID, mid_conID};
	public string[] colTypes_Mid = new string[2] {"int", "int"};

	public string[] colNames_Meta = new string[2] {meta_nodeIDCounter, meta_conIDCounter};
	public string[] colTypes_Meta = new string[2] {"int", "int"};

	public string[] colNames_NodeParent = new string[5] {nodeparent_parentID, nodeparent_childID, nodeparent_childLocalX, nodeparent_childLocalY, nodeparent_childLocalZ};
	public string[] colTypes_NodeParent = new string[5] {"int", "int", "float", "float", "float"};


	/***************************************/
	/*    High-Level Database Functions    */
	/*     - [Open database]               */
	/*     - [Create tables]               */
	/***************************************/

	/***** Open an existing database *****/
	public void OpenDatabase (string p, NodeCreator n) {
		_constr = "URI=file:" + p; 
		_dbConnection = new SqliteConnection(_constr);
		_dbConnection.Open();
	}

	/***** Create new table *****/
	public void CreateNewTable (TableType newTableType) {
		_dbCommand=_dbConnection.CreateCommand();
		string query = "";

		switch (newTableType) {
		case TableType.Node: 
			query = "CREATE TABLE " + tn_node + "(" + colNames_Nodes[0] + " " + colTypes_Nodes[0];
			for (int i = 1; i < colNames_Nodes.Length; i++) {
				query += ", " + colNames_Nodes[i] + " " + colTypes_Nodes[i];
			}
			query += ")";
			break;
		case TableType.Connection:
			query = "CREATE TABLE " + tn_connection + "(" + colNames_Connections[0] + " " + colTypes_Connections[0];
			for (int i = 1; i < colNames_Connections.Length; i++) {
				query += ", " + colNames_Connections[i] + " " + colTypes_Connections[i];
			}
			query += ")";			
			break;
		case TableType.NodeConIdentifier:
			query = "CREATE TABLE " + tn_mid + "(" + colNames_Mid[0] + " " + colTypes_Mid[0];
			for (int i = 1; i < colNames_Mid.Length; i++) {
				query += ", " + colNames_Mid[i] + " " + colTypes_Mid[i];
			}
			query += ")";			
			break;
		case TableType.Meta:
			query = "CREATE TABLE " + tn_meta + "(" + colNames_Meta[0] + " " + colTypes_Meta[0];
			for (int i = 1; i < colNames_Meta.Length; i++) {
				query += ", " + colNames_Meta[i] + " " + colTypes_Meta[i];
			}
			query += ")";			
			break;
		case TableType.NodeParent:
			query = "CREATE TABLE " + tn_nodeparent + "(" + colNames_NodeParent[0] + " " + colTypes_NodeParent[0];
			for (int i = 1; i < colNames_NodeParent.Length; i++) {
				query += ", " + colNames_NodeParent[i] + " " + colTypes_NodeParent[i];
			}
			query += ")";			
			break;
		default:
			Debug.Log("Unknown table type");
			break;
		}

		if (query.Length > 0) {
			_dbCommand.CommandText = query;
			try {
				_dbReader = _dbCommand.ExecuteReader ();
			} catch (Exception e) {
				Debug.Log ("Don't worry; a table of type [" + newTableType + "] already exists!");
			}
		}
	}

	/***************************************/
	/*    Large-scale database reads       */
	/*     - [Read all nodes]              */
	/*     - [Read all connections]        */
	/*     - [Find nodes for connection]   */
	/***************************************/

	/***** Populate nodes in NodeCreator from node table; create new gameobjects *****/
	public void ReadNodesFromDatabase () {
		_dbCommand = _dbConnection.CreateCommand();
		_dbCommand.CommandText = "SELECT * FROM " + tn_node;
		_dbReader = _dbCommand.ExecuteReader();

		while(_dbReader.Read()) { 
			int idNumber = (int)_dbReader.GetValue(0);
			string name = (string)_dbReader.GetValue(1);
			string desc = (string)_dbReader.GetValue(2);
			float posX = (float)((double)_dbReader.GetValue(3));
			float posY = (float)((double)_dbReader.GetValue(4));
			float posZ = (float)((double)_dbReader.GetValue(5));
			NodeCreator.creator.LoadNewNode(idNumber, name, desc, posX, posY, posZ);
		}
	}

	/***** Read all connections from the database and create new game objects for each one *****/
	public void ReadConnectionsFromDatabase () {
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "SELECT * FROM " + tn_connection;
		_dbReader = _dbCommand.ExecuteReader();

		while (_dbReader.Read()) { 
			int idNumber = (int)_dbReader.GetValue(0);
			string label = (string)_dbReader.GetValue(1);
			float thickness = (float)((double)_dbReader.GetValue(2));
			bool isVisible = Convert.ToBoolean(_dbReader.GetValue(3));

			NodeCreator.connectionCentralHub.LoadNewConnection(idNumber, label, thickness, isVisible);
		}
	}

	/***** Get the two nodes that the given connection connects and return those two nodes *****/
	public List<DragNode> FindNodesForConnectionID (int connectionID) {
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "SELECT " + mid_nodeID + " FROM " + tn_mid + " WHERE " + mid_conID + " = " + connectionID + ";";
		_dbReader = _dbCommand.ExecuteReader ();

		List<int> nodeIDNumbers = new List<int> ();

		while (_dbReader.Read()) {
			int nextID = (int)_dbReader.GetValue(0);
			nodeIDNumbers.Add(nextID);
		}

		List<DragNode> returnNodes = new List<DragNode> ();
		foreach (int nextNodeID in nodeIDNumbers) {
			DragNode searchNode;
			if(NodeCreator.allNodesDictionary.TryGetValue(nextNodeID, out searchNode)) {
				returnNodes.Add(searchNode);
			}
			else {
				Debug.Log ("Error: node does not exist");
			}
		}

		return returnNodes;

	}

	/****************************************/
	/*    Singular additions to database    */
	/*     - [Add new node]                 */
	/*     - [Add new connection]           */
	/****************************************/

	/***** Add newly created node to node table *****/
	public void AddNewNodeToDatabase (int initIDNumber) {
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "INSERT INTO " + tn_node + " ('" + node_idNumber + "') VALUES ('" + initIDNumber + "');";
		_dbReader = _dbCommand.ExecuteReader ();
	}

	/***** Add newly created connection to connection table and two references to its nodes in the mid table *****/
	public void AddNewConnectionToDatabase (int initIDNumber) {
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "INSERT INTO " + tn_connection + " ('" + con_idNumber + "') VALUES ('" + initIDNumber + "');";
		_dbReader = _dbCommand.ExecuteReader ();
	}

	/***** Add a new reference to a node and a connection to the mid table *****/
	public void AddMidConnectionToDatabalse (int idNumberNode, int idNumberConnection) {
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "INSERT INTO " + tn_mid + " VALUES ('" + idNumberNode + "', '" + idNumberConnection + "');";
		_dbReader = _dbCommand.ExecuteReader ();
	}

	/*****************************************/
	/*    Singular removals from database    */
	/*     - [Remove node]                   */
	/*     - [Remove connection]             */
	/*****************************************/

	/***** Removes a single node from all of the relevant tables *****/
	public void RemoveNodeFromDatabase (int removedIDNumber) {
		/* Delete single node from Node table */
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "DELETE FROM " + tn_node + " WHERE " + node_idNumber + " = '" + removedIDNumber + "';";
		_dbCommand.ExecuteReader ();

		//TODO: Find all connection IDs in the mid table where the node id = removeIDNumber, then delete all of those connection ids from the connection table and the mid table

		/* Select all rows in mid table with the removed node's ID number */
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "SELECT * FROM " + tn_mid + " WHERE " + mid_nodeID + " = '" + removedIDNumber + "';";
		_dbReader = _dbCommand.ExecuteReader ();

		/* Get all of the connection IDs that are associated with the node ID */
		List<int> connectionsToRemove = new List<int> ();
		while (_dbReader.Read()) { 
			int deletableConnectionID = (int)_dbReader.GetValue (1);
			connectionsToRemove.Add(deletableConnectionID);
		}

		/* Remove all associated connections from both the connections table and the mid table */
		foreach (int i in connectionsToRemove) {
			_dbCommand = _dbConnection.CreateCommand ();
			_dbCommand.CommandText = "DELETE FROM " + tn_connection + " WHERE " + con_idNumber + " = '" + i + "';";
			_dbCommand.ExecuteReader();

			_dbCommand = _dbConnection.CreateCommand ();
			_dbCommand.CommandText = "DELETE FROM " + tn_mid + " WHERE " + mid_conID + " = '" + i + "';";
			_dbCommand.ExecuteReader();
		}

		/* Remove the node from the parent table */
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "DELETE FROM " + tn_nodeparent + " WHERE " + nodeparent_childID + " = '" + removedIDNumber + "';";
		_dbReader = _dbCommand.ExecuteReader ();

		//TODO: Figure out what to do about children when their parents are deleted: delete recursively? put them somewhere else?
	}

	/***** Remove a single connection from the database *****/
	public void RemoveConnectionFromDatabase (int removedIDNumber) {
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "SELECT * FROM " + tn_mid + " WHERE " + mid_conID + " = '" + removedIDNumber + "';";
		_dbReader = _dbCommand.ExecuteReader ();
		
		/* Get all of the node IDs that are associated with the connection ID */
		List<int> connectionsToRemove = new List<int> ();
		while (_dbReader.Read()) { 
			int deletableConnectionID = (int)_dbReader.GetValue (0);
			connectionsToRemove.Add(deletableConnectionID);
		}

		foreach (int i in connectionsToRemove) {
			_dbCommand = _dbConnection.CreateCommand ();
			_dbCommand.CommandText = "DELETE FROM " + tn_mid + " WHERE " + mid_nodeID + " = '" + i + "';";
			_dbCommand.ExecuteReader ();
		}

		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "DELETE FROM " + tn_connection + " WHERE " + con_idNumber + " = '" + removedIDNumber + "';";
		_dbCommand.ExecuteReader ();
	}

	/***** Set a new position for a node when it is created *****/
	public void SetNodePosition (int idNumber, 
	                             float newX, 
	                             float newY, 
	                             float newZ) {
		/* Set position in Node table */
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "UPDATE " + tn_node + " SET " + " locationX = '" + newX + "' WHERE " + "idNumber = " + idNumber;
		_dbReader = _dbCommand.ExecuteReader();

		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "UPDATE " + tn_node + " SET " + " locationY = '" + newY + "' WHERE " + "idNumber = " + idNumber;
		_dbReader = _dbCommand.ExecuteReader ();

		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "UPDATE " + tn_node + " SET " + " locationZ = '" + newZ + "' WHERE " + "idNumber = " + idNumber;
		_dbReader = _dbCommand.ExecuteReader();

		/* Set position in Parent table */
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "UPDATE " + tn_nodeparent + " SET "+ nodeparent_childLocalX+ " = '" + newX + "' WHERE " + nodeparent_childID + " = " + idNumber;
		_dbReader = _dbCommand.ExecuteReader();
		
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "UPDATE " + tn_nodeparent + " SET "+ nodeparent_childLocalY+ " = '" + newY + "' WHERE " + nodeparent_childID + " = " + idNumber;
		_dbReader = _dbCommand.ExecuteReader ();
		
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "UPDATE " + tn_nodeparent + " SET "+ nodeparent_childLocalZ+ " = '" + newZ + "' WHERE " + nodeparent_childID + " = " + idNumber;
		_dbReader = _dbCommand.ExecuteReader();
	}

	/**************************/
	/*  Meta Table Functions  */
	/**************************/

	/***** Populate the meta table's row on first creation *****/
	public void CreateInitialCounterRow () {
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "SELECT Count(*) FROM " + tn_meta + ";";
		_dbReader = _dbCommand.ExecuteReader ();

		_dbReader.Read ();
		int rowCount = (int)_dbReader.GetInt32 (0);

		if (rowCount < 1) {
			_dbCommand = _dbConnection.CreateCommand ();
			_dbCommand.CommandText = "INSERT INTO " + tn_meta + " VALUES ('1', '1');";
			_dbReader = _dbCommand.ExecuteReader ();
		} 
	}

	/***** Increment ID counter for either Nodes or Connections in the Meta table *****/
	public void IncrementIDCounter (TableType idType) {
		string updatedColumn = TableTypeToString (idType);
		int counter = GetCurrentIDCount (idType);
		counter++;
		
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "UPDATE " + tn_meta + " SET " + updatedColumn + " = '" + counter + "';"; 
		_dbReader = _dbCommand.ExecuteReader ();
	}

	public string TableTypeToString (TableType thisType) {
		string myType = "";
		switch (thisType) {
		case TableType.Node:
			myType = meta_nodeIDCounter;
			break;
		case TableType.Connection:
			myType = meta_conIDCounter;
			break;
		default:
			Debug.Log("Unknown ID Type");
			break;
		}
		return myType;
	}

	/***** Get the current ID number count for the specified table type from the meta table *****/
	public int GetCurrentIDCount (TableType idType) {
		string whichColumn = TableTypeToString (idType);
		
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "SELECT " + whichColumn + " FROM " + tn_meta +";"; 
		_dbReader = _dbCommand.ExecuteReader ();
		
		_dbReader.Read ();
		int counter = (int)_dbReader.GetInt64 (0);
		return counter;
	}

	/***** Used to set things like titles, descriptions, etc. *****/
	public void SetObjectInTable (string tableName,
	                              int idNumber,
	                              string columnName,
	                              object newObject) {
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "UPDATE " + tableName + " SET " + columnName + " = '" + newObject + "' WHERE " + "idNumber = " + idNumber;
		_dbReader = _dbCommand.ExecuteReader ();		
	}

	/****************************/
	/*  Parent Table Functions  */
	/****************************/

	/***** Utility: Get parent ID number, return 0 if parent is null *****/
	public int GetParentID (DragNode parentNode) {
		int parentID;
		if (parentNode) {
			parentID = parentNode.idNumber;
		} else {
			parentID = 0;
		}
		return parentID;
	}

	/***** Add a new node to the parent/child node table *****/
	public void AddNodeToParentTable (DragNode newNode, DragNode parentNode) {	
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "INSERT INTO " + tn_nodeparent + " VALUES ('" + GetParentID(parentNode) + "', '" + newNode.idNumber + "', '" + newNode.transform.position.x + "', '" + newNode.transform.position.y + "', '" + newNode.transform.position.z + "');";
		_dbReader = _dbCommand.ExecuteReader ();
	}

	/***** Edit a node's parent *****/
	public void UpdateNodeParent (DragNode updateThisNode, DragNode newParentNode) {
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "UPDATE " + tn_nodeparent + " SET " + nodeparent_parentID + " = '" + GetParentID (newParentNode) + "' WHERE " + nodeparent_childID + " = " + updateThisNode.idNumber + ";";
		_dbReader = _dbCommand.ExecuteReader ();		
	}
	
	
}
