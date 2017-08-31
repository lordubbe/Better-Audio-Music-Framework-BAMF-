using UnityEngine;
using System.Collections;

[System.Serializable]
public class BAMF_Parameter : ScriptableObject {
	public float value;
	public string name;

	public BAMF_Parameter(float v, string s){
		value = v; 
		name = s;
	}
}
