using UnityEngine;
using System.Collections;

[System.Serializable]
public class Ingr_IsCompleted : Ingredient {
	public new Ingr_Type myType;// = Ingr_Type.IsComplete;
	public bool isComplete;
}


/* Information for DISPLAY ONLY */
public class SuperviseCompleted : MonoBehaviour, ISuperviseIngredient {
	public void SelectedNodeDisplay (DragNode selectedNode) {//, Ingredient i) {
		//Ingr_IsCompleted ic_i = (Ingr_IsCompleted)i;
		print ("Displaying isCompleted here!");
		Ingr_IsCompleted myIngrCompleted = (Ingr_IsCompleted) selectedNode.GetIngredientFromSerialized (Ingr_Type.IsComplete);
		if (myIngrCompleted.isComplete) {
			print ("It's done!");
			//selectedNode.GetComponent<Renderer>().material
		} else {
			print ("Task is not yet completed.");
		}
	}
}
