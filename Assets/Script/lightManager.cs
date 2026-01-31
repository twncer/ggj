using UnityEngine;

public class lightManager : MonoBehaviour
{
	[SerializeField] private Transform _objet;
	private	Light[]	_lights;
	public int		_lightCount = 10;
	public float	_maxIntensity = 5f;
	public float	_minIntensity = 0f;
	public float	_spawnInterval = 1f;
	public float	_duration = 5f;
	

	void Awake()
	{
		_lights = new Light[_objet.childCount];	
	}
	

    void Update()
    {
    }
}
