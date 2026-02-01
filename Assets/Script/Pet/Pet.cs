using UnityEngine;

public class Pet : MonoBehaviour
{
    public Player player;
    public Pickaxe pickaxe;

    public enum State
    {
        IDLE = 0,
        CATCHING,
        HOLDING,
        FETCHING,
        MOUNTED
    }
    private State _currentState;

    // hovering
    private const float _hoverSpeed = 1f;
    private const float _hoverAmplitude = 0.1f;
    private Vector3 _basePosition;

    // defaults (relative to player)
    private Vector3 _defaultOffset;
    private Quaternion _defaultRotation;

    // attr
    private const float _speedX = 15f;
    private const float _speedY = 3.2f;
    // private const float _speedZ = 1f;
    private const float _rotationSpeed = 2048f;

    void Start()
    {
        _currentState = State.IDLE;
        _basePosition = transform.position;

        _defaultOffset = transform.position - player.transform.position;
        _defaultRotation = transform.rotation;
    }

    public void Reset()
    {
        transform.position = player.transform.position + _defaultOffset;
        transform.rotation = _defaultRotation;
        _basePosition = transform.position;
        _currentState = State.IDLE;
    }

    void FixedUpdate()
    {
        if (_currentState is State.IDLE || _currentState is State.HOLDING)
        {
            float hover = Mathf.Sin(Time.time * _hoverSpeed) * _hoverAmplitude;
            transform.position = _basePosition + Vector3.up * hover;
        }
        // if (_currentState is State.HOLDING)
        // {
        //     Quaternion targetRotation = Quaternion.Euler(0, 180, 0);
        //     transform.rotation = Quaternion.RotateTowards(
        //         transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
        // }
        else if (_currentState is State.CATCHING)
        {
            Vector3 target = pickaxe.transform.position;
            target.y += 0f; // temp
            Vector3 current = transform.position;

            float newX = Mathf.MoveTowards(current.x, target.x, _speedX * Time.deltaTime);
            float newY = Mathf.MoveTowards(current.y, target.y, _speedY * Time.deltaTime);

            transform.position = new Vector3(newX, newY, current.z);

            float distance = Vector2.Distance(new Vector2(newX, newY), new Vector2(target.x, target.y));
            if (distance < 0.1f)
            {
                pickaxe.Hang();
                pickaxe.transform.SetParent(transform);
                pickaxe.transform.localPosition = new Vector3(pickaxe.transform.localPosition.x, pickaxe.transform.localPosition.y, 0);
                _basePosition = transform.position;
                _currentState = State.HOLDING;
            }
        }
        else if (_currentState is State.FETCHING)
        {
            Vector3 target = player.transform.position + _defaultOffset;
            Vector3 current = transform.position;

            float newX = Mathf.MoveTowards(current.x, target.x, _speedX * Time.deltaTime);
            float newY = Mathf.MoveTowards(current.y, target.y, _speedY * Time.deltaTime);

            transform.position = new Vector3(newX, newY, current.z);

            float distance = Vector2.Distance(new Vector2(newX, newY), new Vector2(target.x, target.y));
            if (distance < 0.1f)
            {
                pickaxe.Reset();
                Reset();
            }
        }
    }

    // bring the pickaxe back
    public void Fetch()
    {
        if (_currentState is State.CATCHING or State.FETCHING)
            return;
        if (_currentState is State.HOLDING)
        {
            // Fix Z to player's Z
            transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z);
            _currentState = State.FETCHING;
            return;
        }
        // Fix Z to player's Z
        transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z);
        _currentState = State.CATCHING;
        pickaxe.SetPlayerCollision(false);
    }
}
