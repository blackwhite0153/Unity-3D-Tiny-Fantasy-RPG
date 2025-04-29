using UnityEngine;
using UnityEngine.UI;

public class GameStartManager : MonoBehaviour
{
    public Button GameStartButton;

    public GameObject GamePlayingObject;
    public GameObject GameClearObject;
    public GameObject GameOverObject;

    private void Start()
    {
        GameStartButton.onClick.AddListener(OnGameStartButtonClick);

        if (GameManager.Instance.IsRestart)
        {
            this.gameObject.SetActive(false);
            GamePlayingObject.SetActive(true);
            GameClearObject.SetActive(false);
            GameOverObject.SetActive(false);

            GameManager.Instance.IsRestart = false;
        }
        else
        {
            this.gameObject.SetActive(true);
            GamePlayingObject.SetActive(false);
            GameClearObject.SetActive(false);
            GameOverObject.SetActive(false);
        }
    }

    private void OnGameStartButtonClick()
    {
        GamePlayingObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}