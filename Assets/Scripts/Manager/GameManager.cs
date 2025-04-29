
public class GameManager : Singleton<GameManager>
{
    private bool _isGameClear;
    private bool _isRestart;

    public bool IsGameClear
    {
        get { return _isGameClear; }
        set { _isGameClear = value; }
    }

    public bool IsRestart
    {
        get { return _isRestart; }
        set { _isRestart = value; }
    }

    protected override void Initialize()
    {
        base.Initialize();

        _isGameClear = false;
        _isRestart = false;
    }
}