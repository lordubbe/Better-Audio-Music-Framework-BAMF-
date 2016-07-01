using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[System.Serializable]
public class BAMF_FloatNode : BAMF_NodeBase {
	#region public variables
	public BAMF_NodeOutput output;
	#endregion

	public BAMF_FloatNode(){
		output = new BAMF_NodeOutput(NodeConnectionType.Float);
	}

	#region main methods
	public override void InitNode ()
	{
		base.InitNode ();
		nodeType = NodeType.Float;
		nodeRect = new Rect (10f, 10f, 150f, 65f);
		outputs.Add (output);
	}

	public override void UpdateNode (Event e, Rect viewRect)
	{
		base.UpdateNode (e, viewRect);
	}

	#if UNITY_EDITOR
	public override void UpdateNodeGUI (Event e, Rect viewRect, GUISkin viewSkin)
	{
		base.UpdateNodeGUI (e, viewRect, viewSkin);
		output.outputRect = new Rect (nodeRect.x + nodeRect.width - 2f, nodeRect.y + (nodeRect.height / 2f) - 9f, 16f, 16f);
		if(GUI.RepeatButton(output.outputRect, "", viewSkin.GetStyle("NodeInput_"+output.type.ToString()))) {
		//if (GUI.Button (new Rect (nodeRect.x + nodeRect.width -2f, nodeRect.y + (nodeRect.height / 2f) - 9f, 16f, 16f), "", viewSkin.GetStyle("NodeOutput"))) {//TODO: Maybe make REPEAT BUTTON (while held down)
			//
			if (parentGraph != null) {
				parentGraph.connectionMode = true;
				parentGraph.clickedOutput = this.output;
				this.output.isClicked = true;
			}
		}
	} 
	#endif
	#endregion

	#region utility methods
	#endregion
}
