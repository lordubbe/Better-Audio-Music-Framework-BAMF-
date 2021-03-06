﻿using UnityEngine;
using UnityEditor;
using System.Collections;

public static class BAMF_NodeUtilities {

	public static void CreateNewGameInfo(string name){
		BAMF_GameStatesAndParameters currentInfo = ScriptableObject.CreateInstance<BAMF_GameStatesAndParameters> () as BAMF_GameStatesAndParameters;
		if (currentInfo != null) {
			currentInfo.name = name;
			setAllInfoNotCurrent ();
			currentInfo.isCurrentGameInfo = true;
			currentInfo.InitInfo ();

			AssetDatabase.CreateAsset (currentInfo, "Assets/BAMF (Better Audio & Music Framework/Database/" + name + ".asset");
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();

			BAMF_NodeEditorWindow currentWindow = EditorWindow.GetWindow<BAMF_NodeEditorWindow> () as BAMF_NodeEditorWindow;
			if (currentWindow != null) {
				currentWindow.currentGameInfo = currentInfo;
			}
		} else {
			EditorUtility.DisplayDialog ("Whoops!", "Unable to create new game info. Sorry.", "OK");
		}
	}

	public static void CreateNewGraph(string name){

		BAMF_NodeGraph currentGraph = ScriptableObject.CreateInstance<BAMF_NodeGraph> () as BAMF_NodeGraph;
		if (currentGraph != null) {
			currentGraph.graphName = name;
			currentGraph.InitGraph ();

			AssetDatabase.CreateAsset (currentGraph, "Assets/BAMF (Better Audio & Music Framework/Database/" + name + ".asset");
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();

			BAMF_NodeEditorWindow currentWindow = EditorWindow.GetWindow<BAMF_NodeEditorWindow> () as BAMF_NodeEditorWindow;
			if (currentWindow != null) {
				currentWindow.currentGraph = currentGraph;
			}
		} else {
			EditorUtility.DisplayDialog ("ERROR:", "Unable to create new graph. Sorry.", "OK");
		}
	}

	public static void UnloadGraph(){
		BAMF_NodeEditorWindow currentWindow = EditorWindow.GetWindow<BAMF_NodeEditorWindow> () as BAMF_NodeEditorWindow;
		if (currentWindow != null) {
			currentWindow.currentGraph = null;
		}
	}

	public static void UnloadGameInfo(){
		BAMF_NodeEditorWindow currentWindow = EditorWindow.GetWindow<BAMF_NodeEditorWindow> () as BAMF_NodeEditorWindow;
		if (currentWindow != null) {
			setAllInfoNotCurrent ();
			currentWindow.currentGameInfo = null;
		}
	}

	public static void LoadGraph(){ //TODO: COPY FROM AAE TO MAKE DRAG & DROP FUNCTIONALITY
		BAMF_NodeGraph currentGraph = null;
		string graphPath = EditorUtility.OpenFilePanel ("Load Graph", Application.dataPath + "/BAMF (Better Audio & Music Framework/Database/Database", "");
		int appPathLength = Application.dataPath.Length;
		string finalPath = graphPath.Substring (appPathLength - 6);
		currentGraph = AssetDatabase.LoadAssetAtPath (finalPath, typeof(BAMF_NodeGraph)) as BAMF_NodeGraph;
		if (currentGraph != null) {
			BAMF_NodeEditorWindow currentWindow = EditorWindow.GetWindow<BAMF_NodeEditorWindow> () as BAMF_NodeEditorWindow;
			if (currentWindow != null) {
				currentWindow.currentGraph = currentGraph;
			}
		} else {
			EditorUtility.DisplayDialog ("ERROR:", "Unable to load selected item (not a graph?)", "OK");
		}
	}

	public static void LoadGameInfo(){
		BAMF_GameStatesAndParameters currentInfo = null;
		string graphPath = EditorUtility.OpenFilePanel("Load Game States & Parameters", Application.dataPath + "/BAMF (Better Audio & Music Framework/Database/Database", "");
		int appPathLength = Application.dataPath.Length;
		string finalPath = graphPath.Substring (appPathLength - 6); 
		currentInfo = AssetDatabase.LoadAssetAtPath (finalPath, typeof(BAMF_GameStatesAndParameters)) as BAMF_GameStatesAndParameters;
		if (currentInfo != null) {
			BAMF_NodeEditorWindow currentWindow = EditorWindow.GetWindow<BAMF_NodeEditorWindow> () as BAMF_NodeEditorWindow;
			if (currentWindow != null) {
				setAllInfoNotCurrent ();
				currentInfo.isCurrentGameInfo = true;
				currentWindow.currentGameInfo = currentInfo;
			}
		} else {
			EditorUtility.DisplayDialog ("ERROR:", "Unable to load selected item (not Game States & Parameters?)", "OK");
		}
	}

	public static void CreateParameter(BAMF_GameStatesAndParameters currentInfo){
		if (currentInfo != null) {
			BAMF_Parameter p = ScriptableObject.CreateInstance<BAMF_Parameter> () as BAMF_Parameter;
			p.name = "Parameter " + currentInfo.parameters.Count.ToString ();
			p.value = 0f;
			currentInfo.parameters.Add (p);
			AssetDatabase.AddObjectToAsset (p, currentInfo);
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
		}
	}

	public static void CreateState(BAMF_GameStatesAndParameters currentInfo){
		if (currentInfo != null) {
			BAMF_State s = ScriptableObject.CreateInstance<BAMF_State> () as BAMF_State;
			s.name = "State " + currentInfo.states.Count.ToString ();
			s.isActive = false;
			currentInfo.states.Add (s);
			AssetDatabase.AddObjectToAsset (s, currentInfo);
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
		}
	}

	//NODES
	public static void CreateNode(BAMF_NodeGraph currentGraph, NodeType nodeType, Vector2 mousePos){
		if (currentGraph != null) {
			BAMF_NodeBase currentNode = null;
			switch (nodeType) {
			case NodeType.Float:
				//
				currentNode = ScriptableObject.CreateInstance<BAMF_FloatNode> () as BAMF_FloatNode;
				currentNode.nodeName = nodeType.ToString () + " Node";
				break;
			case NodeType.Add:
				currentNode = ScriptableObject.CreateInstance<BAMF_AddNode> () as BAMF_AddNode;
				currentNode.nodeName = nodeType.ToString () + " Node";
				break;
			case NodeType.Music:
				//
				currentNode = ScriptableObject.CreateInstance<BAMF_MusicNode> () as BAMF_MusicNode;
				//currentNode.nodeName = "M U S I C  P I E C E"; 
				currentNode.nodeName = "Music Piece"; 
				break;
			case NodeType.ParameterModifier:
				//
				currentNode = ScriptableObject.CreateInstance<BAMF_ParameterModifier> () as BAMF_ParameterModifier;
				//currentNode.nodeName = "P A R A M E T E R";
				currentNode.nodeName = "Parameter Modifier";
				break;
			case NodeType.StateManager:
				//
				currentNode = ScriptableObject.CreateInstance<BAMF_StateManager> () as BAMF_StateManager;
				currentNode.nodeName = "State Manager";
				break;
			default: 
				//
				break;
			}

			if (currentNode != null) {
				currentNode.InitNode ();
				currentNode.nodeRect.x = mousePos.x;
				currentNode.nodeRect.y = mousePos.y;
				currentNode.parentGraph = currentGraph;
				currentNode.name = currentNode.nodeName;
				currentGraph.nodes.Add (currentNode);

				AssetDatabase.AddObjectToAsset (currentNode, currentGraph);
				AssetDatabase.SaveAssets ();
				AssetDatabase.Refresh ();
			}
		}
	}

	public static void DeleteNode(int deleteNodeIdx, BAMF_NodeGraph currentGraph){
		if (currentGraph != null) {
			if (currentGraph.nodes.Count >= deleteNodeIdx) {
				BAMF_NodeBase deleteNode = currentGraph.nodes [deleteNodeIdx];
				if (deleteNode != null) {
					for(int i=0; i<deleteNode.outputs.Count; i++){
					/*	Debug.Log ("deleting " + deleteNode.nodeName 
								+ " from input " + deleteNode.outputs [i].connectedNodeInputID 
								+ " of " + deleteNode.outputs [i].connectedNode.nodeName 	//!!!!!!!!!
								+ ", which has " + deleteNode.outputs [i].connectedNode.inputs.Count 
								+ " inputs in total."); */
						//Debug.Log ("connected node has "+deleteNode.outputs[i].connectedNode+" outputs!");
						if (deleteNode.outputs [i].connectedNode != null) {
							deleteNode.outputs [i].connectedNode.inputs [deleteNode.outputs [i].connectedNodeInputID].isOccupied = false;
							deleteNode.outputs [i].connectedNode.inputs [deleteNode.outputs [i].connectedNodeInputID].connectedNode = null;
							deleteNode.outputs [i].connectedNode.inputs [deleteNode.outputs [i].connectedNodeInputID].connectedNodeOutputID = new int (); 
						}
					}
					currentGraph.nodes.RemoveAt (deleteNodeIdx);
					GameObject.DestroyImmediate (deleteNode, true);
					AssetDatabase.SaveAssets ();
					AssetDatabase.Refresh ();
				}
			}
		}
	}

	public static void setAllInfoNotCurrent(){
		BAMF_GameStatesAndParameters[] allInfo = ScriptableObject.FindObjectsOfType<BAMF_GameStatesAndParameters> ();
		for (int i = 0; i < allInfo.Length; i++) {
			allInfo [i].isCurrentGameInfo = false;
		}
	}

	public static void DrawGrid(Rect viewRect, float gridSpacing, float gridOpacity, Color gridColor, Vector2 gridOffset, int nodeCount){
		EditorGUI.DrawRect (viewRect, new Color (0, 0, 0, 0.2f));
		int widthDivs = Mathf.CeilToInt ((viewRect.width - gridOffset.x) / gridSpacing);
		int heightDivs = Mathf.CeilToInt ((viewRect.height - gridOffset.y) / gridSpacing);
		if (nodeCount == 0) {
			nodeCount = 1;
		}
		Handles.BeginGUI ();
		Handles.color = new Color (gridColor.r, gridColor.g, gridColor.b, gridOpacity);
		for (int x = 0; x < widthDivs; x++) {
			Handles.DrawLine (new Vector3 (gridOffset.x/nodeCount + gridSpacing * x, 0, 0), new Vector3 (gridOffset.x/nodeCount + gridSpacing * x, viewRect.height));
		}
		for (int y = 0; y < heightDivs; y++) {
			Handles.DrawLine (new Vector3 (0, gridOffset.y/nodeCount + gridSpacing * y, 0), new Vector3 (viewRect.width, gridOffset.y/nodeCount + gridSpacing * y));
		}
		Handles.color = Color.white;
		Handles.EndGUI ();
	}

}
