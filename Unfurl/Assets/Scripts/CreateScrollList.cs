using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MediaItem 
{
	public string name;
	public int idNumber;
	public Button.ButtonClickedEvent clickResult;
}

public class CreateScrollList : MonoBehaviour {
	public GameObject buttonTemplate;
	public ScrollRect scrollBox;
	public Transform contentPanel;
	public DatabaseUtils databaseAccess;
	public InputField simpleSearchField;
	public List<MediaContent> allButtons;
	public Text resultsNumberDisplay;
	
	void Start () {
		databaseAccess = GetComponent<DatabaseUtils> ();
		PopulateList ();
	}

	/***** Initially populate list on runtime *****/
	void PopulateList () {
		foreach (var databaseItem in databaseAccess.localMediaList) {
			AddNewItemToList(databaseItem.Value, databaseItem.Key);
		}
		resultsNumberDisplay.text = allButtons.Count.ToString();
	}

	/***** Add a new item to the list of buttons *****/
	public MediaContent AddNewItemToList (string newNameLabel, int newIdNumber) {
		GameObject newButton = Instantiate (buttonTemplate) as GameObject;
		MediaContent nextMediaContent = newButton.GetComponent <MediaContent> ();
		
		nextMediaContent.nameLabel.text = newNameLabel;
		nextMediaContent.idNumber = newIdNumber;
		nextMediaContent.button.onClick.AddListener(delegate {MediaListClickEvent(nextMediaContent.idNumber, nextMediaContent);});
		
		newButton.transform.SetParent(contentPanel);
		allButtons.Add (nextMediaContent);

		Canvas.ForceUpdateCanvases();
		scrollBox.verticalNormalizedPosition = 0;
		Canvas.ForceUpdateCanvases();

		return nextMediaContent;
	}

	/***** Simple search based on name of media *****/
	public void SimpleNameSearch () {
		foreach (MediaContent b in allButtons) {
			Destroy(b.gameObject);
		}
		allButtons.Clear ();

		string searchString = simpleSearchField.text;
		Debug.Log (searchString);

		foreach (var databaseItem in databaseAccess.localMediaList) {

			bool contains = databaseItem.Value.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
			if(contains) {
				AddNewItemToList(databaseItem.Value, databaseItem.Key);
			}
		}
		resultsNumberDisplay.text = allButtons.Count.ToString();
	}

	/***** On clicking a title, populate the details section from the database *****/
	public void MediaListClickEvent (int clickedIDNumber, MediaContent contentButton) {
		databaseAccess.SelectExistingMediaFromList (clickedIDNumber, contentButton);
	}



}
