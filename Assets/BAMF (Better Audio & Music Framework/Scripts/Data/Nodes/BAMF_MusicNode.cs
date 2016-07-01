using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BAMF_MusicNode : BAMF_NodeBase {

	public string musicPieceName;
	//public List<AudioClip> layers;

	public override void InitNode (){
		base.InitNode ();
		nodeType = NodeType.Music;
		nodeRect = new Rect (0f, 0f, 200f, 300f);
		contentRect = new Rect (10f, 30f, 180f, 250);
		outputs.Add(new BAMF_NodeOutput(NodeConnectionType.Add));
	}

	public override void UpdateNode (Event e, Rect viewRect){
		base.UpdateNode (e, viewRect);
	}

	#if UNITY_EDITOR
	public override void UpdateNodeGUI (Event e, Rect viewRect, GUISkin viewSkin){
		base.UpdateNodeGUI (e, viewRect, viewSkin);
		outputs[0].outputRect = new Rect (nodeRect.x + nodeRect.width - 2f, nodeRect.y + (nodeRect.height / 12f) - 9f, 16f, 16f);
		if(GUI.RepeatButton(outputs[0].outputRect, "", viewSkin.GetStyle("NodeInput_"+outputs[0].type.ToString()))) {
			if (parentGraph != null) {
				parentGraph.connectionMode = true;
				parentGraph.clickedNode = this;
				parentGraph.clickedNodeOutputID = 0;
				this.outputs[0].isClicked = true;
			}
		}

		GUILayout.BeginArea (contentRect);
		GUILayout.Label (musicPieceName != null ? musicPieceName : "Drop an Audio Clip below.", viewSkin.GetStyle("NodeContentText")); 
		GUILayout.EndArea ();
		DropArea (new Rect(contentRect.x+5f, contentRect.y+25f, contentRect.width-10f, contentRect.height/6), viewSkin);

	} 

	void DropArea(Rect dropRect, GUISkin viewSkin){
		GUI.Box(dropRect, "", viewSkin.GetStyle("DropArea"));
	}

	#endif
}
