using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[System.Serializable]
public class BAMF_FloatNode : BAMF_NodeBase {
	#region public variables
	#endregion

	public BAMF_FloatNode(){
	}

	#region main methods
	public override void InitNode ()
	{
		base.InitNode ();
		nodeType = NodeType.Float;
		nodeRect = new Rect (10f, 10f, 150f, 65f);
		outputs.Add (new BAMF_NodeOutput(NodeConnectionType.Float));
	}

	public override void UpdateNode (Event e, Rect viewRect)
	{
		base.UpdateNode (e, viewRect);
	}

	#if UNITY_EDITOR
	public override void UpdateNodeGUI (Event e, Rect viewRect, GUISkin viewSkin)
	{
		base.UpdateNodeGUI (e, viewRect, viewSkin);
		outputs[0].outputRect = new Rect (nodeRect.x + nodeRect.width - 2f, nodeRect.y + (nodeRect.height / 2f) - 9f, 16f, 16f);
		if(GUI.RepeatButton(outputs[0].outputRect, "", viewSkin.GetStyle("NodeInput_"+outputs[0].type.ToString()))) {
		//if (GUI.Button (new Rect (nodeRect.x + nodeRect.width -2f, nodeRect.y + (nodeRect.height / 2f) - 9f, 16f, 16f), "", viewSkin.GetStyle("NodeOutput"))) {//TODO: Maybe make REPEAT BUTTON (while held down)
			//
			if (parentGraph != null) {
				parentGraph.connectionMode = true;
				parentGraph.clickedNode = this;
				parentGraph.clickedNodeOutputID = 0;
				this.outputs[0].isClicked = true;
			}
		}
	} 
	#endif
	#endregion

	#region utility methods
	#endregion
}
