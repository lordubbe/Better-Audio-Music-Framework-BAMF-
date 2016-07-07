using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[System.Serializable]
public class BAMF_ViewBase {
	
	#region public variables
	public string viewTitle;
	public Rect viewRect;
	#endregion

	#region protected variables
	protected GUISkin viewSkin;
	protected BAMF_NodeGraph currentGraph;
	#endregion

	#region Constructors
	public BAMF_ViewBase(string title){
		viewTitle = title;

	}
	#endregion

	public void OnEnable(){
		GetEditorSkin ();
	}

	#region main methods
	public virtual void UpdateView(Rect editorRect, Rect percentageRect, Event e, BAMF_NodeGraph currentGraph){

		if (viewSkin == null) {
			GetEditorSkin ();
			return;
		}
		//set the current view graph
		this.currentGraph = currentGraph;

		//update view rect
		viewRect = new Rect (editorRect.x * percentageRect.x, editorRect.y * percentageRect.y, editorRect.width * percentageRect.width, editorRect.height * percentageRect.height);


	}

	public virtual void ProcessEvents(Event e){

	}
	#endregion

	#region utility methods
	protected void GetEditorSkin(){
		viewSkin = (GUISkin)Resources.Load ("GUISkins/EditorSkins/NodeEditorSkin");
	}
	#endregion

}
