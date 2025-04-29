using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameClearManager : MonoBehaviour
{
    private GameObject _ui;

    private Camera _camera;

    public GameObject GameStartObject;
    public GameObject GamePlayingObject;
    public GameObject GameOverObject;

    public Button RestartButton;
    public Button MainButton;

    private void Start()
    {
        _ui = GetComponentInChildren<Canvas>().gameObject;
        _ui.SetActive(false);

        _camera = GetComponentInChildren<Camera>();
        _camera.transform.position = new Vector3(65.0f, 0.0f, -65.0f);

        RestartButton.onClick.AddListener(OnRestartButtonClick);
        MainButton.onClick.AddListener(OnMainButtonClick);
    }

    private void OnEnable()
    {
        GameStartObject.SetActive(false);
        GamePlayingObject.SetActive(false);
        GameOverObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        EndingCameraMove();
    }

    private void OnRestartButtonClick()
    {
        GameManager.Instance.IsRestart = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnMainButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void EndingCameraMove()
    {
        if (_camera.transform.position.y < 20.0f)
        {
            _camera.transform.position += Vector3.up * 1.5f * Time.deltaTime;
        }
        else
        {
            _ui.SetActive(true);
        }
    }
}