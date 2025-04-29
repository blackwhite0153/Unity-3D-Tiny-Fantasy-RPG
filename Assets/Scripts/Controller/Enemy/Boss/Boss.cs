
public class BossState
{
    private int _defaultHp;
    private int _defaultAtk;
    private int _defaultDef;
    private float _defaultSpeed;

    public int BossHp { get; set; }
    public int BossAtk { get; set; }
    public int BossDef { get; set; }
    public float BossSpeed { get; set; }

    // �⺻���� �����ϴ� ������ �߰�
    public BossState()
    {
        // �ʱⰪ ����
        _defaultHp = 500;
        _defaultAtk = 50;
        _defaultDef = 30;
        _defaultSpeed = 3.0f;

        // �ɷ�ġ �ʱ�ȭ
        ResetStats();
    }

    // �ɷ�ġ�� �⺻������ �ʱ�ȭ
    public void ResetStats()
    {
        BossHp = _defaultHp;
        BossAtk = _defaultAtk;
        BossDef = _defaultDef;
        BossSpeed = _defaultSpeed;
    }
}


public class Boss
{
    public BossState BossState { get; private set; }

    public Boss()
    {
        BossState = new BossState();
    }

    // �ɷ�ġ�� ���� ������ �ʱ�ȭ
    public void ResetStats()
    {
        BossState.ResetStats();
    }
}