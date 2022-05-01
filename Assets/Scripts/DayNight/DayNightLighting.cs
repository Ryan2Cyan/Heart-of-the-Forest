using UnityEngine;

public class DayNightLighting : MonoBehaviour
{
	//References
	[SerializeField] private Light DirectionalLight;
	[SerializeField] private LightPreset Preset;
	[SerializeField] private GameObject GameStateObject;
	[SerializeField] private Light MoonLight;
	//Variables
	[SerializeField] private float DayLength;
	[SerializeField] private float TimeOfDay;
	private float t = 0.0f;
	private bool isDay = true;

	private void Start()
	{
		GameState GState = GameStateObject.GetComponent<GameState>();
		DayLength = GState.maximumDayTime * 2;
		//Sets time of day to start at dusk
		TimeOfDay = DayLength * 0.25f;
	}

	private void Update()
	{
		if (Preset == null)
			return;

		if (isDay)
		{
			if (Application.isPlaying)
			{
				TimeOfDay += Time.deltaTime;
				TimeOfDay %= DayLength; //Clamp between 0-DayLength
				UpdateLighting(TimeOfDay / DayLength);
			}
			else
			{
				UpdateLighting(TimeOfDay / DayLength);
			}
		}
		else
		{
			if (Application.isPlaying)
			{
				//Keep the directional light moving during night until it's dark
				if(!(TimeOfDay % DayLength > DayLength * 0.95))
				{
					TimeOfDay += Time.deltaTime;
					TimeOfDay %= DayLength; //Clamp between 0-DayLength
					UpdateLighting(TimeOfDay / DayLength);
				}
				
			}
			else
			{
				UpdateLighting(TimeOfDay / DayLength);
			}

		}

		t += 0.2f * Time.deltaTime;
		MoonLight.intensity = Mathf.Lerp(0, 0.2f, t);

	}

	private void UpdateLighting(float timePercent)
	{
		RenderSettings.ambientLight = Preset.Ambientcolour.Evaluate(timePercent);
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

	public void toDay()
	{
		isDay = true;
		TimeOfDay = DayLength * 0.25f;
		MoonLight.intensity = 0;
	}

	public void toNight()
	{
		isDay = false;
		TimeOfDay = DayLength * 0.75f;
		MoonLight.intensity = 0;
	}
}
