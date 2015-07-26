﻿using UnityEngine;
using System.Collections;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;

public class DatabaseAccess {

	public NodeCreator theCreator;

	private string _constr;
	private IDbConnection _dbConnection;
	private IDbCommand _dbCommand;
	private IDataReader _dbReader;

	public void OpenDatabase (string p, NodeCreator n) {
		_constr = "URI=file:" + p; 
		_dbConnection = new SqliteConnection(_constr);
		_dbConnection.Open();

		theCreator = n;
	}

	public void ReadNodesFromDatabase () {
		_dbCommand=_dbConnection.CreateCommand();
		_dbCommand.CommandText = "SELECT * FROM NodeTable;";
		_dbReader=_dbCommand.ExecuteReader();
		
		Debug.Log (_dbReader.FieldCount);
		
		//ArrayList readArray = new ArrayList();
		while(_dbReader.Read()) { 
			ArrayList lineArray = new ArrayList();
			int idNumber = (int)_dbReader.GetValue(0);
			string name = (string)_dbReader.GetValue(1);
			string desc = (string)_dbReader.GetValue(2);

			Debug.Log(_dbReader.GetValue(3) + " is of type: " + _dbReader.GetValue(3).GetType());
			float posX = (float)((double)_dbReader.GetValue(3));
			float posY = (float)((double)_dbReader.GetValue(4));
			float posZ = (float)((double)_dbReader.GetValue(5));
			theCreator.LoadNewNode(idNumber, name, desc, posX, posY, posZ);

			//for (int i = 0; i < _dbReader.FieldCount; i++) {
				//Debug.Log("#####> " + _dbReader.GetValue(i));
				//lineArray.Add(_dbReader.GetValue(i)); // This reads the entries in a row
			//}
			//readArray.Add(lineArray); // This makes an array of all the rows
		}
		//ArrayList firstTwoAdded = (ArrayList)readArray [0];
		//int myI = (int)firstTwoAdded [0];
		//Debug.Log ("ADDED: " + myI);

		//List<DragNode> databaseNodes = new List<DragNode> ();
		//return databaseNodes;
	}


	
}