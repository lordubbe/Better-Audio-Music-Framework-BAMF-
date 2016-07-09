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
	public List<float> Parameters;
	#endregion

	#region protected variables
	Rect grabRect;
	Rect StatesRect;
	Rect ParametersRect;
	int margin = 5;
	#endregion

	//?
	bool isGrabbing = false;
	int selectedState;
	float testParam;
	#region constructor
	public BAMF_NodePropertyView () : base ("Game States & Parameters"){}
	#endregion

	void OnEnable(){
		StatesRect = new Rect(viewRect.x+margin, 40, viewRect.width-(2*margin), (viewRect.height/2)-(4*margin));
		ParametersRect = new Rect (viewRect.x + margin, StatesRect.y + StatesRect.height + margin, viewRect.width - (2 * margin), (viewRect.height / 2) - (8 * margin));

	}

	#region main methods
	public override void UpdateView (Rect editorRect, Rect percentageRect, Event e, BAMF_NodeGraph currentGraph)
	{
		base.UpdateView (editorRect, percentageRect, e, currentGraph);
		GUI.Box (viewRect, viewTitle, viewSkin.GetStyle("ViewBG"));

		GUI.Box (StatesRect, "STATES", viewSkin.GetStyle ("NodeContent"));

		GUI.Box (ParametersRect, "PARAMETERS", viewSkin.GetStyle ("NodeContent"));

		//grabbing
		grabRect = new Rect(viewRect.x+margin, StatesRect.y+StatesRect.height, StatesRect.width, margin);

		GUILayout.BeginArea (StatesRect); //STATES
		//GUILayout.Toolbar(0, new string[]{"A", "B", "C", "D"});
		selectedState = GUI.SelectionGrid (new Rect(margin, 30, StatesRect.width-(margin*2), 100), selectedState, new string[]{ "Combat", "Exploration", "Suspense", "Dead", "Power Boost"}, 1, viewSkin.GetStyle("Selection"));
		GUILayout.EndArea ();

		GUILayout.BeginArea (ParametersRect); //PARAMETERS
		GUILayout.Space(30);
		Parameters = new List<float> (){ 0.1f, 0.4f, 0.95f, 0.63f };
		if (Parameters != null) {
			GUILayout.BeginHorizontal ();
			for (int i = 0; i < Parameters.Count; i++) {
				GUILayout.BeginVertical ();
				float w = (ParametersRect.width / (Parameters.Count+1)) * (i+1) - 8;
				Rect sliderRect = new Rect (w, 30, 16, 80);
				Parameters[i] = GUI.VerticalSlider(sliderRect, Parameters[i], 1f, 0f);
				GUIStyle whiteText = new GUIStyle ();
				whiteText.normal.textColor = Color.white;
				whiteText.wordWrap = true;
				string val = Parameters [i].ToString ();
				if (val.Length > 3) {
					val = val.Substring (0, 3);
				}
				GUI.Label (new Rect (sliderRect.x, sliderRect.y + sliderRect.height + 5, (ParametersRect.width / (Parameters.Count+1))-8, 15), val, whiteText);
				GUILayout.EndVertical ();
			}
			GUILayout.EndHorizontal ();
		}
		//testParam = EditorGUI.Slider (new Rect (margin, 30, ParametersRect.width - (margin * 2), 20), testParam, 0, 1);
		//testParam = EditorGUILayout.Knob(new Vector2(40, 40), testParam, 0f, 1f, "", Color.black, new Color(247/255f,148/255f,30/255f), true);
		GUILayout.EndArea ();

		ProcessEvents (e);
	}

	public override void ProcessEvents (Event e)
	{
		base.ProcessEvents (e);
		if (viewRect.Contains (e.mousePosition)) {//mouse is inside this viewRect!
			
		}

		if (grabRect.Contains (e.mousePosition)) {
			EditorGUI.DrawRect (grabRect, new Color (1, 1, 1, 0.1f));
			EditorGUIUtility.AddCursorRect (new Rect (e.mousePosition.x - 50, e.mousePosition.y - 50, 100, 100), MouseCursor.ResizeVertical);
			if (e.type == EventType.MouseDown) {
				isGrabbing = true;
			}
		}
		if (e.type == EventType.MouseUp) {
			isGrabbing = false;
		}

		if (isGrabbing) {
			EditorGUIUtility.AddCursorRect (new Rect(e.mousePosition.x-50, e.mousePosition.y-50,100,100), MouseCursor.ResizeVertical);
			StatesRect.height = viewRect.height*(e.mousePosition.y / viewRect.height)-StatesRect.y;
			ParametersRect.y = StatesRect.y+StatesRect.height + margin;
			ParametersRect.height = viewRect.height - StatesRect.y - margin*2 - StatesRect.height;
		}
	}
	#endregion

	#region utility methods
	#endregion
}
