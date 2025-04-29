using UnityEngine;

public class MinimapCameraController : MonoBehaviour
{
    private PlayerController _player;
    private Transform _target;

    private Camera _camera;

    private Vector3 _position;
    private Vector3 _rotation;

    private bool _isInitialized = false;

    private void Update()
    {
        WaitForPlayer();
        CameraFollow();
    }

    private void Setting()
    {
        _target = _player.gameObject.transform;

        _camera = GetComponent<Camera>();

        _position = new Vector3(0.0f, 10.0f, 0.0f);
        _rotation = new Vector3(90.0f, 0.0f, 0.0f);

        transform.rotation = Quaternion.Euler(_rotation);

        _camera.orthographic = true;
        _camera.orthographicSize = 10.0f;
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
        if (_target != null)
        {
            transform.position = _target.position + _position;
        }
    }
}