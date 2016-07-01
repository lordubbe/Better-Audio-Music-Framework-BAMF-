using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BAMF_NodePropertyView : BAMF_ViewBase {
	#region public variables
	#endregion

	#region protected variables
	#endregion

	#region constructor
	public BAMF_NodePropertyView () : base ("Properties"){}
	#endregion

	#region main methods
	public override void UpdateView (Rect editorRect, Rect percentageRect, Event e, BAMF_NodeGraph currentGraph)
	{
		base.UpdateView (editorRect, percentageRect, e, currentGraph);
		GUI.Box (viewRect, viewTitle, viewSkin.GetStyle("ViewBG"));

		GUILayout.BeginArea (viewRect);

		GUILayout.EndArea ();

		ProcessEvents (e);
	}

	public override void ProcessEvents (Event e)
	{
		base.ProcessEvents (e);

		if (viewRect.Contains (e.mousePosition)) {//mouse is inside this viewRect!

		}

	}
	#endregion

	#region utility methods
	#endregion
}
