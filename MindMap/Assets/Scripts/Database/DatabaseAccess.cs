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

	public enum TableType {Node, Connection, NodeConIdentifier};

	/***** Database operators *****/
	private string _constr;
	private IDbConnection _dbConnection;
	private IDbCommand _dbCommand;
	private IDataReader _dbReader;

	/***** Database and table names *****/
	public string dbn_MainDatabase = "CentralGraphDatabase.sqdb";
	public string tn_node = "NodeTable";
	public string tn_connection = "ConnectionTable";
	public string tn_mid = "MidTable";

	public const string node_idNumber = "idNumber";
	public const string node_name = "Name";
	public const string node_desc = "Description";
	public const string node_locX = "locationX";
	public const string node_locY = "locationY";
	public const string node_locZ = "locationZ";

	/***** Table column names and types *****/
	public static string[] colNames_Nodes = new string[6] {node_idNumber,node_name,node_desc,node_locX,node_locY,node_locZ}; 
	public static string[] colTypes_Nodes = new string[6] {"int","text","text","float","float","float"}; 

	public string[] colNames_Connections = new string[4] {"idNumber", "Label", "Thickness", "IsVisible"};
	public string[] colTypes_Connections = new string[4] {"int", "text", "float", "bool"};
	
	public string[] colNames_Mid = new string[2] {"NodeIDNumber", "ConnectionIDNumber"};
	public string[] colTypes_Mid = new string[2] {"int", "int"};


	/************************/
	/*  Database Functions  */
	/************************/

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

	/***** Populate nodes in NodeCreator from node table *****/
	public void ReadNodesFromDatabase () {
		_dbCommand=_dbConnection.CreateCommand();
		_dbCommand.CommandText = "SELECT * FROM " + tn_node;
		_dbReader=_dbCommand.ExecuteReader();

		while(_dbReader.Read()) { 
			Debug.Log("reading...");
			int idNumber = (int)_dbReader.GetValue(0);
			string name = (string)_dbReader.GetValue(1);
			string desc = (string)_dbReader.GetValue(2);
			float posX = (float)((double)_dbReader.GetValue(3));
			float posY = (float)((double)_dbReader.GetValue(4));
			float posZ = (float)((double)_dbReader.GetValue(5));
			NodeCreator.creator.LoadNewNode(idNumber, name, desc, posX, posY, posZ);
		}
	}

	/***** Set a new position for a node when it is created *****/
	public void SetNodePosition (string tableName, 
	                             int idNumber, 
	                             float newX, 
	                             float newY, 
	                             float newZ) {
		_dbCommand=_dbConnection.CreateCommand();
		_dbCommand.CommandText = "UPDATE " + tableName + " SET " + " locationX = '" + newX + "' WHERE " + "idNumber = " + idNumber;
		_dbReader=_dbCommand.ExecuteReader();

		_dbCommand=_dbConnection.CreateCommand();
		_dbCommand.CommandText = "UPDATE " + tableName + " SET " + " locationY = '" + newY + "' WHERE " + "idNumber = " + idNumber;
		_dbReader=_dbCommand.ExecuteReader();

		_dbCommand=_dbConnection.CreateCommand();
		_dbCommand.CommandText = "UPDATE " + tableName + " SET " + " locationZ = '" + newZ + "' WHERE " + "idNumber = " + idNumber;
		_dbReader=_dbCommand.ExecuteReader();
	}

	public void SetStringInTable (string tableName,
	                          	  int idNumber,
	                              string columnName,
	                              string newName) {
		_dbCommand=_dbConnection.CreateCommand();
		_dbCommand.CommandText = "UPDATE " + tableName + " SET " + columnName + " = '" + newName + "' WHERE " + "idNumber = " + idNumber;
		_dbReader=_dbCommand.ExecuteReader();

		Debug.Log ("Set a new string somewhere!");

	}


	
}
