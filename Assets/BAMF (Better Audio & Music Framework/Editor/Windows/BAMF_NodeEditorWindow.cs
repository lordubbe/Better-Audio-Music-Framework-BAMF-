using UnityEngine;
using UnityEditor;
using System.Collections;

public class BAMF_NodeEditorWindow : EditorWindow {

	#region Variables
	public static BAMF_NodeEditorWindow currentWindow;
	public BAMF_NodePropertyView propertyView;
	public BAMF_NodeWorkView workView;

	public BAMF_NodeGraph currentGraph = null;

	public float viewPercentage = 0.75f;
	#endregion

	#region Main Methods
	public static void InitEditorWindow(){
		currentWindow = EditorWindow.GetWindow<BAMF_NodeEditorWindow> () as BAMF_NodeEditorWindow;
		currentWindow.titleContent = new GUIContent ("BAMF Node Editor");
		CreateViews ();
	}

	void OnEnable(){
		
	}

	void OnDestroy(){
		
	}

	void Update(){

	}

	void OnGUI(){
		//make sure that if the views are randomly lost, we recreate them!
		if (propertyView == null || workView == null) {
			CreateViews ();
			return;

		}

		//Get current event and process it!
		Event e = Event.current;
		ProcessEvents (e);

		//Update views
		workView.UpdateView (position, new Rect(0f, 0f, viewPercentage, 1f), e, currentGraph);
		propertyView.UpdateView (new Rect(position.width, position.y, position.width, position.height), new Rect(viewPercentage, 0f, 1f-viewPercentage, 1f), e, currentGraph);

		Repaint ();
	}
	#endregion

	#region Utility Methods
	static void CreateViews(){
		if (currentWindow != null) {
			currentWindow.propertyView = new BAMF_NodePropertyView ();
			currentWindow.workView = new BAMF_NodeWorkView ();
		} else {
			currentWindow = EditorWindow.GetWindow<BAMF_NodeEditorWindow> () as BAMF_NodeEditorWindow;
		}
	}

	void ProcessEvents (Event e)
	{
		//Update view percentage with arrowkeys // TODO: CHANGE TO MOUSE DRAG!!!
		if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftArrow) {
			viewPercentage -= 0.01f;
			e.Use ();
		}else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.RightArrow) {
			viewPercentage += 0.01f;
			e.Use ();
		}
	}
	#endregion
}
