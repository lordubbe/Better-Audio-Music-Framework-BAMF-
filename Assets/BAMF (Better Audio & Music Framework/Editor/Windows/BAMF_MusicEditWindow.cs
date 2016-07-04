using UnityEngine;
using UnityEditor;
using System.Collections;

public class BAMF_MusicEditWindow : EditorWindow {

	static BAMF_MusicEditWindow currentWindow;
	static BAMF_MusicNode.BAMF_MusicClip node;
	static int layerNumber;
	static Rect previewRect;
	static int margin = 5;

	static float clipLength;
	static float clipLengthSamples;
	static float beatsPerSec;
	static float beatsInClip;

	static bool preEntrySelected;
	static bool postExitSelected;

	public static void InitNodePopup(int layerNo, BAMF_MusicNode.BAMF_MusicClip n){
		currentWindow = EditorWindow.GetWindow<BAMF_MusicEditWindow> () as BAMF_MusicEditWindow;
		currentWindow.titleContent = new GUIContent ("BAMF Cue editor");
		node = n; layerNumber = layerNo;

		clipLength = node.clip.length;//in seconds
		clipLengthSamples = node.clip.samples;
		node.preEntry = 10000;
		node.postExit -= 10000;
	}

	void OnGUI(){
		//first update variables to affect changes to BPM and METER
		beatsPerSec = (node.BPM / 60f);
		beatsInClip = clipLength/beatsPerSec * node.Base;

		GUILayout.Space (20);
		GUILayout.BeginHorizontal ();
		GUILayout.Space (20);
		EditorGUILayout.LabelField ("Edit cues of Layer "+(layerNumber+1)+". '"+node.clipName+"'.", EditorStyles.boldLabel);
		GUILayout.EndHorizontal ();

		GUILayout.BeginArea (new Rect (0, 30, currentWindow.position.width, 200));
		GUILayout.BeginHorizontal ();

		//Draw preview
		previewRect = new Rect(0, margin, currentWindow.position.width, 100f);
		
		EditorGUI.DrawRect (previewRect, Color.black + (Color.white * 0.2f));
		GUI.DrawTexture(previewRect, node.preview);
		DrawGrid (beatsInClip);
		DrawPreEntry ();
		DrawPostExit ();

		GUILayout.EndHorizontal ();
		GUILayout.EndArea ();

		GUILayout.Space (100);

		//Music settings
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("BPM", GUILayout.MaxWidth(40));
		node.BPM = EditorGUILayout.IntField (node.BPM, GUILayout.MaxWidth(78));
		//limit bpm to not crash unity
		node.BPM = Mathf.Clamp(node.BPM, 20, 999);

		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("METER", GUILayout.MaxWidth(40));
		node.Step = EditorGUILayout.IntField (node.Step, GUILayout.MaxWidth(30));
		EditorGUILayout.LabelField ("/", GUILayout.MaxWidth (10));
		node.Base = EditorGUILayout.IntField (node.Base, GUILayout.MaxWidth(30));
		GUILayout.EndHorizontal ();

		//Accept / Cancel buttons
		GUILayout.BeginArea(new Rect(margin, currentWindow.position.height-(40+margin), currentWindow.position.width-(margin*2), 40));
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
		GUILayout.EndArea ();

		Repaint ();
	}

	void DrawGrid(float bars){
		//bars
		for (int i = 0; i < bars / 4f; i++) { //these are 'stronger', or, more white than the beats. This is familiar to what's seen in most audio editing software
			EditorGUI.DrawRect (new Rect (position.width / (bars / (float)node.Step) * i, previewRect.y, 1, previewRect.height), new Color (255, 255, 255, 0.6f));
		}
		//beats
		for (int i = 0; i < bars; i++) {
			EditorGUI.DrawRect (new Rect (position.width / bars * i, previewRect.y, 1, previewRect.height), new Color (255, 255, 255, 0.3f));
		}
	}

	void DrawPreEntry(){
		//pre entry
		Rect top = new Rect((position.width)/clipLengthSamples*node.preEntry-5, previewRect.y-5, 5, 5);
		Rect line = new Rect ((position.width) / clipLengthSamples * node.preEntry, previewRect.y - 5, 1, previewRect.height + 5);
		EditorGUI.DrawRect(line, new Color(0f, 1f, 0f, 1f));//line
		EditorGUI.DrawRect(top, new Color(0f, 1f, 0f, 1f));//top bit
		EditorGUI.DrawRect(new Rect(0, previewRect.y, (position.width/clipLengthSamples)*node.preEntry, previewRect.height), new Color(0f, 1f, 0f, 0.3f));//green alpha box

		Event e = Event.current;
		if (e.type == EventType.MouseDown) {
			if (top.Contains (e.mousePosition) || line.Contains(e.mousePosition)) {
				preEntrySelected = true;
				e.Use ();
			}
		}
		if (e.type == EventType.MouseUp) {
			preEntrySelected = false;
		}
		if (preEntrySelected) {
			float newPreEntry = (e.mousePosition.x/position.width)*clipLengthSamples; 
			node.preEntry = newPreEntry;
		}
	}

	void DrawPostExit(){
		//post exit
		Rect top = new Rect(position.width/clipLengthSamples*node.postExit, previewRect.y-5, 5, 5);
		Rect line = new Rect (position.width / clipLengthSamples * node.postExit, previewRect.y - 5, 1, previewRect.height + 5);
		EditorGUI.DrawRect(line, new Color(1f, 0f, 0f, 1f));
		EditorGUI.DrawRect(top, new Color(1f, 0f, 0f, 1f));//top bit
		EditorGUI.DrawRect(new Rect(position.width/clipLengthSamples*node.postExit, previewRect.y, position.xMax-(position.width/clipLengthSamples)*node.postExit, previewRect.height), new Color(1f, 0f, 0f, 0.3f));//red alpha box

		Event e = Event.current;
		if (e.type == EventType.MouseDown) {
			if (top.Contains (e.mousePosition) || line.Contains(e.mousePosition)) {
				postExitSelected = true;
				e.Use ();
			}
		}
		if (e.type == EventType.MouseUp) {
			postExitSelected = false;
		}
		if (postExitSelected) {
			float newPostExit = (e.mousePosition.x/position.width)*clipLengthSamples; 
			node.postExit = newPostExit;
		}
	}
}
