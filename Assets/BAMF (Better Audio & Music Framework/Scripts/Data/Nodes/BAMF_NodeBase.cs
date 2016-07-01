using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BAMF_NodeBase : ScriptableObject {//want it to be attached to an asset
	#region public variables
	public bool isSelected = false;
	public string nodeName;
	public Rect nodeRect;
	public NodeType nodeType;
	public BAMF_NodeGraph parentGraph;

	public List<BAMF_NodeInput> inputs;
	public List<BAMF_NodeOutput> outputs;
	#endregion

	#region protected variables
	protected GUISkin nodeSkin;
	#endregion

	#region main methods
	public virtual void InitNode(){
		inputs = new List<BAMF_NodeInput>();
		outputs = new List<BAMF_NodeOutput>();
	}

	public virtual void UpdateNode(Event e, Rect viewRect){
		ProcessEvents (e, viewRect);
	}

	#if UNITY_EDITOR
	public virtual void UpdateNodeGUI(Event e, Rect viewRect, GUISkin viewSkin){
		ProcessEvents (e, viewRect);
		if (isSelected) {
			GUI.Box (nodeRect, nodeName, viewSkin.GetStyle ("NodeActive"));
		} else {
			GUI.Box (nodeRect, nodeName, viewSkin.GetStyle ("NodeDefault"));
		}

		EditorUtility.SetDirty (this);
	}
	#endif

	#endregion

	#region utility methods
	void ProcessEvents(Event e, Rect viewRect){
		if (isSelected) {
			if (viewRect.Contains (e.mousePosition)) {
				if (e.type == EventType.mouseDrag) {
					nodeRect.x += e.delta.x;
					nodeRect.y += e.delta.y;

					if (inputs.Count > 0) {
						for (int i = 0; i < inputs.Count; i++) {
							inputs [i].inputRect.x += e.delta.x;
							inputs [i].inputRect.y += e.delta.y;
						}

						for (int i = 0; i < outputs.Count; i++) {
							outputs [i].outputRect.x += e.delta.x;
							outputs [i].outputRect.y += e.delta.y;
						}
					}
				}
			}
		}
	}
	#endregion

	#region subclasses
	[System.Serializable]
	public class BAMF_NodeInput{
		public bool isOccupied = false;
		public BAMF_NodeOutput connectedOutput = null;
		public NodeConnectionType type;
		public Rect inputRect;

		public BAMF_NodeInput(NodeConnectionType t){
			type = t;
		}
	}

	[System.Serializable]
	public class BAMF_NodeOutput{
		public Rect outputRect;
		public NodeConnectionType type;
		public bool isClicked = false;
		public BAMF_NodeOutput(NodeConnectionType t){
			type = t;
		}
	}

	#endregion




}
