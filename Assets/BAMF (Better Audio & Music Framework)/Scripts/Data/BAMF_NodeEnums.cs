using UnityEngine;
using System.Collections;

public enum NodeType{
	Float, Add, Music, ParameterModifier, StateManager
}

public enum NodeConnectionType{
	Float, Add, MusicClip, MusicPiece
}

public enum ParameterModifierType{
	Volume, LowPass, HighPass
}
