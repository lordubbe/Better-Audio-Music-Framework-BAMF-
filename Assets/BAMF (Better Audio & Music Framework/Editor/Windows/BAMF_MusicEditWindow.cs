using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System.Reflection;
using System;

public class BAMF_MusicEditWindow : EditorWindow {

	static BAMF_MusicEditWindow currentWindow;
	static BAMF_MusicNode.BAMF_MusicClip node;
	static BAMF_MusicNode.BAMF_MusicClip origNode;
	static int layerNumber;
	static Rect previewRect;
	Rect previewLine;
	static int margin = 5;

	public static bool playPreEntry = true;
	public static bool playPostExit = true;

	public static bool snapToBeats = true;
	public static bool snapToBars;

	static float clipLength;
	static float clipLengthSamples;
	static float beatsPerSec;
	static float beatsInClip;

	static bool preEntrySelected;
	static bool postExitSelected;

	public static bool loop;
	private bool isLooping;
	bool isPaused;
	int newPos;

	public static void InitNodePopup(int layerNo, BAMF_MusicNode.BAMF_MusicClip n){
		currentWindow = EditorWindow.GetWindow<BAMF_MusicEditWindow> () as BAMF_MusicEditWindow;
		currentWindow.titleContent = new GUIContent ("BAMF Cue editor");
		currentWindow.minSize = new Vector2 (500, 323);
		currentWindow.maxSize = new Vector2 (1000, 323);
		node = n; layerNumber = layerNo;
		origNode = n;

		clipLength = node.clip.length;//in seconds
		clipLengthSamples = node.clip.samples;
	}

	void OnGUI(){
		//first update variables to affect changes to BPM and METER
		beatsPerSec = (node.BPM / 60f);
		beatsInClip = clipLength/beatsPerSec * node.Base;

		GUILayout.Space (20);
		GUILayout.BeginHorizontal ();
		//GUILayout.Space (20);
		EditorGUILayout.LabelField ("Edit cues of Layer "+(layerNumber+1)+". '"+node.clipName+"'.", EditorStyles.boldLabel);
		GUILayout.EndHorizontal ();

		//Draw preview
		
		GUILayout.BeginArea (new Rect (0, 30, currentWindow.position.width, 200));
		GUILayout.BeginHorizontal ();

		previewRect = new Rect(0, margin, currentWindow.position.width, 100f);
		
		EditorGUI.DrawRect (previewRect, Color.black + (Color.white * 0.2f));
		GUI.DrawTexture(previewRect, node.preview);
		DrawGrid (beatsInClip);
		DrawPreEntry ();
		DrawPostExit ();

		GUILayout.EndHorizontal ();
		GUILayout.EndArea ();

		GUILayout.Space (100);

		EditCues ();

		//Music settings
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical ();
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
		GUILayout.EndVertical ();

		// Snapping
		GUILayout.Space(90);
		GUILayout.BeginVertical ();
		snapToBeats = EditorGUILayout.ToggleLeft("Snap to beats", snapToBeats);
		snapToBars = EditorGUILayout.ToggleLeft("Snap to bars", snapToBars);
		GUILayout.EndVertical ();

		// pre/post exit toggles
		GUILayout.Space(-40);
		GUILayout.BeginVertical ();
		playPreEntry = EditorGUILayout.ToggleLeft("Play pre-entry", playPreEntry);
		playPostExit = EditorGUILayout.ToggleLeft("Play post-exit", playPostExit);
		GUILayout.EndVertical ();

		GUILayout.EndHorizontal ();

		TransportControls ();

		//Accept / Cancel buttons
		GUILayout.BeginArea(new Rect(margin, currentWindow.position.height-(40+margin), currentWindow.position.width-(margin*2), 40));
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Save", GUILayout.Height (40))) { 
			//save it
			node.parentNode.SetLayer(node, layerNumber); 
			currentWindow.Close ();
		}

		if (GUILayout.Button ("Cancel", GUILayout.Height (40))) {
			node.parentNode.SetLayer(origNode, layerNumber); 
			currentWindow.Close ();
		}
		GUILayout.EndHorizontal ();
		GUILayout.EndArea ();

		Repaint ();
	}

	void DrawGrid(float bars){
		for (int i = 0; i < bars; i++) {
			if (node.Step > 0) {
				if (i % node.Step == 0) {//bars
					EditorGUI.DrawRect (new Rect (position.width / bars * i, previewRect.y, 1, previewRect.height), new Color (255, 255, 255, 0.9f));
				} else {//beats
					EditorGUI.DrawRect (new Rect (position.width / bars * i, previewRect.y, 1, previewRect.height), new Color (255, 255, 255, 0.3f));
				}
			}
		}
	}

	void DrawPreEntry(){
		//pre entry
		Rect top = new Rect((position.width)/clipLengthSamples*node.preEntry-5, previewRect.y-5, 5, 5);
		Rect line = new Rect ((position.width) / clipLengthSamples * node.preEntry, previewRect.y - 5, 1, previewRect.height + 5);
		Rect lineHandle = line; lineHandle.x -= 5; lineHandle.width += 10;
		EditorGUI.DrawRect(line, new Color(0f, 1f, 0f, 1f));//line
		EditorGUI.DrawRect(top, new Color(0f, 1f, 0f, 1f));//top bit
		EditorGUI.DrawRect(new Rect(0, previewRect.y, (position.width/clipLengthSamples)*node.preEntry, previewRect.height), new Color(0f, 1f, 0f, 0.3f));//green alpha box

		Event e = Event.current;
		if (e.type == EventType.MouseDown) {
			if (top.Contains (e.mousePosition) || lineHandle.Contains (e.mousePosition)) {
				preEntrySelected = true;
				e.Use ();
			}
		} else if (top.Contains (e.mousePosition) || lineHandle.Contains (e.mousePosition)) {
			EditorGUI.DrawRect (lineHandle, new Color (1, 1, 1, 0.2f));
		}
		if (e.type == EventType.MouseUp) {
			preEntrySelected = false;
		}
		if (preEntrySelected) {
			float newPreEntry = (e.mousePosition.x/position.width)*clipLengthSamples;
			if (newPreEntry < node.postExit && newPreEntry >= 0) {
				node.preEntry = newPreEntry;
			}
		}
	}

	void DrawPostExit(){
		//post exit
		Rect top = new Rect(position.width/clipLengthSamples*node.postExit, previewRect.y-5, 5, 5);
		Rect line = new Rect (position.width / clipLengthSamples * node.postExit, previewRect.y - 5, 1, previewRect.height + 5);
		Rect lineHandle = line; lineHandle.x -= 5; lineHandle.width += 10;
		EditorGUI.DrawRect(line, new Color(1f, 0f, 0f, 1f));
		EditorGUI.DrawRect(top, new Color(1f, 0f, 0f, 1f));//top bit
		EditorGUI.DrawRect(new Rect(position.width/clipLengthSamples*node.postExit, previewRect.y, position.xMax-(position.width/clipLengthSamples)*node.postExit, previewRect.height), new Color(1f, 0f, 0f, 0.3f));//red alpha box

		Event e = Event.current;
		if (e.type == EventType.MouseDown) {
			if (top.Contains (e.mousePosition) || lineHandle.Contains(e.mousePosition)) {
				postExitSelected = true;
				e.Use ();
			}
		}else if (top.Contains (e.mousePosition) || lineHandle.Contains (e.mousePosition)) {
			EditorGUI.DrawRect (lineHandle, new Color (1, 1, 1, 0.2f));
		}
		if (e.type == EventType.MouseUp) {
			postExitSelected = false;
		}
		if (postExitSelected) {
			float newPostExit = (e.mousePosition.x/position.width)*clipLengthSamples; 
			if (newPostExit > node.preEntry && newPostExit <= node.clip.samples) {
				node.postExit = newPostExit;
			}
		}
	}

	void EditCues(){ //this section takes care of editing the pre and post exits
		if (node != null) {
			//GUILayout.Label (new GUIContent("Entry & Exit Cues", "Here you can edit Entry & Exit markers for the current clip. The Entry marker should be where the 'body' of the clip starts, while the Exit marker is where the 'body' of the clip ends."));
			EditorGUILayout.MinMaxSlider (ref node.preEntry, ref node.postExit, 0f, clipLengthSamples);
			GUI.backgroundColor = Color.white;

			if (!snapToBeats && !snapToBars) {
				//... not much
			} else if (snapToBeats) { //if snapping to beats is enabled then we round the values for the slider, which makes the snapping occur
				float newPre = (clipLengthSamples / beatsInClip) * Mathf.RoundToInt (((node.preEntry / clipLengthSamples) * beatsInClip));
				float newPost = (clipLengthSamples / beatsInClip) * Mathf.RoundToInt (((node.postExit / clipLengthSamples) * beatsInClip));
				node.preEntry = newPre;
				node.postExit = newPost;
			} else if (snapToBars) {//only called if !snapToBeats
				node.preEntry = (clipLengthSamples / (beatsInClip / node.Step)) * Mathf.RoundToInt (((node.preEntry / clipLengthSamples) * (beatsInClip / node.Step)));
				node.postExit = (clipLengthSamples / (beatsInClip / node.Step)) * Mathf.RoundToInt (((node.postExit / clipLengthSamples) * (beatsInClip / node.Step)));
			}
		}
	}
///*
	void TransportControls(){
		if (node != null) {
			GUILayout.Label (new GUIContent("Transport", "These are your Transport Controls with which you can play, pause and stop the clip, as well as test if it loops correctly with itself.")); //just creates the headline
			GUILayout.BeginHorizontal (); 
			GUILayout.Space (position.width / 4);

			GUI.backgroundColor = new Color (0, 0.71f, 0.97f); //make the buttons nice and blue
			GUI.contentColor = Color.white; 

			//now draw the buttons and put the icons on them so it's like people are used to instead of just text
			if (GUILayout.Button (new GUIContent(Resources.Load("Textures/Editor/PLAY") as Texture2D, "PLAY"))) {//PLAY
				if (!IsClipPlaying (node.clip)) { //if it's not already playing then play it
					PlayClip (node.clip);

				} else if (isPaused) {	//if it's paused then just resume playing from where it left off
					ResumeClip (node.clip);
					isPaused = false;
				}
			}
			if(IsClipPlaying (node.clip) && !isPaused){ //if the clip is playing and it's not paused then draw outline
				drawOutline ();
			}
			if (GUILayout.Button (new GUIContent(Resources.Load("Textures/Editor/PAUSE") as Texture2D, "PAUSE"))) {//PAUSE
				if (IsClipPlaying (node.clip)) { //only pause if the clip is actually playing
					PauseClip (node.clip);
					isPaused = true;
				}
			}
			if (isPaused) { //if it's paused then draw outline 
				drawOutline ();
			}
			//GUILayout.Space (10);
			if (GUILayout.Button (new GUIContent(Resources.Load("Textures/Editor/STOP") as Texture2D, "STOP"))) {//STOP
				StopAllClips ();
				if (isPaused) { //if it's paused and STOP is pressed then unpause as well
					isPaused = false;
				}
			}

			if(GUILayout.Button(new GUIContent(Resources.Load("Textures/Editor/LOOPSMALL") as Texture2D, "LOOP"))){
				if (loop) {
					loop = false;
				} else if (!loop) {
					loop = true;
				}
			}
			if (loop) {
				drawOutline ();
			}

			GUI.backgroundColor = Color.white;
			GUI.contentColor = Color.white;

			if (loop && !isLooping) {
				LoopClip (node.clip, true);
				isLooping = true;
			} else if (isLooping && !loop) {
				LoopClip (node.clip, false);
				isLooping = false;
			}
			GUILayout.Space (position.width / 4);
			GUILayout.EndHorizontal ();
			if (!isLooping) { //if the player has not enabled looping then the track will (obviously) not loop, and as such, the biggest features of this plugin can not be previewed
				EditorGUILayout.HelpBox ("Remember to check the LOOP button if you want to test how the music piece loops!", MessageType.Warning); //...so we remind the user to enable it
			} else {
				GUILayout.Space (43); //if looping is enabled then we don't want to display the helpbox, but we also don't want to fuck up the formatting so instead we add some space to keep it consistent
			}

			previewRect.y += 30;
			//Draw preview line
			previewLine = new Rect (0, previewRect.y, 1, previewRect.height); 				//configure the dimension of the preview line (yellow line with top)
			Rect previewLineTop = new Rect (0, previewRect.y - 10, 10, 10); 									//this is the top part
			previewLine.x = position.width * GetClipSamplePosition (node.clip) / clipLengthSamples;	//we set the x position to correspond to the current sample that the track is playing, relative to the music preview
			previewLineTop.x = -4 + position.width * GetClipSamplePosition (node.clip) / clipLengthSamples;
			EditorGUI.DrawRect (previewLine, Color.yellow);	//now actually draw it :b
			EditorGUI.DrawRect (previewLineTop, Color.yellow);

			if (loop) {	//if looping is enabled we will also draw the loop point 
				Rect loopPoint = new Rect (0, previewRect.y-5, 1, previewRect.height + 5);
				loopPoint.x = position.width * (node.postExit - node.preEntry) / clipLengthSamples;
				Rect loopPointArm = new Rect (loopPoint.x, previewRect.y - 5, 3, 1);
				Rect loopLabelPos = new Rect (loopPoint.x + 2, previewRect.y - 16, 60, 30);
				EditorGUI.LabelField (loopLabelPos, "loop point", EditorStyles.whiteLabel);
				EditorGUI.DrawRect (loopPoint, Color.white);
				EditorGUI.DrawRect (loopPointArm, Color.white);
			}

			//CLICKING ON TIMELINE
			Event e = Event.current; //store the current event 
			if (e.type == EventType.mouseDown && previewRect.Contains (e.mousePosition)) { //if this event is 'mouse down' and it's within the music rect then we want to move the playback to another sample
				if (loop && ((e.mousePosition.x / position.width) * clipLengthSamples) < node.postExit - node.preEntry) { //don't allow clicks beyond the loop point
					newPos = (int)((e.mousePosition.x / position.width) * clipLengthSamples); 
					SetClipSamplePosition (node.clip, newPos);
				} else if (!loop) { //if looping is off then we just change the positions
					newPos = (int)((e.mousePosition.x / position.width) * clipLengthSamples); 
					SetClipSamplePosition (node.clip, newPos);
				}
			}
		}
	}
//*/
	void drawOutline(){ //draw outline just gets the last rect drawn (which is why it doesn't need input arguments to draw the correct places
		Rect b = GUILayoutUtility.GetLastRect ();
		EditorGUI.DrawRect(new Rect(b.x-3, b.y-3, b.width+6, 3), Color.yellow); //we just make the outline a thickness of 3 OUTSIDE the last rect
		EditorGUI.DrawRect(new Rect(b.x-3, b.y+b.height, b.width+6, 3), Color.yellow);
		EditorGUI.DrawRect(new Rect(b.x-3, b.y, 3, b.height+3), Color.yellow);
		EditorGUI.DrawRect(new Rect(b.x+b.width, b.y, 3, b.height+3), Color.yellow);
	}

	void PlayClip(AudioClip clip){
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
		MethodInfo method = audioUtilClass.GetMethod("PlayClip", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(AudioClip) }, null);
		method.Invoke(null, new object[] { clip });
	}

	void LoopClip(AudioClip clip, bool on)
	{
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
		MethodInfo method = audioUtilClass.GetMethod("LoopClip", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(AudioClip), typeof(bool) }, null);
		method.Invoke(null, new object[] { clip, on });
	}

	void PauseClip(AudioClip clip) {
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
		MethodInfo method = audioUtilClass.GetMethod("PauseClip", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] {typeof(AudioClip)}, null);
		method.Invoke(null, new object[] {clip});
	}

	void ResumeClip(AudioClip clip) {
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
		MethodInfo method = audioUtilClass.GetMethod("ResumeClip", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] {typeof(AudioClip)}, null);
		method.Invoke(null, new object[] {clip});
	}

	void StopAllClips(){
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
		MethodInfo method = audioUtilClass.GetMethod("StopAllClips", BindingFlags.Static | BindingFlags.Public);
		method.Invoke(null, null);
	}

	void StopClip(AudioClip clip) {
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
		MethodInfo method = audioUtilClass.GetMethod("StopClip", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] {typeof(AudioClip)}, null);
		method.Invoke(null, new object[] {clip});
	}

	float GetClipPosition(AudioClip clip){
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
		MethodInfo method = audioUtilClass.GetMethod("GetClipPosition", BindingFlags.Static | BindingFlags.Public);
		float position = (float)method.Invoke(null, new object[] { clip });
		return position;
	}

	public static int GetClipSamplePosition(AudioClip clip) {
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
		MethodInfo method = audioUtilClass.GetMethod("GetClipSamplePosition", BindingFlags.Static | BindingFlags.Public);

		int position = (int)method.Invoke(null,new object[] {clip});
		return position;
	}

	public static void SetClipSamplePosition(AudioClip clip , int iSamplePosition) {
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
		MethodInfo method = audioUtilClass.GetMethod("SetClipSamplePosition", BindingFlags.Static | BindingFlags.Public);

		method.Invoke(null, new object[] {clip, iSamplePosition});
	}

	public static int GetSampleCount(AudioClip clip) {
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
		MethodInfo method = audioUtilClass.GetMethod("GetSampleCount", BindingFlags.Static | BindingFlags.Public);

		int samples = (int)method.Invoke(null, new object[] {clip});
		return samples;
	}

	bool IsClipPlaying(AudioClip clip){
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
		MethodInfo method = audioUtilClass.GetMethod("IsClipPlaying", BindingFlags.Static | BindingFlags.Public);
		bool playing = (bool)method.Invoke(null, new object[] { clip, });
		return playing;
	}
}
