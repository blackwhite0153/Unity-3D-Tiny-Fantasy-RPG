using UnityEngine;

public class CameraController : MonoBehaviour
{
    private PlayerController _player;
    private Transform _target;

    private Vector3 _position;

    private bool _isInitialized = false;

    private float _orbitSpeed;  // 회전 속도

    private float _currentAngle;

    private void Update()
    {
        WaitForPlayer();

        if (_target != null)
        {
            CameraFollow();
            CameraRotation();
            CameraLookAt();
        }
    }

    private void Setting()
    {
        _target = _player.gameObject.transform;

        _orbitSpeed = 100.0f;

        _currentAngle = 0.0f;

        _position = new Vector3(17.0f, 15.0f, -17.0f);
    }

    private void WaitForPlayer()
    {
        if (_isInitialized) return;

        if (_player == null)
        {
            _player = FindAnyObjectByType<PlayerController>();
        }

        if (_player != null)
        {
            Setting();
            _isInitialized = true;
        }
    }

    private void CameraFollow()
    {
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

    private void CameraLookAt()
    {
        transform.LookAt(_target);
    }
}