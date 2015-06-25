using UnityEngine;
using System.Collections;

[System.Serializable]
public class Ingr_Priority : Ingredient {
	public new Ingr_Type myType;// = Ingr_Type.Priority;
	public int myPriority;
}

public class SupervisePriority : MonoBehaviour, ISuperviseIngredient {

	public SupervisePriority () {

	}

	public void SelectedNodeDisplay (DragNode selectedNode){//, Ingredient i) {
		//Ingr_Priority p_i = (Ingr_Priority)i;
		print ("Displaying priority here!");
		selectedNode.transform.localScale = new Vector3 (2.0f, 4.0f, 4.0f);
	}

}
