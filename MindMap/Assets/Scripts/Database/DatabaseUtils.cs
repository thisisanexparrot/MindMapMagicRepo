using UnityEngine;
using System.Collections;

public static class DatabaseUtils {

	/***** Open database *****/
	public static void OpenDatabase_DB (dbAccess db, string p) {
		db.OpenDB (p);
	}

	// NOTE: This will not work in C# because it returns an arraylist with all of the values. 
	/***** Read the entire contents of the database *****/
	public static void ReadFullTable_DB (dbAccess db, string tableName) {
		db.ReadFullTable (tableName);
	}

	/***** Delete all table contents *****/
	public static void DeleteTableContents_DB (dbAccess db, string tableName) {
		db.DeleteTableContents (tableName);
	}

	/***** Create a new table in the given database *****/
	public static void CreateTable_DB (dbAccess db,
	                                   string tableName,
	                                   string[] columns,
	                                   string[] columnTypes) {
		db.CreateTable (tableName, columns, columnTypes);
	}

	/***** Insert one new row with one new value in a given column *****/
	public static void InsertIntoSingle_DB (dbAccess db,
	                                        string tableName,
	                                        string columnName,
	                                        string value) {
		db.InsertIntoSingle (tableName, columnName, value);
	}

	/***** Insert one new row with several new values in the given columns *****/
	public static void InsertIntoSpecific_DB (dbAccess db,
	                                          string tableName,
	                                          string[] columnNames,
	                                          string[] values,
	                                          string[] valueTypes) {
		Debug.Log ("Insert Into Specific: " + valueTypes [1]);
		db.InsertIntoSpecificCaster (tableName, columnNames, values, valueTypes);
		Debug.Log ("Done inserting");
	}

	/***** Insert one new row with all new values in column order *****/
	public static void InsertInto_DB (dbAccess db,
	                                  string tableName,
	                                  string[] values,
	                                  string[] valueTypes) {
		db.InsertIntoCaster (tableName, values, valueTypes);
	}

	public static void SingleSelectWhere_DB (dbAccess db,
	                                         string tableName,
	                                         string itemToSelect,
	                                         string wCol,
	                                         string wPar,
	                                         string wValue) {
		db.SingleSelectWhere (tableName, itemToSelect, wCol, wPar, wValue);
	}

	public static void CloseDatabase_DB (dbAccess db) {
		db.CloseDB ();
	}

	/***** Update specified column with new value *****/
	public static void UpdateColumn_DB (dbAccess db, 
	                               	    string tableName,
	                               	    string column,
	                             	    string newInserted,
	                                    string newInsertedType,
	                                    string wChecker,
	                                    string wPar,
	                                    string wValue) {
		db.UpdateColumn (tableName, column, newInserted, newInsertedType, wChecker, wPar, wValue);
	}

	public static void DeleteFromTableWhere_DB (dbAccess db,
	                                            string tableName,
	                                            string wChecker,
	                                            string wPar,
	                                            string wValue) {
		db.DeleteFromTableWhere (tableName, wChecker, wPar, wValue);
	}

}









