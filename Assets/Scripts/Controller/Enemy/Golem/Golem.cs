
public class GolemState
{
    private int _defaultHp;
    private int _defaultAtk;
    private int _defaultDef;
    private float _defaultSpeed;

    public int GolemHp { get; set; }
    public int GolemAtk { get; set; }
    public int GolemDef { get; set; }
    public float GolemSpeed { get; set; }

    // �⺻���� �����ϴ� ������ �߰�
    public GolemState()
    {
        // �ʱⰪ ����
        _defaultHp = 50;
        _defaultAtk = 10;
        _defaultDef = 5;
        _defaultSpeed = 2.5f;

        // �ɷ�ġ �ʱ�ȭ
        ResetStats();
    }

    // �ɷ�ġ�� �⺻������ �ʱ�ȭ
    public void ResetStats()
    {
        GolemHp = _defaultHp;
        GolemAtk = _defaultAtk;
        GolemDef = _defaultDef;
        GolemSpeed = _defaultSpeed;
    }
}

public class Golem
{
    public GolemState GolemState { get; private set; }

    public Golem()
    {
        GolemState = new GolemState();
    }

    // �ɷ�ġ�� ���� ������ �ʱ�ȭ
    public void ResetStats()
    {
        GolemState.ResetStats();
    }
}