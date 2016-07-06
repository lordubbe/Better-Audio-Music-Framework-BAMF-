using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BAMF_NodeWorkView : BAMF_ViewBase {
	#region public variables
	#endregion

	#region protected variables
	private Vector2 mousePos;
	int deleteNodeIdx = 0;
	bool draggingworkspace = false;
	Vector2 gridOffset = new Vector2(0f,0f);
	bool multiSelect = false;
	Vector2 multiSelectStartPos;
	Vector2 multiSelectEndPos;

	//music previews
	BAMF_MusicNode.BAMF_MusicClip clipToEdit;
	#endregion

	#region constructor
	public BAMF_NodeWorkView () : base ("Work View"){}
	#endregion

	#region main methods
	public override void UpdateView (Rect editorRect, Rect percentageRect, Event e, BAMF_NodeGraph currentGraph)
	{
		base.UpdateView (editorRect, percentageRect, e, currentGraph);
		if (currentGraph != null) {
			viewTitle = currentGraph.graphName;
		} else {
			viewTitle = "No Graph";
		}
		GUI.Box (viewRect, viewTitle, viewSkin.GetStyle("ViewBG"));
		if (currentGraph != null) {
			BAMF_NodeUtilities.DrawGrid (viewRect, 25, .1f, Color.white, gridOffset, currentGraph.nodes.Count);
			BAMF_NodeUtilities.DrawGrid (viewRect, 50f, .2f, Color.white, gridOffset, currentGraph.nodes.Count);
		} else {
			//TODO: draw drag & drop screen here
		}
		GUILayout.BeginArea (viewRect); //actual workspace view

		if (currentGraph != null) {
			currentGraph.UpdateGraphGUI (e, viewRect, viewSkin );
		}

		if (multiSelect) {
			multiSelectEndPos = Event.current.mousePosition;
			Rect selection = new Rect (multiSelectStartPos.x, multiSelectStartPos.y, multiSelectEndPos.x - multiSelectStartPos.x, multiSelectEndPos.y - multiSelectStartPos.y);
			EditorGUI.DrawRect (selection, new Color (1, 1, 1, 0.2f));
			for (int i = 0; i < currentGraph.nodes.Count; i++) {
				if (!currentGraph.nodes [i].isSelected) {
					for (float x = multiSelectStartPos.x; x < multiSelectEndPos.x; x += 10) {
						for (float y = multiSelectStartPos.y; y < multiSelectEndPos.y; y += 10) {
							if (currentGraph.nodes [i].nodeRect.Contains (new Vector2 (x, y))) {
								currentGraph.nodes [i].isSelected = true;
								currentGraph.nodes [i].canMove = false;
							}
						}
					}
				} else {
					bool stillSelected = false;
					for (float x = multiSelectStartPos.x; x < multiSelectEndPos.x; x += 10) {
						for (float y = multiSelectStartPos.y; y < multiSelectEndPos.y; y += 10) {
							if (currentGraph.nodes [i].nodeRect.Contains (new Vector2 (x, y))) {
								stillSelected = true;
							}
						}
					}
					if (!stillSelected) {
						currentGraph.nodes [i].isSelected = false;
						currentGraph.nodes [i].canMove = true;
					}
				}
			}
		} else {
			if (currentGraph != null) {
				currentGraph.amountSelected = 0;
				for (int i = 0; i < currentGraph.nodes.Count; i++) {
					if (currentGraph.nodes [i].isSelected) {
						currentGraph.amountSelected++;
					}
				}
				if (currentGraph.amountSelected == 0) {
					currentGraph.multiSelected = false;
				}
			}
		}


		GUILayout.EndArea ();

		ProcessEvents (e);
	}

	public override void ProcessEvents (Event e)
	{
		base.ProcessEvents (e);
		if (viewRect.Contains (e.mousePosition)) { //If mouse is inside this view
			if (e.button == 2) {//middle click
				if (e.type == EventType.MouseDown) {
					mousePos = e.mousePosition;
					bool overNode = false;
					if (currentGraph != null) {
						if (currentGraph.nodes.Count > 0) {
							for (int i = 0; i < currentGraph.nodes.Count; i++) {
								if (currentGraph.nodes [i].nodeRect.Contains (mousePos)) {
									overNode = true;
								}
							}
						}
					}
					if (!overNode) {
						draggingworkspace = true;
					}
				}
				if (e.type == EventType.MouseDrag) {
					if (draggingworkspace) {
						for (int i = 0; i < currentGraph.nodes.Count; i++) {
							
							gridOffset.x += e.delta.x;
							gridOffset.y += e.delta.y;
							if (gridOffset.x > 0) {
								gridOffset.x = 0;
							} else {
								currentGraph.nodes [i].nodeRect.x += e.delta.x;
							}
							if (gridOffset.y > 0) {
								gridOffset.y = 0;
							} else {
								currentGraph.nodes [i].nodeRect.y += e.delta.y;
							}

							//also move inputs of music nodes 
							if (currentGraph.nodes [i].nodeType == NodeType.Music) {
								if (currentGraph.nodes [i].inputs.Count > 0) {
									Debug.Log (currentGraph.nodes [i].inputs.Count);
									for (int j = 0; j < currentGraph.nodes [i].inputs.Count; j++) {
										if ((gridOffset.x < 0)) {
											currentGraph.nodes [i].inputs [j].inputRect.x += e.delta.x;
										}
										if ((gridOffset.y < 0)) {
											currentGraph.nodes [i].inputs [j].inputRect.y += e.delta.y;
										}
									}
								}
							}
						}
					}
				}
				if (e.type == EventType.MouseUp) {
					draggingworkspace = false;
				}
			}

			if (e.button == 0) { //left click (MULTISELECT)
				if (e.type == EventType.MouseDown) {
					mousePos = e.mousePosition;
					bool overNode = false;
					int overIdx = 0;
					if (currentGraph != null) {
						if (currentGraph.nodes.Count > 0) {
							for (int i = 0; i < currentGraph.nodes.Count; i++) {
								if (currentGraph.nodes [i].nodeRect.Contains (mousePos)) {
									overNode = true;
									overIdx = i;
								}
							}
						}
					}
					if (!overNode) {
						multiSelectStartPos = mousePos;
						multiSelectEndPos = mousePos;
						multiSelect = true;
						currentGraph.amountSelected = 0;
					} else {
						if (!currentGraph.nodes [overIdx].isSelected) {
							if (e.shift) {
								currentGraph.multiSelected = true;
							} else {
								currentGraph.multiSelected = false;
							}
						}
					}
				}

				if (e.type == EventType.MouseDrag) {
					
				}

				if (e.type == EventType.MouseUp) {
					for (int i = 0; i < currentGraph.nodes.Count; i++) {
						if (currentGraph.nodes [i].isSelected) {
							if (multiSelect) {
								currentGraph.multiSelected = true; 
							}
						} 
						currentGraph.nodes [i].canMove = true;
					}
					multiSelect = false;
				}
			}

			if (e.button == 1) {//right click
				if (e.type == EventType.mouseDown) {
					//context menu
					mousePos = e.mousePosition;
					bool overNode = false;
					bool overMusic = false;
					deleteNodeIdx = 0;
					if (currentGraph != null) {
						if (currentGraph.nodes.Count > 0) {
							for (int i = 0; i < currentGraph.nodes.Count; i++) {
								if (currentGraph.nodes [i].nodeRect.Contains (mousePos)) {
									deleteNodeIdx = i;
									overNode = true;
								}
							}
						}
					}
					if (currentGraph != null) {
						if (currentGraph.nodes != null) {
							for (int i = 0; i < currentGraph.nodes.Count; i++) {
								if (currentGraph.nodes [i].nodeRect.Contains (e.mousePosition)) {
									if (currentGraph.nodes [i].nodeType == NodeType.Music) {
										if (currentGraph.nodes [i].GetLayers ().Count > 0) {
											List<BAMF_MusicNode.BAMF_MusicClip> layers = currentGraph.nodes [i].GetLayers ();
											for (int j = 0; j < layers.Count; j++) {
												e = Event.current;
												if (layers [j].prevRect.Contains (e.mousePosition)) {
													overNode = false;
													overMusic = true;
													clipToEdit = layers [j];
												}
											}
										}
									}
								}
							}
						}
					}


					if (overNode) {
						ProcessContextMenu (e, 1); // 1 = node context menu
					} else if (overMusic) {
						ProcessContextMenu (e, 2); // music layer context menu
					}else {
						ProcessContextMenu (e, 0); // 0 = normal context menu
					}
				}
			}
		}
	}
	#endregion

	#region utility methods
	void DeselectAllNodes(){
		for (int i = 0; i < currentGraph.nodes.Count; i++) {
			currentGraph.nodes [i].isSelected = false;
		}
	}

	void ProcessContextMenu(Event e, int contextID){
		GenericMenu menu = new GenericMenu ();

		if (contextID == 0) {
			menu.AddItem (new GUIContent ("Create New Graph"), false, ContextCallback, "0");
			menu.AddItem (new GUIContent ("Load Graph"), false, ContextCallback, "1");

			if (currentGraph != null) {
				menu.AddItem (new GUIContent ("Unload Current Graph"), false, ContextCallback, "2");

				menu.AddSeparator ("");
				menu.AddItem (new GUIContent ("Music Piece"), false, ContextCallback, "Music");
				menu.AddItem (new GUIContent ("Parameter Modifier"), false, ContextCallback, "ParameterModifier");

				menu.AddSeparator ("");
				menu.AddItem (new GUIContent ("Float node"), false, ContextCallback, "3");
				menu.AddItem (new GUIContent ("Add Node"), false, ContextCallback, "4");
			}
		} else if (contextID == 1) {
			menu.AddItem (new GUIContent ("Delete Node"), false, ContextCallback, "5");
		} else if (contextID == 2) { // over music layer
			menu.AddItem (new GUIContent ("Edit Cues for Layer "+(clipToEdit.layerNumber+1)), false, ContextCallback, "editCues");
			menu.AddSeparator ("");
			if (clipToEdit.layerNumber > 0) {
				menu.AddItem (new GUIContent ("Move Layer Up"), false, ContextCallback, "moveLayerUp");
				if (clipToEdit.layerNumber == clipToEdit.parentNode.layers.Count - 1) {
					menu.AddDisabledItem (new GUIContent ("Move Layer Down"));
				}
			}
			if (clipToEdit.layerNumber < clipToEdit.parentNode.layers.Count - 1) {
				if (clipToEdit.layerNumber == 0) {
					menu.AddDisabledItem (new GUIContent ("Move Layer Up"));
				}
				menu.AddItem (new GUIContent ("Move Layer Down"), false, ContextCallback, "moveLayerDown");
			}
			menu.AddSeparator ("");
			menu.AddItem (new GUIContent ("Delete Layer "+(clipToEdit.layerNumber+1)), false, ContextCallback, "deleteLayer");
			menu.AddSeparator ("");
			menu.AddItem (new GUIContent ("Delete Node"), false, ContextCallback, "5");
		}
		menu.ShowAsContext ();
		e.Use ();
	}

	void ContextCallback(object obj){
		switch (obj.ToString ()) {
		case "0"://create new graph
			//
			BAMF_NodePopupWindow.InitNodePopup ();
			break;
		case "1"://load graph
			//
			BAMF_NodeUtilities.LoadGraph();
			break;
		case "2"://unload graph
			//
			BAMF_NodeUtilities.UnloadGraph();
			break;
		case "3"://Float node
			//
			BAMF_NodeUtilities.CreateNode(currentGraph, NodeType.Float, mousePos);
			break;
		case "4"://Add node
			//
			BAMF_NodeUtilities.CreateNode(currentGraph, NodeType.Add, mousePos);
			break;
		case "5"://Add node
			//
			BAMF_NodeUtilities.DeleteNode(deleteNodeIdx, currentGraph);
			break;
		case "Music":
			//
			BAMF_NodeUtilities.CreateNode(currentGraph, NodeType.Music, mousePos);
			break;
		case "ParameterModifier":
			//
			BAMF_NodeUtilities.CreateNode(currentGraph, NodeType.ParameterModifier, mousePos);
			break;
		case "editCues":
			// 
			BAMF_MusicEditWindow.InitNodePopup (clipToEdit.layerNumber, clipToEdit);
			break;
		case "deleteLayer":
			// DELETE THE LAYER
			clipToEdit.parentNode.layers.RemoveAt(clipToEdit.layerNumber);
			break;
		case "moveLayerUp":
			//
			BAMF_MusicNode.BAMF_MusicClip tmp = clipToEdit.parentNode.layers [clipToEdit.layerNumber];
			clipToEdit.parentNode.layers [clipToEdit.layerNumber] = clipToEdit.parentNode.layers [clipToEdit.layerNumber - 1];
			clipToEdit.parentNode.layers [clipToEdit.layerNumber-1] = tmp;
			break;
		case "moveLayerDown":
			//
			BAMF_MusicNode.BAMF_MusicClip temp = clipToEdit.parentNode.layers [clipToEdit.layerNumber];
			clipToEdit.parentNode.layers [clipToEdit.layerNumber] = clipToEdit.parentNode.layers [clipToEdit.layerNumber + 1];
			clipToEdit.parentNode.layers [clipToEdit.layerNumber+1] = temp;
			break;
		default:
			//
			break;
		}
	}
	#endregion
}
