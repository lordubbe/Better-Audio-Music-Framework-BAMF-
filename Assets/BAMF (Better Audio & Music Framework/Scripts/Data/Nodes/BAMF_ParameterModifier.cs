using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

public class BAMF_ParameterModifier : BAMF_NodeBase {

	public AnimationCurve curve;
	public ParameterModifierType type;
	BAMF_GameStatesAndParameters gameInfo;
	public List<BAMF_Parameter> possibleParameters;
	public BAMF_Parameter parameter;
	int selectedParameter = 0;

	public override void InitNode(){
		base.InitNode ();
		nodeType = NodeType.ParameterModifier;

		nodeRect = new Rect (0, 0, 150, 100);
		contentRect = new Rect (10, 30, 130, 80);

		outputs.Add (new BAMF_NodeOutput (NodeConnectionType.MusicClip));

		curve = new AnimationCurve (new Keyframe[]{new Keyframe(0,0), new Keyframe(1,1)});
		curve.SmoothTangents (0, 0); curve.SmoothTangents (1, 0);
	}

	#if UNITY_EDITOR
	public override void UpdateNodeGUI (Event e, Rect viewRect, GUISkin viewSkin)
	{
		base.UpdateNodeGUI (e, viewRect, viewSkin);
		contentRect.x = nodeRect.x  +5f; contentRect.y = nodeRect.y + 25f; 
		contentRect.width = nodeRect.width - 10f; contentRect.height = nodeRect.height-30;
		GUI.Box (contentRect, "", viewSkin.GetStyle ("NodeContent"));

		outputs[0].outputRect = new Rect (nodeRect.x + nodeRect.width - 2f, nodeRect.y+5, 16f, 16f);
		if(GUI.RepeatButton(outputs[0].outputRect, "", viewSkin.GetStyle("NodeInput_"+outputs[0].type.ToString()))) {
			if (parentGraph != null) {
				parentGraph.connectionMode = true;
				parentGraph.clickedNode = this;
				parentGraph.clickedNodeOutputID = 0;
				this.outputs[0].isClicked = true;
			}
		}

		GUILayout.BeginArea (contentRect);
		curve = EditorGUI.CurveField (new Rect (5, 5, contentRect.width - 10, 40), curve);
		GUILayout.Space (50);

		GUILayout.BeginHorizontal ();
		GUILayout.Space (5);
		type = (ParameterModifierType) EditorGUILayout.EnumPopup (type);

		//find current game states 
		if (possibleParameters == null || gameInfo == null) {
			UpdateParameters ();
		} else {
			if (gameInfo.parameters.Count > 0) {
				possibleParameters = gameInfo.parameters;
			}
		}
		if (possibleParameters != null && selectedParameter >= possibleParameters.Count) {
			Debug.Log (selectedParameter+", but count is: "+possibleParameters.Count+"... "+parameter.value);
		}
		if (possibleParameters != null && possibleParameters.Count > 0 && selectedParameter != null) {
			string[] paramStrings = new string[possibleParameters.Count];
			for (int i = 0; i < possibleParameters.Count; i++) {
				paramStrings [i] = possibleParameters [i].name;
			}
			selectedParameter = EditorGUILayout.Popup (selectedParameter, paramStrings);

			if (possibleParameters != null && possibleParameters.Count > 0 && selectedParameter != null && selectedParameter < possibleParameters.Count) {
				parameter = possibleParameters [selectedParameter];
			} else {
				parameter = possibleParameters [0];
				selectedParameter = 0;
			}
		} else {
			GUIStyle whiteText = new GUIStyle ();
			whiteText.normal.textColor = Color.white;
			//whiteText.alignment = TextAnchor.UpperCenter;
			EditorGUILayout.LabelField ("No params.", whiteText, GUILayout.MaxWidth(contentRect.width/2-10));
		}

//		BAMF_Parameter[] paramz = ScriptableObject.FindObjectsOfType<BAMF_Parameter>();
//		string[] paramStrings = new string[paramz.Length];
//		if (paramz != null && paramz.Length > 0) {
//			for (int i = 0; i < paramz.Length; i++) {
//				paramStrings [i] = paramz [i].name;
//			}
//			selectedParameter = EditorGUILayout.Popup (selectedParameter, paramStrings);
//			parameter = paramz [selectedParameter];
//		}

		if (parameter != null && possibleParameters != null && possibleParameters.Count > 0) {
			EditorGUI.DrawRect (new Rect (5 + parameter.value * (contentRect.width - 10), 5+1, 1, 37), Color.white);
		}

		GUILayout.Space (5);
		GUILayout.EndHorizontal ();
		GUILayout.EndArea ();
	}


	public override void UpdateParameters(){
		BAMF_GameStatesAndParameters[] gameInfos = Resources.FindObjectsOfTypeAll<BAMF_GameStatesAndParameters>();
		if (gameInfos != null) {
			for (int i = 0; i < gameInfos.Length; i++) {
				if (gameInfos [i].isCurrentGameInfo) {
					gameInfo = gameInfos [i];
					if (gameInfos [i].parameters != null) {
						possibleParameters = gameInfos [i].parameters;
					}
				}
			}
		}
	}
	#endif
}
