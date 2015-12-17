using UnityEngine;
using System.Collections;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;
using System;
using UnityEngine.UI;


public class DatabaseUtils : MonoBehaviour {

	/***** Database operators *****/
	private string _constr;
	private IDbConnection _dbConnection;
	private IDbCommand _dbCommand;
	private IDataReader _dbReader;
	public const string dbn_MainDatabase = "MediaDatabase.sqdb";

	/***** Database data *****/
	public const string metaTableName = "MetaTable";
	public const string numberOfEntries = "numberOfEntries";

	public const string tableName = "MediaTable";
	public const string idNumber = "idNumber";
	public const string title = "title";
	public const string description = "description";
	public const string dateFirstAccessed = "dateFirstAccessed";
	public const string timeTaken = "timeTaken";
	public const string suggestedBy = "suggestedBy";
	public const string completed = "isCompleted";
	public const string thoughts = "thoughts";
	public const string tags = "tags";
	public const string wasDeleted = "wasDeleted";
	public const string mediaType = "mediaType";


	public int localNumberOfEntries;
	
	public Dictionary<int, string> localMediaList;
	public bool addingNewEntry;

	/***** Text Entry Boxes *****/
	public InputField field_title;
	public InputField field_description;
	public InputField field_thoughts;
	public InputField field_tags;
	public InputField field_type;


	/***** Currently edited entry *****/
	public int c_idNumber;
	public string c_title;
	public string c_description;
	public string c_dateFirstAccessed;
	public MediaContent c_buttonFromList;
	
	/***** Table column names and types *****/
	public static string[] colNames = new string[11] {idNumber, title, description, dateFirstAccessed, timeTaken, suggestedBy, completed, thoughts, tags, wasDeleted, mediaType}; 
	public static string[] colTypes = new string[11] {"int", "text","text","text","real","text","text","text","text","int","text"}; 

	public static string[] metaColNames = new string[1] {numberOfEntries};
	public static string[] metaColTypes = new string[1] {"int"};

	
	public static DatabaseUtils centralDatabase;
	public CreateScrollList centralScrollList;



	void Awake () 
	{
		if (centralDatabase == null)
		{
			DontDestroyOnLoad(gameObject);
			centralDatabase = this;
		}
		else if (centralDatabase != this)
		{
			Destroy(gameObject);
		}

		OpenDatabase (dbn_MainDatabase);
		CreateOriginalTables ();
		CreateInitialCounterRow ();
		ReadDataOnAwake ();
		centralScrollList = GetComponent<CreateScrollList> ();
		OpenNewMediaOption ();
	}

	/***** Open an existing database *****/
	public void OpenDatabase (string p) 
	{
		_constr = "URI=file:" + p; 
		_dbConnection = new SqliteConnection(_constr);
		_dbConnection.Open();
	}

	public void CreateOriginalTables () 
	{
		_dbCommand = _dbConnection.CreateCommand();
		string query = "CREATE TABLE " + tableName + "(" + colNames[0] + " " + colTypes[0];
		for (int i = 1; i < colNames.Length; i++) {
			query += ", " + colNames[i] + " " + colTypes[i];
		}
		query += ")";

		if (query.Length > 0) {
			_dbCommand.CommandText = query;
			try {
				_dbReader = _dbCommand.ExecuteReader ();
			} catch (Exception e) {
				Debug.Log ("Don't worry; a media table already exists!");
			}
		}

		_dbCommand = _dbConnection.CreateCommand();
		query = "CREATE TABLE " + metaTableName + "(" + metaColNames[0] + " " + metaColTypes[0];
		for (int i = 1; i < metaColNames.Length; i++) {
			query += ", " + metaColNames[i] + " " + metaColTypes[i];
		}
		query += ")";
		
		if (query.Length > 0) {
			_dbCommand.CommandText = query;
			try {
				_dbReader = _dbCommand.ExecuteReader ();
			} catch (Exception e) {
				Debug.Log ("Don't worry; a media table already exists!");
			}
		}

	}

	public void ReadDataOnAwake () 
	{
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "SELECT * FROM " + metaTableName;
		_dbReader = _dbCommand.ExecuteReader();
		
		while (_dbReader.Read()) { 
			int db_NumberOfEntries = (int)_dbReader.GetValue(0);
			localNumberOfEntries = db_NumberOfEntries;
		}

		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "SELECT * FROM " + tableName;
		_dbReader = _dbCommand.ExecuteReader();

		localMediaList = new Dictionary<int, string> ();
		while (_dbReader.Read()) { 
			int nextIdNumber = (int)_dbReader.GetValue(0);
			string nextName = (string)_dbReader.GetValue(1);
			int nextWasDeleted = (int)_dbReader.GetValue(9);
			if(nextWasDeleted == 0) {
				localMediaList.Add(nextIdNumber, nextName);
			}
		}
	}

	/***** Populate the meta table's row on first creation *****/
	public void CreateInitialCounterRow () {
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "SELECT Count(*) FROM " + metaTableName + ";";
		_dbReader = _dbCommand.ExecuteReader ();
		
		_dbReader.Read ();
		int rowCount = (int)_dbReader.GetInt32 (0);
		
		if (rowCount < 1) {
			_dbCommand = _dbConnection.CreateCommand ();
			_dbCommand.CommandText = "INSERT INTO " + metaTableName + " VALUES ('1');";
			_dbReader = _dbCommand.ExecuteReader ();
		} 
	}

	public void OpenNewMediaOption () {
		addingNewEntry = true;
		field_title.text = "";
		field_description.text = "";
		field_thoughts.text = "";
		field_tags.text = "";
		field_type.text = "";
	}

	/***** If an item is selected from the list, update the detail panel *****/
	public void SelectExistingMediaFromList (int selectedIDNumber, MediaContent buttonClickedFromList) {
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "SELECT * FROM " + tableName + " WHERE " + idNumber + " = '" + selectedIDNumber + "';";
		_dbReader = _dbCommand.ExecuteReader ();

		while (_dbReader.Read()) { 
			if(_dbReader.GetValue(1) != null) {field_title.text = (string)_dbReader.GetValue(1);}
			else {field_title.text = "";}
			if(_dbReader.GetValue(2) != null) {field_description.text = (string)_dbReader.GetValue(2);}
			else {field_description.text = "";}
			if(_dbReader.GetValue(7) != null) {field_thoughts.text = (string)_dbReader.GetValue(7);}
			else {field_thoughts.text = "";}
			if(_dbReader.GetValue(10) != null) {field_type.text = (string)_dbReader.GetValue(10);}
			else {field_type.text = "";}
		}

		c_idNumber = selectedIDNumber;
		c_title = field_title.text;
		c_description = field_description.text;
		c_buttonFromList = buttonClickedFromList;

		addingNewEntry = false;

	}

	public void InitiateTitleEdit ()
	{
		if (addingNewEntry) {
			AddNewMedia ();
		} else {
			EditMediaName();
		}
	}

	/***** Triggered by adding new media *****/
	public void AddNewMedia () 
	{
		string newTitle = field_title.text;
		newTitle = newTitle.Replace("'", "''");

		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "INSERT INTO " + tableName + " ('" + idNumber + "', '" + title + "', '" + wasDeleted + "') VALUES ('" + localNumberOfEntries + "', '" + newTitle + "', '" + 0 +"');";
		_dbReader = _dbCommand.ExecuteReader ();

		c_buttonFromList = centralScrollList.AddNewItemToList(field_title.text, localNumberOfEntries);
		c_idNumber = localNumberOfEntries;
		c_title = field_title.text;
		c_description = field_description.text;
		addingNewEntry = false;

		IncrementNumberOfEntries ();
	}

	/***** Edit an existing piece of media's name *****/
	public void EditMediaName () 
	{
		string newTitle = field_title.text;
		newTitle = newTitle.Replace("'", "''");

		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "UPDATE " + tableName + " SET " + title + " = '" + newTitle + "' WHERE " + idNumber + " = " + c_idNumber;
		_dbReader = _dbCommand.ExecuteReader();

		c_buttonFromList.nameLabel.text = field_title.text;
	}

	/***** Edit an existing piece of media's description *****/
	public void EditMediaDescription () 
	{
		string newDesc = field_description.text;
		newDesc = newDesc.Replace("'", "''");

		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "UPDATE " + tableName + " SET " + description + " = '" + newDesc + "' WHERE " + idNumber + " = " + c_idNumber;
		_dbReader = _dbCommand.ExecuteReader();
	}

	/***** Edit an existing piece of media's type *****/
	public void EditMediaType () 
	{
		string newType = field_type.text;
		newType = newType.Replace("'", "''");
		
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "UPDATE " + tableName + " SET " + mediaType + " = '" + newType + "' WHERE " + idNumber + " = " + c_idNumber;
		_dbReader = _dbCommand.ExecuteReader();
	}

	/***** Increment the total number of media entries made by the user *****/
	public void IncrementNumberOfEntries () 
	{
		localNumberOfEntries++;
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "UPDATE " + metaTableName + " SET " + numberOfEntries + " = '" + localNumberOfEntries + "';"; 
		_dbReader = _dbCommand.ExecuteReader();

	}

	/***** Mark entry for deletion but DO NOT yet remove from database *****/
	public void DeleteEntry () 
	{
		_dbCommand = _dbConnection.CreateCommand ();
		_dbCommand.CommandText = "UPDATE " + tableName + " SET " + wasDeleted + " = '" + 1 + "' WHERE " + idNumber + " = " + c_idNumber;
		_dbReader = _dbCommand.ExecuteReader();

		Destroy (c_buttonFromList.gameObject);

		field_title.text = "";
		field_description.text = "";
		field_tags.text = "";
		field_thoughts.text = "";
		field_type.text = "";

		c_title = "";
		c_idNumber = 0;
		c_description = "";
		c_buttonFromList = null;

	}
	


}
