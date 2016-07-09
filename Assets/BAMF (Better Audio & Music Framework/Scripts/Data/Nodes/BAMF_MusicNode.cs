using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BAMF_MusicNode : BAMF_NodeBase {

	public List<BAMF_MusicClip> layers;
	int musicBPM = 120;
	int musicStep = 4;
	int musicBase = 4;

	//preview rect info
	Rect previewRect;
	Rect iconRect;

	//for object picker
	AudioClip ObjectPickerSelection = null;
	int ObjectPickerID = 0;

	public override void InitNode (){
		base.InitNode ();
		nodeType = NodeType.Music;

		nodeRect = new Rect (0f, 0f, 200f, 300f);
		contentRect = new Rect (10f, 30f, 180f, 250);
		previewRect = new Rect (0f, 0f, contentRect.width, 50f);

		outputs.Add(new BAMF_NodeOutput(NodeConnectionType.Add));
		layers = new List<BAMF_MusicClip> ();
	}

	public override void UpdateNode (Event e, Rect viewRect){
		base.UpdateNode (e, viewRect);
	}

	#if UNITY_EDITOR
	public override void UpdateNodeGUI (Event e, Rect viewRect, GUISkin viewSkin){
		base.UpdateNodeGUI (e, viewRect, viewSkin);
		Rect allMusic = new Rect (contentRect.x, contentRect.y, contentRect.width, (contentRect.height));
		Rect info = new Rect (nodeRect.x+10, nodeRect.y + nodeRect.height - 47, nodeRect.width-2, 75);

		float test = (layers.Count>0 ? 130 : 140) + (70*(layers.Count));
		nodeRect.height = test;

		contentRect.x = nodeRect.x  +5f; contentRect.y = nodeRect.y + 25f; 
		contentRect.width = nodeRect.width - 10f; contentRect.height = nodeRect.height - 73;
		GUI.Box (contentRect, "", viewSkin.GetStyle ("NodeContent"));


		outputs[0].outputRect = new Rect (nodeRect.x + nodeRect.width - 2f, nodeRect.y + (nodeRect.height / 12f) - 9f, 16f, 16f);
		if(GUI.RepeatButton(outputs[0].outputRect, "", viewSkin.GetStyle("NodeInput_"+outputs[0].type.ToString()))) {
			if (parentGraph != null) {
				parentGraph.connectionMode = true;
				parentGraph.clickedNode = this;
				parentGraph.clickedNodeOutputID = 0;
				this.outputs[0].isClicked = true;
			}
		}

		//DRAW INPUTS
		for (int i = 0; i < inputs.Count; i++) {
			if(GUI.RepeatButton(inputs[i].inputRect, "", viewSkin.GetStyle("NodeInput_"+inputs[i].type.ToString()))) {
			}
		}


		if (layers.Count > 0) {
			GUILayout.BeginArea (allMusic);

			// Draw all layers
			for (int i = 0; i < layers.Count; i++) { 
				GUILayout.BeginHorizontal ();
				GUILayout.Label (((i+1).ToString()+". '"+layers[i].clipName+"'"), viewSkin.GetStyle ("NodeContentText"));
				GUILayout.EndHorizontal ();

				Rect lastRect = GUILayoutUtility.GetLastRect (); //REAL
				Rect thisRect = new Rect (5f, (lastRect.y+lastRect.height) + previewRect.y * (1 + i), contentRect.width-10, 50);
				EditorGUI.DrawRect (thisRect, Color.black + (Color.white * 0.2f));
				if (layers [i].preview != null) {
					GUI.DrawTexture (thisRect, layers [i].preview);
				} else {
					while (layers [i].preview == null) {//continuously try to get the preview
						layers [i].preview = AssetPreview.GetAssetPreview (layers [i].clip); 
						System.Threading.Thread.Sleep (15);
					}
					if (layers [i].preview != null) {
						layers [i].preview.filterMode = FilterMode.Point;
					}
				}
				layers [i].prevRect = thisRect;

				//draw cues
				Rect pre = new Rect(5 + layers[i].prevRect.width / layers[i].clip.samples * layers[i].preEntry, layers[i].prevRect.y, 1, layers[i].prevRect.height);
				Rect untilPre = new Rect (5, layers [i].prevRect.y, layers [i].prevRect.width / layers [i].clip.samples * layers [i].preEntry, layers [i].prevRect.height);
				EditorGUI.DrawRect (pre, new Color(0,1,0,0.5f));
				EditorGUI.DrawRect (untilPre, new Color (0, 0, 0, 0.5f));

				Rect post = new Rect (5 + layers [i].prevRect.width / layers [i].clip.samples * layers [i].postExit, layers [i].prevRect.y, 1, layers [i].prevRect.height);
				Rect fromPost = new Rect (post.x+1, post.y, layers[i].prevRect.width - post.x + 4, post.height);
				EditorGUI.DrawRect (fromPost, new Color (0, 0, 0, 0.5f));
				EditorGUI.DrawRect (post, new Color(1,0,0,0.5f));

				layers [i].prevRect.x += nodeRect.x;
				layers [i].prevRect.y += nodeRect.y+30;
				layers [i].layerNumber = i;
				
				GUILayout.Space (thisRect.height); //make space for next layer
			}

			DropArea (new Rect (GUILayoutUtility.GetLastRect ().x+30, GUILayoutUtility.GetLastRect ().y+GUILayoutUtility.GetLastRect ().height+15, contentRect.width - 60f, 30), viewSkin);

			GUILayout.EndArea ();




			//INFO AREA
			{//General info
				GUILayout.BeginArea (info);
				//GUILayout.Label ("Musical Properties", viewSkin.GetStyle ("NodeContentText"));

				GUILayout.BeginHorizontal ();

				GUILayout.BeginVertical ();
				GUILayout.Label ("BPM", viewSkin.GetStyle ("NodeContentTextNonCenter"));
				musicBPM = EditorGUILayout.IntField (musicBPM, viewSkin.GetStyle ("NodeContentTextNonCenter"), GUILayout.MaxHeight (20), GUILayout.MaxWidth (30));
				GUILayout.EndVertical ();

				GUILayout.Space (20);

				GUILayout.BeginVertical ();
				GUILayout.Label ("METER", viewSkin.GetStyle ("NodeContentTextNonCenter"));
				GUILayout.BeginHorizontal ();
				musicStep = EditorGUILayout.IntField (musicStep, viewSkin.GetStyle ("NodeContentTextNonCenter"), GUILayout.MaxWidth (10), GUILayout.MaxHeight (20));
				GUILayout.Label ("/", viewSkin.GetStyle ("NodeContentText"));
				musicBase = EditorGUILayout.IntField (musicBase, viewSkin.GetStyle ("NodeContentTextNonCenter"), GUILayout.MaxWidth (10), GUILayout.MaxHeight (20));
				GUILayout.Space (60);
				GUILayout.EndHorizontal ();

				iconRect = new Rect (info.width - (info.height - 25), 8, info.height - 45, info.height - 45);
				GUI.DrawTexture (iconRect, Resources.Load ("Textures/Editor/icon_combat") as Texture2D);
				handleIconClick ();
				GUILayout.EndVertical ();

				for (int i = 0; i < 5; i++) {
					GUILayout.BeginVertical ();
					GUILayout.Space (60);
					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndArea ();
			}

		} else { //user has yet to drop an Audio Clip
			GUILayout.BeginArea (contentRect);
			GUILayout.Label ("Drop an Audio Clip below.", viewSkin.GetStyle ("NodeContentText")); 
			GUILayout.EndArea ();
			DropArea (new Rect (contentRect.x + 15f, contentRect.y + 25f, contentRect.width - 30f, 30), viewSkin);
		}
	} 

	void DropArea(Rect dropRect, GUISkin viewSkin){
		
		Event e = Event.current;
		GUI.Box(dropRect, "", viewSkin.GetStyle("DropArea")); 

		if (dropRect.Contains (e.mousePosition)) {
			switch (e.type) {
			case EventType.dragUpdated:
			case EventType.dragPerform:
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if (e.type == EventType.dragPerform) {
					DragAndDrop.AcceptDrag ();

					//If the user dropped an audioclip
					if (DragAndDrop.objectReferences [0].GetType () == typeof(AudioClip)) {
						BAMF_MusicClip music = new BAMF_MusicClip ((AudioClip)DragAndDrop.objectReferences [0]);
						AddNewLayer (music);
					}

				}

				break;
//			case EventType.mouseDown: OBJECT PICKER IS BUGGY
//				EditorGUIUtility.ShowObjectPicker<AudioClip> (null, false, "", 0);
//				ObjectPickerID = EditorGUIUtility.GetObjectPickerControlID ();
//				ObjectPickerSelection = (AudioClip)EditorGUIUtility.GetObjectPickerObject ();
//				break;
			}
		}
		if (Event.current.commandName == "ObjectSelectorUpdated") {
			ObjectPickerSelection = (AudioClip)EditorGUIUtility.GetObjectPickerObject ();
		}
		if (Event.current.commandName == "ObjectSelectorClosed" && ObjectPickerSelection != null && EditorGUIUtility.GetObjectPickerControlID() == ObjectPickerID) {
			//selection = (AudioClip)EditorGUIUtility.GetObjectPickerObject ();
			BAMF_MusicClip music = new BAMF_MusicClip (ObjectPickerSelection);
			AddNewLayer (music);
			ObjectPickerSelection = null;
		}
	}

	void handleIconClick(){
		Event e = Event.current;
		if(iconRect.Contains(e.mousePosition)){
			EditorGUIUtility.AddCursorRect (new Rect(e.mousePosition.x-50, e.mousePosition.y-50,100,100), MouseCursor.Link);
			if(e.type == EventType.mouseDown){
				Debug.Log("clicked icon");
			}
		}
	}

	public override List<BAMF_MusicClip> GetLayers ()
	{
		return layers;
	}

	public override void SetLayer (BAMF_MusicClip newClip, int idx)
	{
		layers [idx] = newClip;
	}

	void AddNewLayer (BAMF_MusicClip music)
	{
		music.clipName = music.clip.name;
		music.postExit = music.clip.samples;
		music.parentNode = this;

		//Fetch the preview for this audioclip
		while (music.preview == null) {//continuously try to get the preview
			music.preview = AssetPreview.GetAssetPreview (music.clip); 
			System.Threading.Thread.Sleep (15);
		}
		if (music.preview != null) {
			music.preview.filterMode = FilterMode.Point;
		}
		layers.Add (music);
		BAMF_NodeInput input = new BAMF_NodeInput (NodeConnectionType.MusicClip);
		input.inputRect = new Rect (music.prevRect.x - 16, music.prevRect.y + (music.prevRect.height / 2 - 8), 16, 16);
		input.inputRect.x += nodeRect.x; input.inputRect.y += nodeRect.y+(70*(layers.Count));
		inputs.Add (input);
	}

	#endif

	#region subclasses
	[System.Serializable]
	public class BAMF_MusicClip{
		public AudioClip clip = null;
		public string clipName;
		public Texture2D preview = null;
		public Rect prevRect;
		public float preEntry = 0;
		public float postExit = 0;
		public int BPM = 120;
		public int Step = 4;
		public int Base = 4;

		public int layerNumber;
		public BAMF_MusicNode parentNode;
		public BAMF_MusicClip(AudioClip c){
			clip = c;
		}
	}

	#endregion
}
