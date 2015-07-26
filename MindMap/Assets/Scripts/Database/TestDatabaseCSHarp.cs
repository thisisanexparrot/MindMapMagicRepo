//using UnityEngine;
//using System.Collections;
//
//public class TestDatabaseCSHarp : MonoBehaviour {
//	public string databaseName = "TestDB_CSharp2.sqdb";
//	public string tableName = "TestTableC";
//	public ArrayList newAL;
//
//	private dbAccess db;
//
//	// Use this for initialization
//	void Start () {
//		print ("Create database...");
//		db = new dbAccess();
//		db.OpenDB(databaseName);
//		string[] columnNames = new string[2] {"Name","Supername"};
//		string[] columnValues = new string[2] {"text","text"};
//		db.CreateTable(tableName,columnNames,columnValues);
//		print ("Created database!");
//	}
//	
//	// Update is called once per frame
//	void Update () {
//		if(Input.GetKeyDown(KeyCode.A)) {
//			print ("Getting the data");
//			newAL = db.ReadFullTable(tableName);
//			print ("Length: " + newAL.Count);
//			foreach (ArrayList al in newAL) {
//				foreach (string s in al) {
//					print(">>> " + s);
//				}
//			}
//		}
//	}
//}
