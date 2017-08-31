using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
public class BAMF_StateManager : BAMF_NodeBase {

	public List<BAMF_State> states;
	BAMF_GameStatesAndParameters gameInfo;
	public List<BAMF_State> possibleStates;

	public override void InitNode ()
	{
		base.InitNode ();
		nodeType = NodeType.StateManager;
		nodeRect = new Rect (0, 0, 200, 300);
		contentRect = new Rect (10, 30, 180, 250);

		inputs.Add (new BAMF_NodeInput (NodeConnectionType.MusicPiece));
	}

	#if UNITY_EDITOR
	public override void UpdateNodeGUI (Event e, Rect viewRect, GUISkin viewSkin)
	{
		base.UpdateNodeGUI (e, viewRect, viewSkin);
		contentRect.x = nodeRect.x + 5; contentRect.y = nodeRect.y + 25;
		contentRect.width = nodeRect.width - 10; contentRect.height = nodeRect.height - 30;
		//Draw content rect
		GUI.Box (contentRect, "", viewSkin.GetStyle ("NodeContent"));

		//DRAW INPUTS
		for (int i = 0; i < inputs.Count; i++) {
			if(GUI.RepeatButton(inputs[i].inputRect, "", viewSkin.GetStyle("NodeInput_"+inputs[i].type.ToString()))) {
			}
		}
	}
	#endif

	public override void UpdateStates ()
	{
		BAMF_GameStatesAndParameters[] gameInfos = Resources.FindObjectsOfTypeAll<BAMF_GameStatesAndParameters>();
		if (gameInfos != null) {
			for (int i = 0; i < gameInfos.Length; i++) {
				if (gameInfos [i].isCurrentGameInfo) {
					gameInfo = gameInfos [i];
					if (gameInfos [i].states != null) {
						possibleStates = gameInfos [i].states;
					}
				}
			}
		}
	}

}
