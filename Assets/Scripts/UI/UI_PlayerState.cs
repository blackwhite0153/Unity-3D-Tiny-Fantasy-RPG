using TMPro;
using UnityEngine.UI;

public class UI_PlayerState : UI_Base
{
    private PlayerController _player;

    private TMP_Text _levelText;
    private Slider _hpBar;
    private Slider _expBar;

    private bool _isInitialized = false;

    private int _level;
    private int _maxHp;
    private int _currentHp;
    private int _maxExp;
    private int _currentExp;

    protected override void Initialize()
    {
        base.Initialize();
    }

    private void Update()
    {
        WaitForPlayer();

        if (_player != null)
        {
            LevelTextUpdate();
            HpBarUpdate();
            ExpBarUpdate();
        }
    }

    private void Setting()
    {
        _levelText = GetComponentInChildren<TMP_Text>();

        Slider[] sliders = GetComponentsInChildren<Slider>();

        foreach (Slider slider in sliders)
        {
            if (slider.gameObject.name == "HpBarSlider")
            {
                _hpBar = slider;
            }
            if (slider.gameObject.name == "ExpBarSlider")
            {
                _expBar = slider;
            }
        }

        if (_player != null)
        {
            _level = _player.PlayerLevel;
            _maxHp = _player.PlayerMaxHp;
            _maxExp = _player.PlayerMaxExp;
        }

        _hpBar.maxValue = _maxHp;
        _hpBar.minValue = 0;

        _expBar.maxValue = _maxExp;
        _hpBar.minValue = 0;
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
    private void LevelTextUpdate()
    {
        _level = _player.PlayerLevel;
        _levelText.text = $"Lv. {_level}";
    }

    private void HpBarUpdate()
    {
        _currentHp = _player.PlayerCurrentHp;
        _hpBar.value = _currentHp;
    }

    private void ExpBarUpdate()
    {
        _currentExp = _player.PlayerCurrentExp;
        _expBar.value = _currentExp;
    }
}