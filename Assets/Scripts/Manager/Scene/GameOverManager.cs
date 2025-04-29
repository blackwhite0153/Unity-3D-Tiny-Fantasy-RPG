using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject GameStartObject;
    public GameObject GamePlayingObject;
    public GameObject GameClearObject;

    public Button RestartButton;
    public Button MainButton;

    private void Start()
    {
        RestartButton.onClick.AddListener(OnRestartButtonClick);
        MainButton.onClick.AddListener(OnMainButtonClick);
    }

    private void OnEnable()
    {
        GameStartObject.SetActive(false);
        GameClearObject.SetActive(false);
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
}