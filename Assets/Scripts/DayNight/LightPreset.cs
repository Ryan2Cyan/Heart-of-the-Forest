using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName ="lighting Preset", menuName ="Scriptables/Light Preset",order =1)]
public class LightPreset : ScriptableObject
{
	public Gradient directionalColour;
	public Gradient fogColour;
	public Gradient ambientColour;
}
