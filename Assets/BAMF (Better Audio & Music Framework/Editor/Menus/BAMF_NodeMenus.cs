using UnityEngine;
using UnityEditor;
using System.Collections;

public static class BAMF_NodeMenus {

	[MenuItem("BAMF/Node Editor")]
	public static void InitNodeEditor(){
		BAMF_NodeEditorWindow.InitEditorWindow ();
	}
}
