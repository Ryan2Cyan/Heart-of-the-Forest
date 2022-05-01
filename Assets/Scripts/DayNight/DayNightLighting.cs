using UnityEngine;

[ExecuteAlways]
public class DayNightLighting : MonoBehaviour
{
	//References
	[SerializeField] private Light DirectionalLight;
	[SerializeField] private LightPreset Preset;
	//Variables
	[SerializeField] private float DayLength;
	[SerializeField] private float TimeOfDay;

	private void Start()
	{
		//Sets time of day to start at dusk
		TimeOfDay = DayLength * 0.25f;
	}

	private void Update()
	{
		if (Preset == null)
			return;

		
			if(Application.isPlaying)
			{
				TimeOfDay += Time.deltaTime;
				TimeOfDay %= DayLength; //Clamp between 0-24
				UpdateLighting(TimeOfDay / DayLength);
			}
			else
			{
				UpdateLighting(TimeOfDay / DayLength);
			}
	}

	private void UpdateLighting(float timePercent)
	{
		RenderSettings.fogColor = Preset.FogColour.Evaluate(timePercent);

		if (DirectionalLight != null)
		{
			DirectionalLight.color = Preset.DirectionalColour.Evaluate(timePercent);
			DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
		}
	}

	private void OnValidate()
	{
		if (DirectionalLight != null)
			return;

		if (RenderSettings.sun != null)
		{
			DirectionalLight = RenderSettings.sun;
		}
	}
}
