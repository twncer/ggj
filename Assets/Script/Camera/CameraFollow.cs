using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    private const float _smoothSpeed = 5f;
    private Vector3 _offset;

    void Start()
    {
        _offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        Vector3 targetPos = target.position + _offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, _smoothSpeed * Time.deltaTime);
    }
}
