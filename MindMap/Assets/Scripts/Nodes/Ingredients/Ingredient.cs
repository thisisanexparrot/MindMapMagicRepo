using UnityEngine;
using System.Collections;


public enum Ingr_Type{Priority, IsComplete};

[System.Serializable]
public class Ingredient {
	public Ingr_Type myType;
}

public interface ISuperviseIngredient {
	void SelectedNodeDisplay (DragNode selectedNode, Ingredient i); 
}