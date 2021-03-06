﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BAMF_GameStatesAndParameters : ScriptableObject {
	public string gameInfoInstanceName = "Game Info";
	public List<BAMF_Parameter> parameters;
	public List<BAMF_State> states;
	public bool isCurrentGameInfo = false;

	public void InitInfo(){
		if (parameters == null) {
			parameters = new List<BAMF_Parameter> ();
		}
		if (states == null) {
			states = new List<BAMF_State> ();
		}
	}

	public void UpdateGameInfo(){
		EditorUtility.SetDirty (this);//save changes to the scriptable object as it is changed in the editorwindow
	}
}
