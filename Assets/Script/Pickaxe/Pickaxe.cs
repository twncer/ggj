using UnityEngine;
using UnityEngine.InputSystem;

public class Pickaxe : MonoBehaviour
{
    public Player player;

    public enum State
    {
        IDLE = 0,
        CHARGING,
        FLYING,
        FIXED,
        DOLL,
        RETURNING
    };
    private State _currentState;

    // Charge
    private const float _maxCharge = 100f;
    private const int _maxChargeDuration = 2;
    private float _chargeSpeed;
    private float _currentCharge;

    // Flying state
    private float _flightStartTime;
    private float _expectedFlightTime;

    // Throw force
    private const float _minThrowForce = 3f;
    private const float _maxThrowForce = 20f;

    // Spin
    private float _spinSpeed = 0f;

    // Rigidbody
    private Rigidbody _rb;

    // Raycast mask (ignores Pickaxe layer)
    private int _raycastMask;

    // defaults (local to player)
    private Vector3 _defaultLocalPosition;
    private Vector3 _defaultLocalRotation;

    public State GetCurrentState()
    {
        return _currentState;
    }

    void Start()
    {
        transform.SetParent(player.transform);

        _currentState = State.IDLE;

        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _rb.useGravity = true;

        _chargeSpeed = _maxCharge / _maxChargeDuration;
        _currentCharge = 0f;
        _raycastMask = ~LayerMask.GetMask("Pickaxe");

        _defaultLocalPosition = transform.localPosition;
        _defaultLocalRotation = transform.localEulerAngles;

        SetPlayerCollision(false);
    }

    public void SetPlayerCollision(bool enable)
    {
        foreach (Collider pickaxeCol in GetComponentsInChildren<Collider>())
            foreach (Collider playerCol in player.GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(pickaxeCol, playerCol, !enable);
    }

    public void Reset()
    {
        transform.SetParent(player.transform);
        transform.localPosition = _defaultLocalPosition;
        transform.localEulerAngles = _defaultLocalRotation;
        _rb.linearDamping = 0f;
        _rb.angularDamping = 0f;
        _rb.isKinematic = true;
        _currentState = State.IDLE;
        SetPlayerCollision(false);
    }

    void FixedUpdate()
    {
        if (_currentState is State.CHARGING)
        {
            _currentCharge += _chargeSpeed * Time.fixedDeltaTime;
            _currentCharge = Mathf.Min(_currentCharge, _maxCharge);
        }
        else if (_currentState is State.FLYING)
        {
            _rb.angularVelocity = Vector3.forward * _spinSpeed * Mathf.Deg2Rad;
        }
    }

    public void Charge()
    {
        if (_currentState is State.CHARGING || 
            _currentState is State.FLYING)
            return;

        _currentState = State.CHARGING;
        _currentCharge = 0f;
    }

    bool GetHitPoint(Vector3 startPos, Vector3 startVel,
        out RaycastHit hit, out float flightTime)
    {
        float step = Time.fixedDeltaTime;
        int maxSteps = 1024;

        Vector3 position = startPos;
        Vector3 velocity = startVel;
        flightTime = 0f;

        for (int i = 0; i < maxSteps; i++)
        {
            Vector3 nextPos =
                position + velocity * step +
                0.5f * Physics.gravity * step * step;

            Vector3 move = nextPos - position;

            if (Physics.Raycast(position, move.normalized, out hit, move.magnitude, _raycastMask))
                return true;

            velocity += Physics.gravity * step;
            position = nextPos;
            flightTime += step;
        }

        hit = default;
        return false;
    }

    GameObject impactMarker;
    void ShowImpactMarker(Vector3 point, Vector3 normal)
    {
        if (impactMarker == null)
        {
            impactMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            impactMarker.transform.localScale = Vector3.one * 0.2f;

            Renderer r = impactMarker.GetComponent<Renderer>();
            r.material = new Material(Shader.Find("Standard"));
            r.material.color = Color.red;

            Destroy(impactMarker.GetComponent<Collider>());
        }

        impactMarker.SetActive(true);
        impactMarker.transform.position = point + normal * 0.05f;
    }

    public void ThrowPickaxe()
    {
        if (_currentState is not State.CHARGING)
            return;

        _currentState = State.FLYING;
        transform.eulerAngles = Vector3.zero;
        transform.SetParent(null);

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        Plane plane = new Plane(Camera.main.transform.forward, transform.position);

        if (!plane.Raycast(ray, out float enter))
            return;

        Vector3 target = ray.GetPoint(enter);
        Vector3 direction = (target - transform.position).normalized;

        float throwForce = Mathf.Lerp(_minThrowForce, _maxThrowForce,
            _currentCharge / _maxCharge);

        Vector3 initialVelocity = direction * throwForce;

        if (GetHitPoint(transform.position, initialVelocity,
            out RaycastHit hit, out float flightTime))
        {
            if (flightTime > 0.05f)
                _spinSpeed = 720f / flightTime;
            else
                _spinSpeed = 720f / 0.05f;

            _expectedFlightTime = flightTime;
            _flightStartTime = Time.time;

            ShowImpactMarker(hit.point, hit.normal);
        }

        _rb.isKinematic = false;
        _rb.linearVelocity = initialVelocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_currentState is not State.FLYING)
            return ;

        if (collision.gameObject.CompareTag("Player"))
            return ;

        if (collision.gameObject.CompareTag("Floor"))
        {
            _rb.linearDamping = 10f;
            _rb.angularDamping = 10f;
            _currentState = State.DOLL;
            return;
        }

        _currentState = State.FIXED;
        SetPlayerCollision(true);
        _rb.isKinematic = true;
    }

    public void Hang()
    {
        _currentState = State.RETURNING;
        _rb.isKinematic = true;
    }

}
