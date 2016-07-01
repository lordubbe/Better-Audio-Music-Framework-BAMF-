using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BAMF_NodeGraph : ScriptableObject {
	#region public variables
	public string graphName = "New Graph";
	public List<BAMF_NodeBase> nodes;
	public BAMF_NodeBase selectedNode;

	public bool selectMode = false;
	public bool connectionMode = false;
	public BAMF_NodeBase.BAMF_NodeOutput clickedOutput = null;

	public Texture2D bezierTex;
	#endregion

	#region main methods
	void OnEnable(){
		if (nodes == null) {
			nodes = new List<BAMF_NodeBase> ();
		}
		bezierTex = Resources.Load ("Textures/Editor/bezierCurve") as Texture2D;
	}

	public void InitGraph(){
		if (nodes.Count > 0) {
			//go through nodes
			for (int i = 0; i < nodes.Count; i++) {
				nodes [i].InitNode ();
			}
		}
	}

	public void UpdateGraph(){
		if (nodes.Count > 0) {
			//update logic for updating the nodes
		}
	}

	#if UNITY_EDITOR
	public void UpdateGraphGUI(Event e, Rect viewRect, GUISkin viewSkin){
		if (nodes.Count > 0) {
			//update logic for updating the nodes
			ProcessEvents(e, viewRect);
			for (int i = 0; i < nodes.Count; i++) {
				nodes [i].UpdateNodeGUI (e, viewRect, viewSkin);
			}
		}

		//if in connectionmode
		if (connectionMode) {
			if (clickedOutput != null) {
				DrawConnectionToMouse (e.mousePosition);
			}
		}

		EditorUtility.SetDirty (this);//save changes to the scriptable object as it is changed in the editorwindow
	}
	#endif

	#endregion

	#region utility methods
	void ProcessEvents(Event e, Rect viewRect){
		if(viewRect.Contains(e.mousePosition)){//if the event happens inside this graph
			if (e.button == 0) {//left click
				if (e.type == EventType.MouseDown) {
					DeselectAllNodes ();
					bool setNode = false;
					selectedNode = null;
					for (int i = 0; i < nodes.Count; i++) {
						if (nodes [i].nodeRect.Contains (e.mousePosition)) { //if clicking a node
							nodes [i].isSelected = true;
							selectedNode = nodes [i];//now we know which node is clicked
							setNode = true; // then we should select the node
						}
					}

					for (int i = 0; i < nodes.Count; i++) {//check if over input
						for (int k = 0; k < nodes [i].inputs.Count; k++) {
							if (nodes [i].inputs [k].inputRect.Contains (e.mousePosition)) {//if mouse down is over a input node
								if (connectionMode) {
									if (clickedOutput.type == nodes [i].inputs [k].type) {
										nodes [i].inputs [k].connectedOutput = clickedOutput;
										nodes [i].inputs [k].isOccupied = nodes [i].inputs [k].connectedOutput != null ? true : false;
										connectionMode = false;
										clickedOutput.isClicked = false;
										nodes [i].inputs [k].connectedOutput.isClicked = false;
										clickedOutput = null;
									}
								} else {
									//
									if (nodes [i].inputs [k].isOccupied) {
										clickedOutput = nodes [i].inputs [k].connectedOutput;
										clickedOutput.isClicked = true;
										nodes [i].inputs [k].connectedOutput = null;
										nodes [i].inputs [k].isOccupied = nodes [i].inputs [k].connectedOutput != null ? true : false;
										connectionMode = true;
									}
								}
							}
						}
					}
					

					if (!setNode) {
						DeselectAllNodes ();
					}
				
//					if (connectionMode) {
//						connectionMode = false;
//					}

				} else if (e.type == EventType.MouseUp) {
					if (connectionMode) {
						for (int i = 0; i < nodes.Count; i++) {//check if not over output
							for (int k = 0; k < nodes [i].inputs.Count; k++) {
								if (nodes [i].inputs [k].inputRect.Contains (e.mousePosition)) {//if mouse up is over a input node
									nodes [i].inputs [k].connectedOutput = clickedOutput;
									nodes [i].inputs [k].connectedOutput.connectedNode = nodes [i];
									nodes [i].inputs [k].connectedOutput.connectedNodeInputID = k;
									nodes [i].inputs [k].isOccupied = nodes [i].inputs [k].connectedOutput != null ? true : false;
									connectionMode = false;
									clickedOutput.isClicked = false;
									nodes [i].inputs [k].connectedOutput.isClicked = false;
									clickedOutput = null;
								}
							}
						}
					}
					clickedOutput = null;
					for (int i = 0; i < nodes.Count; i++) {
						for (int j = 0; j < nodes [i].outputs.Count; j++) {
							nodes [i].outputs [j].isClicked = false;
						}
					}
					if (connectionMode) {
						connectionMode = false;
					}
				} 
			}
		}
	}

	void DeselectAllNodes(){
		for (int i = 0; i < nodes.Count; i++) {
			nodes [i].isSelected = false;
		}
	}

	void DrawConnectionToMouse(Vector2 mousePosition){
		Handles.BeginGUI ();
		if (clickedOutput.isClicked) {
			Handles.color = Color.white;
			Vector3 startPos = new Vector3 (clickedOutput.outputRect.x + clickedOutput.outputRect.width, clickedOutput.outputRect.y + clickedOutput.outputRect.height / 2, 0);
			Vector3 endPos = new Vector3 (mousePosition.x, mousePosition.y, 0);
			Handles.DrawBezier (startPos, endPos, 
				startPos + Vector3.right * 50, endPos + Vector3.left * 50, Color.white, bezierTex != null ? bezierTex : null, 3f);
		}
		Handles.EndGUI ();
	}

	#endregion
}
