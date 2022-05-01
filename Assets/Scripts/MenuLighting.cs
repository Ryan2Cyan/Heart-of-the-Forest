using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLighting : MonoBehaviour
{
	//References
	[SerializeField] private Light directionalLight;
	[SerializeField] private LightPreset preset;
	[SerializeField] private Light moonLight;
	//Variables
	[SerializeField] private float dayLength;
	[SerializeField] private float timeOfDay;
	[SerializeField, Range(0f, 1f)] private float cycleStart;
	private float t = 0.0f;
	private bool isDay = true;

	private void Start()
	{
		//Sets time of day to start at dawn
		timeOfDay = dayLength * cycleStart;
		StartCoroutine(TurnToNight());
	}

	IEnumerator TurnToNight()
	{
		yield return new WaitForSeconds(dayLength/2);
		ToNight();
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
				UpdateLighting(timeOfDay / dayLength);
			}
			else
			{
				UpdateLighting(timeOfDay / dayLength);
			}
		}
		else
		{
			if (Application.isPlaying)
			{
				//Keep the directional light moving during night until it's dark
				if (!(timeOfDay % dayLength > dayLength * 0.95))
				{
					timeOfDay += Time.deltaTime;
					timeOfDay %= dayLength; //Clamp between 0-DayLength
					UpdateLighting(timeOfDay / dayLength);
				}

			}
			else
			{
				UpdateLighting(timeOfDay / dayLength);
			}
			// Transition moon intensity
			t += 0.2f * Time.deltaTime;
			moonLight.intensity = Mathf.Lerp(0, 0.27f, t);
		}


	}

	private void UpdateLighting(float timePercent)
	{
		RenderSettings.ambientLight = preset.ambientColour.Evaluate(timePercent);
		RenderSettings.fogColor = preset.fogColour.Evaluate(timePercent);

		if (directionalLight != null)
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
