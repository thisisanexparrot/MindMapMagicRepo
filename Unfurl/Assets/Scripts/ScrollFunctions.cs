using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public static class ScrollFunctions
{
	public static void ScrollToTop(this ScrollRect scrollRect)
	{
		scrollRect.normalizedPosition = new Vector2(0, 1);
	}
	public static void ScrollToBottom(this ScrollRect scrollRect)
	{
		scrollRect.normalizedPosition = new Vector2(0, 0);
	}
}
