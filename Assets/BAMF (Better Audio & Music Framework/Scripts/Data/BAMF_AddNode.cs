using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[System.Serializable]
public class BAMF_AddNode : BAMF_NodeBase {
	#region public variables
//	public BAMF_NodeOutput output;
//	public BAMF_NodeInput inputA;
//	public BAMF_NodeInput inputB;

	#endregion

	public BAMF_AddNode(){
//		output = new BAMF_NodeOutput(NodeConnectionType.Float);
//
//		inputA = new BAMF_NodeInput (NodeConnectionType.Float);
//		inputB = new BAMF_NodeInput (NodeConnectionType.Add);
	}

	#region main methods
	public override void InitNode ()
	{
		base.InitNode ();
		nodeType = NodeType.Add;
		nodeRect = new Rect (10f, 10f, 200f, 65f);

		outputs.Add (new BAMF_NodeOutput(NodeConnectionType.Float));
		inputs.Add (new BAMF_NodeInput (NodeConnectionType.Float));
		inputs.Add (new BAMF_NodeInput (NodeConnectionType.Add));
	}

	public override void UpdateNode (Event e, Rect viewRect)
	{
		base.UpdateNode (e, viewRect);
	}

	#if UNITY_EDITOR
	public override void UpdateNodeGUI (Event e, Rect viewRect, GUISkin viewSkin)
	{
		base.UpdateNodeGUI (e, viewRect, viewSkin);
		outputs[0].outputRect = new Rect (nodeRect.x + nodeRect.width, nodeRect.y + (nodeRect.height / 2f) - 9f, 16f, 16f);
		inputs[0].inputRect = new Rect (nodeRect.x - 16f, nodeRect.y + (nodeRect.height / 3f) - 9f, 16f, 16f);
		inputs[1].inputRect = new Rect (nodeRect.x - 16f, nodeRect.y + (nodeRect.height / 3f) * 2 - 9f, 16f, 16f);

		if (GUI.RepeatButton (outputs[0].outputRect, "", viewSkin.GetStyle("NodeInput_"+outputs[0].type.ToString()))) { //output
			//
			if (parentGraph != null) {
				parentGraph.connectionMode = true;
				parentGraph.clickedOutput = this.outputs[0];
				this.outputs[0].isClicked = true;
			}
		}

		GUI.RepeatButton (inputs[0].inputRect, "", viewSkin.GetStyle ("NodeInput_"+inputs[0].type.ToString()));//input A
		GUI.RepeatButton (inputs[1].inputRect, "", viewSkin.GetStyle ("NodeInput_"+inputs[1].type.ToString()));//input B
	} 
	#endif
	#endregion
}
