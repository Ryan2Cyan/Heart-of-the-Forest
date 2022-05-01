using UnityEngine;

public class DayNightLighting : MonoBehaviour
{
	//References
	[SerializeField] private Light directionalLight;
	[SerializeField] private LightPreset preset;
	[SerializeField] private GameObject gameStateObject;
	[SerializeField] private Light moonLight;
	//Variables
	[SerializeField] private float dayLength;
	[SerializeField] private float timeOfDay;
	private float t = 0.0f;
	private bool isDay = true;

	private void Start()
	{
		GameState GState = gameStateObject.GetComponent<GameState>();
		dayLength = GState.maximumDayTime * 2;
		//Sets time of day to start at dawn
		timeOfDay = dayLength * 0.25f;
		RenderSettings.skybox.SetFloat("_AtmosphereThickness", 1f);
	}

	private void Update()
	{
		if (preset == null)
			return;

		if (isDay)
		{
			if (Application.isPlaying)
			{
				timeOfDay += Time.deltaTime;
				timeOfDay %= dayLength; //Clamp between 0-DayLength
				UpdateLighting(timeOfDay / dayLength, true);
			}
			else
			{
				UpdateLighting(timeOfDay / dayLength, true);
			}
		}
		else
		{
			if (Application.isPlaying)
			{
				//Keep the directional light moving during night until it's dark
				if (!(timeOfDay % dayLength > dayLength * 0.85))
				{
					timeOfDay += Time.deltaTime;
					timeOfDay %= dayLength; //Clamp between 0-DayLength
					UpdateLighting(timeOfDay / dayLength, false);
				}
				
			}
			else
			{
				UpdateLighting(timeOfDay / dayLength, false);
			}
			// Transition moon intensity
			t += 0.2f * Time.deltaTime;
			moonLight.intensity = Mathf.Lerp(0, 0.27f, t);
			RenderSettings.skybox.SetFloat("_AtmosphereThickness", Mathf.Lerp(1, 0.28f, t));
		}

		

	}

	private void UpdateLighting(float timePercent, bool affectLightRotation)
	{
		RenderSettings.ambientLight = preset.ambientColour.Evaluate(timePercent);
		RenderSettings.fogColor = preset.fogColour.Evaluate(timePercent);

		if (directionalLight != null && affectLightRotation)
		{
			directionalLight.color = preset.directionalColour.Evaluate(timePercent);
			directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
		}
	}

	private void OnValidate()
	{
		if (directionalLight != null)
			return;

		if (RenderSettings.sun != null)
		{
			directionalLight = RenderSettings.sun;
		}
	}

	public void ToDay()
	{
		isDay = true;
		RenderSettings.skybox.SetFloat("_AtmosphereThickness", 1f);
		timeOfDay = dayLength * 0.25f;
		moonLight.intensity = 0;
	}

	public void ToNight()
	{
		isDay = false;
		timeOfDay = dayLength * 0.75f;
		moonLight.intensity = 0;
	}
}
