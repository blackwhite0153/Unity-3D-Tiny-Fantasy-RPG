using UnityEngine;

public class GamePlayingManager : MonoBehaviour
{
    private PlayerController _player;

    public GameObject GameStartObject;
    public GameObject GameClearObject;
    public GameObject GameOverObject;

    private void OnEnable()
    {
        if (_player != null) _player = null;

        GameStartObject.SetActive(false);
        GameClearObject.SetActive(false);
        GameOverObject.SetActive(false);
    }

    private void Update()
    {
        if (_player == null)
        {
            _player = FindAnyObjectByType<PlayerController>();
        }
        else
        {
            PlayerHpUpdate();
            GameClearCheck();
        }
    }

    private void GameClearCheck()
    {
        if (GameManager.Instance.IsGameClear)
        {
            GameManager.Instance.IsGameClear = false;

            GameStartObject.SetActive(false);
            this.gameObject.SetActive(false);
            GameClearObject.SetActive(true);
            GameOverObject.SetActive(false);
        }
    }

    private void PlayerHpUpdate()
    {
        int hp = _player.PlayerCurrentHp;

        if (hp <= 0 && !GameOverObject.activeSelf)
        {
            GameObject ui = GetComponentInChildren<Canvas>().gameObject;

            GameOverObject.SetActive(true);
            ui.SetActive(false);
        }
    }
}