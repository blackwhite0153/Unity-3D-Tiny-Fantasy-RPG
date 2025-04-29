using UnityEngine;

public class CameraController : MonoBehaviour
{
    private PlayerController _player;
    private Transform _target;

    private Vector3 _position;

    private bool _isInitialized = false;

    private float _orbitSpeed;  // ȸ�� �ӵ�

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

        // ȸ�� ���� ������Ʈ
        _currentAngle += rotationInput * _orbitSpeed * Time.deltaTime;

        // ȸ�� ��ȯ ����
        Quaternion rotation = Quaternion.Euler(0, _currentAngle, 0);    // Y�� �������� ���� ���
        Vector3 rotatedPosition = rotation * _position;  // ���� ��ġ ���͸� ȸ��

        // ī�޶� ��ġ ������Ʈ
        transform.position = _target.position + rotatedPosition;
    }

    private void CameraLookAt()
    {
        transform.LookAt(_target);
    }
}