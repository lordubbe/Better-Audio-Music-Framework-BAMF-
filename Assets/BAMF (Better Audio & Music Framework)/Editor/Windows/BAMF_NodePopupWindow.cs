using UnityEngine;
using UnityEditor;
using System.Collections;

public class BAMF_NodePopupWindow : EditorWindow {
	#region variables
	static BAMF_NodePopupWindow currentPopup;
	string name = "Enter a name here.";
	#endregion

	#region main methods
	public static void InitNodePopup(){
		currentPopup = EditorWindow.GetWindow<BAMF_NodePopupWindow> () as BAMF_NodePopupWindow;
		currentPopup.titleContent = new GUIContent ("Node Popup");
	}

	void OnGUI(){
		GUILayout.Space (20);
		GUILayout.BeginHorizontal ();
		GUILayout.Space (20);

		GUILayout.BeginVertical ();

		EditorGUILayout.LabelField ("Create new Graph.", EditorStyles.boldLabel);
		name = EditorGUILayout.TextField ("Enter name: ", name);
		GUILayout.Space (10);

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Create Graph", GUILayout.Height (40))) {
			if (!string.IsNullOrEmpty (name) && name != "Enter a name here.") {
				BAMF_NodeUtilities.CreateNewGraph (name);
				currentPopup.Close ();
			} else {
				EditorUtility.DisplayDialog ("Node Message:", "Please enter a valid Graph name!", "OK");
			}
		}
		 
		if (GUILayout.Button ("Cancel", GUILayout.Height (40))) {
			currentPopup.Close ();
		}
		GUILayout.EndHorizontal ();

		GUILayout.EndVertical ();

		GUILayout.Space (20);
		GUILayout.EndHorizontal ();
		GUILayout.Space (20);
	}
	#endregion

	#region utility methods
	#endregion

}
