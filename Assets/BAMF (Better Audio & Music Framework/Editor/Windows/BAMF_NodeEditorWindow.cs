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

	//drag window size functionality
	Rect viewPercentageGrabber;
	bool isDragging = false;
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

		//Update views
		viewPercentageGrabber = new Rect(position.width*viewPercentage, 0f, 5, position.height);
		workView.UpdateView (position, new Rect(0f, 0f, viewPercentage, 1f), e, currentGraph);
		propertyView.UpdateView (new Rect(position.width, position.y, position.width, position.height), new Rect(viewPercentage, 0f, 1f-viewPercentage, 1f), e, currentGraph);
		ProcessEvents (e);
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
		if(viewPercentageGrabber.Contains(e.mousePosition)){
			EditorGUI.DrawRect(viewPercentageGrabber, new Color(1,1,1,0.1f));
			if (e.type == EventType.MouseDown) {
				isDragging = true;
			}
			EditorGUIUtility.AddCursorRect (new Rect(e.mousePosition.x-50, e.mousePosition.y-50,100,100), MouseCursor.ResizeHorizontal);
		}
		if (isDragging && e.type == EventType.MouseUp) {
			isDragging = false;
		}

		if (isDragging) {
			EditorGUIUtility.AddCursorRect (new Rect(e.mousePosition.x-50, e.mousePosition.y-50,100,100), MouseCursor.ResizeHorizontal);
			viewPercentage = e.mousePosition.x/position.width;
			if (viewPercentage > 0.95f) {
				viewPercentage = 0.95f;
			}
		}
	}
	#endregion
}
