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
//	public List<BAMF_Parameter> Parameters; 
	#endregion

	#region protected variables
	Rect grabRect;
	Rect StatesRect;
	Rect ParametersRect;
	int margin = 5;
	#endregion

	//program variables
	bool isGrabbing = false;
	int selectedState;

	#region constructor
	public BAMF_NodePropertyView () : base ("Game States & Parameters"){}
	#endregion

	void OnEnable(){
		StatesRect = new Rect(viewRect.x+margin, 40, viewRect.width-(2*margin), (viewRect.height/2)-(4*margin));
		ParametersRect = new Rect (viewRect.x + margin, StatesRect.y + StatesRect.height + margin, viewRect.width - (2 * margin), (viewRect.height / 2) - (8 * margin));
	}

	#region main methods
	public override void UpdateView (Rect editorRect, Rect percentageRect, Event e, BAMF_NodeGraph currentGraph, BAMF_GameStatesAndParameters currentGameInfo)
	{
		base.UpdateView (editorRect, percentageRect, e, currentGraph, currentGameInfo); 
		GUI.Box (viewRect, viewTitle, viewSkin.GetStyle ("ViewBG"));

		if (currentGameInfo != null) {
						StatesRect.width = viewRect.width - (2 * margin); 
			StatesRect.x = viewRect.x + margin;
			GUI.Box (StatesRect, "STATES", viewSkin.GetStyle ("NodeContent"));

			ParametersRect.width = viewRect.width - (2 * margin);
			ParametersRect.x = viewRect.x + margin;
			GUI.Box (ParametersRect, "PARAMETERS", viewSkin.GetStyle ("NodeContent"));

			//grabbing
			grabRect = new Rect (viewRect.x + margin, StatesRect.y + StatesRect.height, StatesRect.width, margin);

			GUILayout.BeginArea (StatesRect); //STATES 
			//GUILayout.Toolbar(0, new string[]{"A", "B", "C", "D"});
			selectedState = GUI.SelectionGrid (new Rect (margin, 30, StatesRect.width - (margin * 2), 100), selectedState, new string[] { 
				"Combat",
				"Exploration",
				"Suspense",
				"Dead",
				"Power Boost"
			}, 1, viewSkin.GetStyle ("Selection"));
			GUILayout.EndArea ();

			GUILayout.BeginArea (ParametersRect); //PARAMETERS
			GUILayout.Space (30);
//			if (currentGameInfo.parameters == null) {
//				currentGameInfo.parameters = new List<BAMF_Parameter> ();
//				for (int i = 0; i < 4; i++) {
//					BAMF_Parameter p = new BAMF_Parameter (UnityEngine.Random.Range (0f, 1f), "Parameter " + i.ToString ());
//					currentGameInfo.parameters.Add (p);
//				}
//			}
			//set paramsPerLine dynamically according to width
			int paramsPerLine = 3;
			if (ParametersRect.width >= 175) {
				paramsPerLine = 4;
			} else if (ParametersRect.width >= 125) {
				paramsPerLine = 3;
			} else if (ParametersRect.width >= 75) {
				paramsPerLine = 2;
			}//Debug.Log (ParametersRect.width);

			if (currentGameInfo.parameters != null) {
				GUILayout.BeginHorizontal ();
				//NICE BUT STUPID TEXT CLIPPING !
//			int lineNumber = 0;
//			for (int i = 0; i < Parameters.Count; i++) {
//				if (i % paramsPerLine == 0) {
//					lineNumber++;
//				}
//				//GUILayout.BeginVertical ();
//				float w = (ParametersRect.width / (paramsPerLine+1)) * ((i%paramsPerLine)+1) - 8;
//				Rect sliderRect = new Rect (w, 30+((25+80)*(lineNumber-1)), 16, 80);
//				Parameters[i] = GUI.VerticalSlider(sliderRect, Parameters[i], 1f, 0f);
//				GUIStyle whiteText = new GUIStyle ();
//				whiteText.normal.textColor = Color.white;
//				whiteText.wordWrap = true;
//				string val = Parameters [i].ToString ();
//				if (val.Length > 3) {
//					val = val.Substring (0, 3);
//				}
//				GUI.Label (new Rect (sliderRect.x, sliderRect.y + sliderRect.height + 5, (ParametersRect.width / (paramsPerLine+1))-8, 15), val, whiteText);
//
//				whiteText.wordWrap = false;
//				whiteText.clipping = TextClipping.Overflow;
//				GUIUtility.RotateAroundPivot (-90, new Vector2 (sliderRect.x, sliderRect.y+sliderRect.height));
//				GUI.Label (new Rect (sliderRect.x, sliderRect.y+sliderRect.height-15, 80, 30), "Parameter " + i.ToString () + ".", whiteText);
//				GUI.matrix = Matrix4x4.identity;
//
//				//GUILayout.EndVertical ();
//			}
				// SECOND TRY !
				GUILayout.BeginVertical ();
				GUIStyle grayText = new GUIStyle ();
				grayText.normal.textColor = new Color (0.7f, 0.7f, 0.7f, 1);
				for (int i = 0; i < currentGameInfo.parameters.Count; i++) {
					GUILayout.BeginHorizontal ();
					Rect sliderRect = new Rect (margin, 15 + 30 * (i + 1), ParametersRect.width - (margin * 2 + 35), 15);
					GUI.backgroundColor = new Color (247 / 255f, 148 / 255f, 30 / 255f);
					currentGameInfo.parameters [i].value = EditorGUI.FloatField (new Rect (sliderRect.x + margin + sliderRect.width, sliderRect.y, ParametersRect.width - sliderRect.width - margin * 3, sliderRect.height), currentGameInfo.parameters [i].value);
					GUILayout.EndHorizontal ();
					Rect labelRect = new Rect (sliderRect.x, sliderRect.y - 10, sliderRect.width, sliderRect.height);
					GUI.Label (labelRect, "Parameter " + i.ToString (), grayText);
					currentGameInfo.parameters [i].value = GUI.HorizontalSlider (sliderRect, currentGameInfo.parameters [i].value, 0f, 1f);
					GUI.backgroundColor = Color.white;
				}
				GUILayout.Space (margin*2 + 30 * currentGameInfo.parameters.Count);
				GUILayout.BeginHorizontal ();
				GUILayout.Space (margin*10);
				if (GUILayout.Button ("", viewSkin.GetStyle ("DropArea"), GUILayout.MaxHeight (30), GUILayout.MaxWidth (120))) {
					BAMF_Parameter p = new BAMF_Parameter (0f, "Parameter " + currentGameInfo.parameters.Count.ToString ());
					currentGameInfo.parameters.Add (p);
				}
				GUILayout.Space (margin*10);
				GUILayout.EndHorizontal ();

				GUILayout.EndVertical ();

				GUILayout.EndHorizontal ();
			}
			GUILayout.EndArea ();

			ProcessEvents (e);
		} else {
			if (viewRect.Contains (e.mousePosition)) {
				if (e.button == 1) {//right click
					if (e.type == EventType.MouseDown) {
						ProcessContextMenu (e);
					}
				}
			}
		}
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
	void ProcessContextMenu(Event e){
		GenericMenu menu = new GenericMenu ();
		menu.AddItem (new GUIContent ("Create New Game Info Instance"), false, ContextCallback, "newGameInfo");
		menu.AddItem (new GUIContent ("Load Game Info Instance"), false, ContextCallback, "loadGameInfo");
		menu.ShowAsContext ();
		e.Use ();
	}

	void ContextCallback(object obj){
		switch (obj.ToString ()) {
		case "newGameInfo":
			BAMF_NodeUtilities.CreateNewGameInfo ("New Game Info"); 
			break;
		case "loadGameInfo":
			BAMF_NodeUtilities.LoadGameInfo ();
			break;
		default:
			//
			break;
		}
	}
	#endregion

	#region subclasses
//	public class BAMF_Parameter{
//		public float value;
//		public string name;
//
//		public BAMF_Parameter(float v, string s){
//			value = v; 
//			name = s;
//		}
//	}
	#endregion
}