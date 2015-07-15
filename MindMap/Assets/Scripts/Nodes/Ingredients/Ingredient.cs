using UnityEngine;
using System.Collections;


public enum Ingr_Type{Priority, IsComplete};

[System.Serializable]
public class Ingredient {
	public Ingr_Type myType;
}


/* Information for DISPLAY ONLY */
public interface ISuperviseIngredient {
	void SelectedNodeDisplay (DragNode selectedNode);//, Ingredient i); 
}