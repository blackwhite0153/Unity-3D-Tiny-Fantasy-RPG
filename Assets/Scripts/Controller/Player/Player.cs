
public class PlayerState
{
    private int _defaultLevel;
    private int _defaultHp;
    private int _defaultExp;
    private int _defaultAtk;
    private int _defaultDef;
    private float _defaultSpeed;

    public int PlayerLevel { get; set; }
    public int PlayerMaxHp { get; set; }
    public int PlayerCurrentHp { get; set; }
    public int PlayerMaxExp { get; set; }
    public int PlayerCurrentExp { get; set; }
    public int PlayerAtk { get; set; }
    public int PlayerDef { get; set; }
    public float PlayerSpeed { get; set; }

    // �⺻���� �����ϴ� ������ �߰�
    public PlayerState()
    {
        // �ʱⰪ ����
        _defaultLevel = 1;
        _defaultExp = 100;
        _defaultHp = 150;
        _defaultAtk = 20;
        _defaultDef = 10;
        _defaultSpeed = 2.5f;

        // �ɷ�ġ �ʱ�ȭ
        ResetStats();
    }

    // �ɷ�ġ�� �⺻������ �ʱ�ȭ
    public void ResetStats()
    {
        PlayerLevel = _defaultLevel;
        PlayerMaxHp = _defaultHp;
        PlayerCurrentHp = PlayerMaxHp;
        PlayerMaxExp = _defaultExp;
        PlayerCurrentExp = 0;
        PlayerAtk = _defaultAtk;
        PlayerDef = _defaultDef;
        PlayerSpeed = _defaultSpeed;
    }
}

public class Player
{
    public PlayerState PlayerState { get; private set; }

    public Player()
    {
        PlayerState = new PlayerState();
    }

    // �ɷ�ġ�� ���� ������ �ʱ�ȭ
    public void ResetStats()
    {
        PlayerState.ResetStats();
    }
}