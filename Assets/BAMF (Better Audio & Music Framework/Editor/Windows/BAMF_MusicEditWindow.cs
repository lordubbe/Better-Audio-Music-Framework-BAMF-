using UnityEngine;
using UnityEditor;
using System.Collections;

public class BAMF_MusicEditWindow : EditorWindow {

	static BAMF_MusicEditWindow currentWindow;
	static BAMF_MusicNode.BAMF_MusicClip  node;
	static int layerNumber;

	public static void InitNodePopup(int layerNo, BAMF_MusicNode.BAMF_MusicClip n){
		currentWindow = EditorWindow.GetWindow<BAMF_MusicEditWindow> () as BAMF_MusicEditWindow;
		currentWindow.titleContent = new GUIContent ("Edit cues for Layer " + (layerNo+1) + ". '" + n.clipName + "'.");
		node = n; layerNumber = layerNo;
	}

	void OnGUI(){
		GUILayout.Space (20);
		GUILayout.BeginHorizontal ();
		GUILayout.Space (20);
		EditorGUILayout.LabelField ("Edit cues of Layer "+(layerNumber+1)+". '"+node.clipName+"'.", EditorStyles.boldLabel);

		GUILayout.EndHorizontal ();


		//Accept / Cancel buttons
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Save", GUILayout.Height (40))) { 
			//save it
			node.parentNode.SetLayer(node, layerNumber); 
			currentWindow.Close ();
		}

		if (GUILayout.Button ("Cancel", GUILayout.Height (40))) {
			currentWindow.Close ();
		}
		GUILayout.EndHorizontal ();
	}
}
