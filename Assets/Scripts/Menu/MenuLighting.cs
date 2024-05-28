using System.Collections;
using UnityEngine;

namespace Menu
{
	public class MenuLighting : MonoBehaviour
	{
		//References
		[SerializeField] private Light directionalLight;
		[SerializeField] private LightPreset preset;
		[SerializeField] private Light moonLight;
		[SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private GameObject menuEnemies;
		//Variables
		private float dayLength;
		[SerializeField] private float timeOfDay;
		[SerializeField, Range(0f, 1f)] private float cycleStart;
		private float t;
		private bool isDay = true;
		private bool canFade;
		private bool fadeDirection = true;
	

		private void Start()
		{
			//Sets time of day to start at dawn
			dayLength = 30f;
			timeOfDay = dayLength * cycleStart;
			StartCoroutine(TurnToNight());
			StartCoroutine(SpawnMenuEnemies());
			RenderSettings.skybox.SetFloat("_AtmosphereThickness", 1f);
		}
	
		private void Update()
		{
			if (preset == null)
				return;

			if(canFade)
			{
				if(canvasGroup.alpha <= 1 && fadeDirection)
				{
					canvasGroup.alpha += Time.deltaTime * 0.5f;
				}
				if(canvasGroup.alpha == 1)
				{
					fadeDirection = false;
					menuEnemies.SetActive(true);
				}
				if (!fadeDirection && canvasGroup.alpha >= 0)
				{
					canvasGroup.alpha -= Time.deltaTime * 0.5f;
				}
				else if (!fadeDirection && canvasGroup.alpha == 0)
					canFade = false;
				
			}

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
				// Transition moon intensity and atmosphere thickness for night skybox
				t += 0.2f * Time.deltaTime;
				moonLight.intensity = Mathf.Lerp(0, 0.27f, t);
				RenderSettings.skybox.SetFloat("_AtmosphereThickness", Mathf.Lerp(1, 0.28f, t));
			}


		}

		private IEnumerator SpawnMenuEnemies()
		{
			yield return new WaitForSeconds((dayLength / 2) + 2);
			canFade = true;
		}


		private IEnumerator TurnToNight()
		{
			yield return new WaitForSeconds(dayLength/2);
			ToNight();
		}
	
		private void UpdateLighting(float timePercent, bool affectLightRotation)
		{
			RenderSettings.ambientLight = preset.ambientColour.Evaluate(timePercent);
			RenderSettings.fogColor = preset.fogColour.Evaluate(timePercent);

			if (directionalLight != null)
			{
				directionalLight.color = preset.directionalColour.Evaluate(timePercent);
				if(affectLightRotation)
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
			ChangeLevelLights(false);
		}

		public void ToNight()
		{
			isDay = false;
			timeOfDay = dayLength * 0.75f;
			directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timeOfDay / dayLength * 360f) - 90f, 170f, 0));
			moonLight.intensity = 0;
			ChangeLevelLights(true);
		}

		//Turn torches on or off
		private static void ChangeLevelLights(bool turnOn)
		{
			int i;
			Light light;
			ParticleSystem vfx;
			GameObject[] levelLights = GameObject.FindGameObjectsWithTag("FireLight");

			if (turnOn)
			{
				for (i = 0; i < levelLights.Length; i++)
				{
					light = levelLights[i].GetComponentInChildren(typeof(Light)) as Light;
					light.enabled = true;
					vfx = levelLights[i].GetComponentInChildren(typeof(ParticleSystem)) as ParticleSystem;
					vfx.Play();
				}
			}
			else
			{
				for (i = 0; i < levelLights.Length; i++)
				{
					light = levelLights[i].GetComponentInChildren(typeof(Light)) as Light;
					light.enabled = false;
					vfx = levelLights[i].GetComponentInChildren(typeof(ParticleSystem)) as ParticleSystem;
					vfx.Stop();
				}
			}
		}
	}
}
