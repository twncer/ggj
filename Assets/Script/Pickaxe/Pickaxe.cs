using UnityEngine;

public class Pickaxe : MonoBehaviour
{
    private const float _maxCharge = 100f;
    private const int _maxChargeDuration = 2;
    private float _chargeSpeed;
    private float _currentCharge;
    private bool _charging;        
    private Rigidbody _rb;

    public void Charge()
    {
        if (_charging)
            return ;
        _charging = true;
        _currentCharge = 0f;
        _chargeSpeed = Time.deltaTime * _maxChargeDuration;
        _chargeSpeed = _maxCharge / _maxChargeDuration;
    }

    void Start()
    {
        _currentCharge = 0f;
        _charging = false;

        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;

        _charging = false;
    }

    void FixedUpdate()
    {       
        if (_charging && _currentCharge < _maxCharge)
        {
            _currentCharge += _chargeSpeed * Time.deltaTime;
        }
    }

    public void ThrowPickaxe()
    {
        Debug.Log($"Throwing with charge: {_currentCharge}");
        _charging = false;
    }
}
