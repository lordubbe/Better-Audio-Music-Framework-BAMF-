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

	public bool connectionMode = false;
	public BAMF_NodeBase clickedNode = null;
	public int clickedNodeOutputID;

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
			if (clickedNode != null) {
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
							if (nodes [i].inputs [k].inputRect.Contains (e.mousePosition)) {//if mouse down is over an input node (ONLY MOUSE DOWN) TODO: FIX NEW STUFF FOR MOUSE UP
								if (connectionMode) {
									if (clickedNode.outputs[clickedNodeOutputID].type == nodes [i].inputs [k].type) {
										nodes [i].inputs [k].connectedNode = clickedNode;
										nodes [i].inputs [k].connectedNodeOutputID = clickedNodeOutputID;
										nodes [i].inputs [k].isOccupied = nodes [i].inputs [k].connectedNode != null ? true : false;
										connectionMode = false;

										//assign stuff from the output node
										nodes [i].inputs [k].connectedNode.outputs [nodes [i].inputs [k].connectedNodeOutputID].isClicked = false;
										nodes [i].inputs [k].connectedNode.outputs [nodes [i].inputs [k].connectedNodeOutputID].connectedNode = nodes [i];
										nodes [i].inputs [k].connectedNode.outputs [nodes [i].inputs [k].connectedNodeOutputID].connectedNodeInputID = k;
										//clickedNode.outputs[clickedNodeOutputID].isClicked = false;
										//nodes [i].inputs [k].connectedNode.outputs[clickedNodeOutputID].isClicked = false;
										clickedNode = null;
										clickedNodeOutputID = 0;
									}
								} else {
									//
									if (nodes [i].inputs [k].isOccupied) {
										clickedNode = nodes [i].inputs[k].connectedNode; //set clicked node info
										clickedNodeOutputID = nodes [i].inputs [k].connectedNodeOutputID; //set clicked node output idx info
										clickedNode.outputs [clickedNodeOutputID].isClicked = true; // set the output of the clicked node to isClicked
										nodes [i].inputs [k].connectedNode.outputs[clickedNodeOutputID].connectedNode = null;
										nodes [i].inputs [k].connectedNode = null;
										nodes [i].inputs [k].isOccupied = nodes [i].inputs [k].connectedNode != null ? true : false;
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
									if (clickedNode.outputs [clickedNodeOutputID].type == nodes [i].inputs [k].type) {
										nodes [i].inputs [k].connectedNode = clickedNode;//.outputs[clickedNodeOutputID] = clickedNode.outputs[clickedNodeOutputID];
										nodes [i].inputs [k].connectedNodeOutputID = clickedNodeOutputID;//.outputs[clickedNodeOutputID].connectedNodeInputID = k;
										nodes [i].inputs [k].isOccupied = nodes [i].inputs [k].connectedNode != null ? true : false;

										//assign stuff from the output node
										nodes [i].inputs [k].connectedNode.outputs [nodes [i].inputs [k].connectedNodeOutputID].isClicked = false;
										nodes [i].inputs [k].connectedNode.outputs [nodes [i].inputs [k].connectedNodeOutputID].connectedNode = nodes [i];
										nodes [i].inputs [k].connectedNode.outputs [nodes [i].inputs [k].connectedNodeOutputID].connectedNodeInputID = k;
										//clickedNode.outputs[clickedNodeOutputID].isClicked = false;
										//nodes [i].inputs [k].connectedNode.outputs[clickedNodeOutputID].isClicked = false;
										clickedNode = null;
										clickedNodeOutputID = 0;
									}
								}
							}
						}
					}
					clickedNode = null;

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
		if (clickedNode.outputs[clickedNodeOutputID].isClicked) {
			Handles.color = Color.white;
			Vector3 startPos = new Vector3 (clickedNode.outputs[clickedNodeOutputID].outputRect.x + clickedNode.outputs[clickedNodeOutputID].outputRect.width, clickedNode.outputs[clickedNodeOutputID].outputRect.y + clickedNode.outputs[clickedNodeOutputID].outputRect.height / 2, 0);
			Vector3 endPos = new Vector3 (mousePosition.x, mousePosition.y, 0);
			Handles.DrawBezier (startPos, endPos, 
				startPos + Vector3.right * 50, endPos + Vector3.left * 50, Color.white, bezierTex != null ? bezierTex : null, 3f);
		}
		Handles.EndGUI ();
	}

	#endregion
}
