using UnityEngine;

public class TestCamera : MonoBehaviour
{
    private PlayerController _player;
    private Transform _target;

    private Vector3 _position;

    private float _orbitSpeed;  // ȸ�� �ӵ�

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

        // ȸ�� ���� ������Ʈ
        _currentAngle += rotationInput * _orbitSpeed * Time.deltaTime;

        // ȸ�� ��ȯ ����
        Quaternion rotation = Quaternion.Euler(0, _currentAngle, 0);    // Y�� �������� ���� ���
        Vector3 rotatedPosition = rotation * _position;  // ���� ��ġ ���͸� ȸ��

        // ī�޶� ��ġ ������Ʈ
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