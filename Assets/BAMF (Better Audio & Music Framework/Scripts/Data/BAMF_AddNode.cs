using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[System.Serializable]
public class BAMF_AddNode : BAMF_NodeBase {
	#region public variables
	public BAMF_NodeOutput output;
	public BAMF_NodeInput inputA;
	public BAMF_NodeInput inputB;

	public Texture2D bezierTex; //TODO: move to nodebase
	#endregion

	public BAMF_AddNode(){
		output = new BAMF_NodeOutput(NodeConnectionType.Float);

		inputA = new BAMF_NodeInput (NodeConnectionType.Float);
		inputB = new BAMF_NodeInput (NodeConnectionType.Add);

		bezierTex = Resources.Load ("Textures/Editor/bezierCurve") as Texture2D; //TODO: move to nodebase
	}

	#region main methods
	public override void InitNode ()
	{
		base.InitNode ();
		nodeType = NodeType.Add;
		nodeRect = new Rect (10f, 10f, 200f, 65f);

		outputs.Add (output);
		inputs.Add (inputA);
		inputs.Add (inputB);
	}

	public override void UpdateNode (Event e, Rect viewRect)
	{
		base.UpdateNode (e, viewRect);
	}

	#if UNITY_EDITOR
	public override void UpdateNodeGUI (Event e, Rect viewRect, GUISkin viewSkin)
	{
		base.UpdateNodeGUI (e, viewRect, viewSkin);
		output.outputRect = new Rect (nodeRect.x + nodeRect.width, nodeRect.y + (nodeRect.height / 2f) - 9f, 16f, 16f);
		inputA.inputRect = new Rect (nodeRect.x - 16f, nodeRect.y + (nodeRect.height / 3f) - 9f, 16f, 16f);
		inputB.inputRect = new Rect (nodeRect.x - 16f, nodeRect.y + (nodeRect.height / 3f) * 2 - 9f, 16f, 16f);

		if (GUI.RepeatButton (output.outputRect, "", viewSkin.GetStyle("NodeOutput"))) { //output
			//
			if (parentGraph != null) {
				parentGraph.connectionMode = true;
				parentGraph.clickedOutput = this.output;
				this.output.isClicked = true;
			}
		}

		GUI.RepeatButton (inputA.inputRect, "", viewSkin.GetStyle ("NodeInput"));//input A
		GUI.RepeatButton (inputB.inputRect, "", viewSkin.GetStyle ("NodeInput"));//input B

		DrawInputLines ();
	} 
	#endif
	#endregion

	#region utility methods
	void DrawInputLines(){
		Handles.BeginGUI ();
		if (inputA.isOccupied && inputA.connectedOutput != null) {
			Vector3 startPos = new Vector3 (inputA.connectedOutput.outputRect.x + inputA.connectedOutput.outputRect.width, inputA.connectedOutput.outputRect.y + inputA.connectedOutput.outputRect.height / 2, 0);
			Vector3 endPos = new Vector3 (nodeRect.x - 16f, nodeRect.y + (nodeRect.height / 3f), 0);
			Handles.DrawBezier (startPos, endPos, 
				startPos + Vector3.right * 50, endPos + Vector3.left * 50, Color.white, bezierTex != null ? bezierTex : null, 3f);
		} else {
			inputA.isOccupied = false;
		}

		if (inputB.isOccupied && inputB.connectedOutput != null) {
			Vector3 startPos = new Vector3 (inputB.connectedOutput.outputRect.x + inputB.connectedOutput.outputRect.width, inputB.connectedOutput.outputRect.y + inputB.connectedOutput.outputRect.height / 2, 0);
			Vector3 endPos = new Vector3 (nodeRect.x - 16f, nodeRect.y + (nodeRect.height / 3f) * 2, 0);
			Handles.DrawBezier (startPos, endPos, 
				startPos + Vector3.right * 50, endPos + Vector3.left * 50, Color.white, bezierTex != null ? bezierTex : null, 3f);
		} else {
			inputB.isOccupied = false;
		}
		Handles.EndGUI ();
	}
	#endregion
}
