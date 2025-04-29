using UnityEngine;

public class TestCamera : MonoBehaviour
{
    private PlayerController _player;
    private Transform _target;

    private Vector3 _position;

    private float _orbitSpeed;  // 회전 속도

    private float _currentAngle;

    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _zoomMax;
    [SerializeField] private float _zoomMin;

    private void Start()
    {
        Setting();
    }

    private void Update()
    {
        CameraFollow();
        CameraRotation();
        //CameraZoom();
        CameraLookAt();
    }

    private void Setting()
    {
        _orbitSpeed = 100.0f;

        _currentAngle = 0.0f;

        _position = new Vector3(6.0f, 6.0f, -6.0f);
    }

    private void CameraFollow()
    {
        if (_player == null)
        {
            _player = FindAnyObjectByType<PlayerController>();
            _target = _player.gameObject.transform;
        }

        transform.position = _target.position + _position;
    }

    private void CameraRotation()
    {
        float rotationInput = 0.0f;

        if (Input.GetKey(KeyCode.Q))
        {
            rotationInput = -1.0f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotationInput = 1.0f;
        }

        // 회전 각도 업데이트
        _currentAngle += rotationInput * _orbitSpeed * Time.deltaTime;

        // 회전 변환 적용
        Quaternion rotation = Quaternion.Euler(0, _currentAngle, 0);    // Y축 기준으로 수평 계산
        Vector3 rotatedPosition = rotation * _position;  // 기존 위치 벡터를 회전

        // 카메라 위치 업데이트
        transform.position = _target.position + rotatedPosition;
    }

    private void CameraZoom()
    {
        float zoomDirection = Input.GetAxis("Mouse ScrollWheel");

        if (transform.position.z >= _zoomMax && zoomDirection > 0) return;
        else if (transform.position.z <= _zoomMin && zoomDirection < 0) return;
        transform.position += transform.forward * zoomDirection * _zoomSpeed;
    }

    private void CameraLookAt()
    {
        transform.LookAt(_target);
    }
}