using UnityEngine;
using System.Collections;

public enum NodeType{
	Float, Add, Music, ParameterModifier
}

public enum NodeConnectionType{
	Float, Add, MusicClip
}

public enum ParameterModifierType{
	Volume, LowPass, HighPass
}
