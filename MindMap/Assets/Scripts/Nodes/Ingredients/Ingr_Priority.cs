using UnityEngine;
using System.Collections;

[System.Serializable]
public class Ingr_Priority : Ingredient {
	public new Ingr_Type myType;// = Ingr_Type.Priority;
	public int myPriority;
}

public class SupervisePriority : MonoBehaviour, ISuperviseIngredient {
	public void SelectedNodeDisplay (DragNode selectedNode, Ingredient i) {
		Ingr_Priority p_i = (Ingr_Priority)i;
		print ("Displaying priority here!");
	}

}
