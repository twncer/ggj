using UnityEngine;

public class lightManager : MonoBehaviour
{
	[SerializeField] private Transform _objet;
	private	Light[]	_lights;
	private float[] _lightTimers;
	private int _currentLightIndex = 0;
	private float _spawnTimer = 0f;
	
	public int		_lightCount = 10;
	public float	_maxIntensity = 5f;
	public float	_minIntensity = 0f;
	public float	_spawnInterval = 1f;
	private float	_duration;
	

	void Awake()
	{
		_duration = _lightCount * _spawnInterval;
		
		_lights = new Light[_lightCount];
		_lightTimers = new float[_lightCount];
		
		for (int i = 0; i < _lightCount; i++)
		{
			GameObject lightObj = new GameObject("Light_" + i);
			lightObj.transform.position = _objet.position;
			
			_lights[i] = lightObj.AddComponent<Light>();
			_lights[i].intensity = 0f;
			_lights[i].enabled = false;
			_lightTimers[i] = -1f;
		}
	}
	

    void Update()
    {
		_spawnTimer += Time.deltaTime;
		if (_spawnTimer >= _spawnInterval)
		{
			SpawnLight();
			_spawnTimer = 0f;
		}
		
		for (int i = 0; i < _lightCount; i++)
		{
			if (_lightTimers[i] >= 0f)
			{
				UpdateLight(i);
			}
		}
    }
	
	void SpawnLight()
	{
		_lights[_currentLightIndex].transform.position = _objet.position;
		_lights[_currentLightIndex].intensity = _maxIntensity;
		_lights[_currentLightIndex].enabled = true;
		_lightTimers[_currentLightIndex] = 0f;
		
		_currentLightIndex = (_currentLightIndex + 1) % _lightCount;
	}
	
	void UpdateLight(int index)
	{
		_lightTimers[index] += Time.deltaTime;
		
		float t = _lightTimers[index] / _duration;
		_lights[index].intensity = Mathf.Lerp(_maxIntensity, _minIntensity, t);
		
		if (_lightTimers[index] >= _duration)
		{
			_lights[index].enabled = false;
			_lightTimers[index] = -1f;
		}
	}
}
