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
	public bool canMove = true;
	public string nodeName;
	public Rect nodeRect;
	public Rect contentRect;
	public NodeType nodeType;
	public BAMF_NodeGraph parentGraph;

	public Texture2D bezierTex; 

	public List<BAMF_NodeInput> inputs;
	public List<BAMF_NodeOutput> outputs;
	#endregion

	#region protected variables
	protected GUISkin nodeSkin;
	#endregion

	public BAMF_NodeBase(){
		inputs = new List<BAMF_NodeInput>();
		outputs = new List<BAMF_NodeOutput>();

	}

	public void OnEnable(){
		bezierTex = Resources.Load ("Textures/Editor/bezierCurve") as Texture2D; //TODO: move to nodebase 
	}

	#region main methods
	public virtual void InitNode(){
		contentRect = new Rect ();
	}

	public virtual void UpdateNode(Event e, Rect viewRect){
		ProcessEvents (e, viewRect);
	}

	#if UNITY_EDITOR
	public virtual void UpdateNodeGUI(Event e, Rect viewRect, GUISkin viewSkin){
		DrawConnections ();
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
		if (isSelected && canMove) {
			if (viewRect.Contains (e.mousePosition)) {
				if (e.type == EventType.mouseDrag && e.button == 0) {
					nodeRect.x += e.delta.x;
					nodeRect.y += e.delta.y;

					if (inputs.Count > 0) {
						for (int i = 0; i < inputs.Count; i++) {
							inputs [i].inputRect.x += e.delta.x;
							inputs [i].inputRect.y += e.delta.y;
						}
					}
					for (int i = 0; i < outputs.Count; i++) {
						outputs [i].outputRect.x += e.delta.x;
						outputs [i].outputRect.y += e.delta.y;
					}
						
				}
			}
		}
	}

	void DrawConnections(){
		Handles.BeginGUI ();
		if (inputs.Count > 0) {
			for (int i = 0; i < inputs.Count; i++) {
				if (inputs [i].isOccupied && inputs [i].connectedNode.outputs [inputs [i].connectedNodeOutputID] != null) {
					Vector3 startPos = new Vector3 (inputs [i].connectedNode.outputs [inputs [i].connectedNodeOutputID].outputRect.x + inputs [i].connectedNode.outputs [inputs [i].connectedNodeOutputID].outputRect.width, inputs [i].connectedNode.outputs [inputs [i].connectedNodeOutputID].outputRect.y + inputs [i].connectedNode.outputs [inputs [i].connectedNodeOutputID].outputRect.height / 2, 0);
					Vector3 endPos = new Vector3 (inputs [i].inputRect.x, inputs [i].inputRect.y + 9f, 0);
					float stiffness = 40;
					float dist = Mathf.Abs(endPos.x-startPos.x);
					//if (dist < 30) {
					//stiffness = Mathf.Clamp(dist, 30, 50);
					//}
					Handles.DrawBezier (startPos, endPos, 
						startPos + Vector3.right * stiffness, endPos + Vector3.left * stiffness, inputs [i].typeColor, bezierTex != null ? bezierTex : null, 3f);
				} else {
					inputs [i].isOccupied = false;
				}
			}
		}
		Handles.EndGUI ();
	}

	#endregion

	#region subclasses
	[System.Serializable]
	public class BAMF_NodeInput{
		public bool isOccupied = false;

		public BAMF_NodeBase connectedNode = null;
		public int connectedNodeOutputID;

		public NodeConnectionType type;
		public Rect inputRect;
		public Color typeColor;

		public BAMF_NodeInput(NodeConnectionType t){
			type = t;
			switch(type){
			case NodeConnectionType.Float:
				typeColor = new Color(247/255f,148/255f,30/255f);
				break;
			case NodeConnectionType.Add:
				typeColor = new Color(0/255f,174/255f,239/255f);
				break;
			case NodeConnectionType.MusicClip:
				typeColor = new Color(61/255f,247/255f,30/255f);
				break;
			default:
				typeColor = Color.white;
				break;
			}
		}
	}

	[System.Serializable]
	public class BAMF_NodeOutput{
		public Rect outputRect;
		public NodeConnectionType type;
		public bool isClicked = false;

		public BAMF_NodeBase connectedNode;
		public int connectedNodeInputID;

		public BAMF_NodeOutput(NodeConnectionType t){
			type = t;
		}
	}

	//Music Piece
	public virtual List<BAMF_MusicNode.BAMF_MusicClip> GetLayers(){
		return null;
	}

	public virtual void SetLayer(BAMF_MusicNode.BAMF_MusicClip newClip, int idx){

	}

	//Parameter Modifier
	public virtual void UpdateParameters(){}

	public virtual float GetParameterValue(){
		return 0f;
	}
	public virtual float EvaluateParameterValue(){
		return 0f;
	}
	public virtual ParameterModifierType GetModifierType(){
		return ParameterModifierType.Volume;
	}

	#endregion




}
