using System.Collections.Generic;
using UnityEngine;

// ���� Ŭ���� : ���� �ӽſ��� ����� ���� ���¸� �����ϴ� �߻� Ŭ����
public abstract class State<T>
{
    protected StateMachine<T> stateMachine; // ���� �ӽ� ����
    protected T context;                    // ���� ���¿��� ����� ���ؽ�Ʈ ��ü
    protected int mecanimStateHash;         // �ִϸ����� ���� �ؽð�

    // �⺻ ������
    public State() { }
    // �ִϸ����� ���� �ؽø� �����ϴ� ������
    public State(int mecanimStateHash)
    {
        this.mecanimStateHash = mecanimStateHash;
    }

    // �ִϸ����� ���� �̸��� �ؽ÷� ��ȯ�Ͽ� �����ϴ� ������
    public State(string mecanimStateName) : this(Animator.StringToHash(mecanimStateName)) { }

    // ���� �ӽŰ� ���ؽ�Ʈ�� �����ϴ� �޼���
    public void SetMachine(StateMachine<T> stateMachine, T context)
    {
        this.stateMachine = stateMachine;
        this.context = context;

        OnInitialized();    // ���� �ʱ�ȭ ȣ��
    }

    // ���� �ʱ�ȭ �� ����� �޼��� (�߻� �޼���, �ݵ�� ���� �ʿ�)
    public abstract void OnInitialized();
    // ���� ���� �� ����� ���� �޼��� (�ʿ� �� �������̵� ����)
    public virtual void OnEnter() { }
    // ���� ���� �� ���۵� ���� �޼��� (�����Ӹ��� ����)
    public virtual void OnUpdate(float deltaTime) { }
    // ���� ������Ʈ �� ����� ���� �޼��� (FixedUpdate���� ����)
    public virtual void OnFixedUpdate(float deltaTime) { }
    // ���� ���� �� ����� ���� �޼���
    public virtual void OnExit() { }
}

// ���� �ӽ� Ŭ���� : ���¸� �����ϴ� ���� �ӽ�
public class StateMachine<T>
{
    // ��ϵ� ���� ���
    private Dictionary<System.Type, State<T>> states = new Dictionary<System.Type, State<T>>();

    private State<T> _currentState; // ���� ����
    private State<T> _prevState;    // ���� ����

    private T _context; // ���� �ӽ��� �����ϴ� ���ؽ�Ʈ ��ü

    private float _elapsedTime;     // ���� ���¿��� ����� �ð�

    public State<T> CurrentState => _currentState;  // ���� ���� ��ȯ
    public State<T> PrevState => _prevState;        // ���� ���� ��ȯ

    public event System.Action OnChangedState;  // ���� ���� �̺�Ʈ

    // ���� �ӽ� ������
    public StateMachine(T context, State<T> state)
    {
        this._context = context;
        AddState(state);            // �ʱ� ���� �߰�
        _currentState = state;      // �ʱ� ���� ����
        _currentState.OnEnter();    // �ʱ� ���� ���� ó��
    }

    // ���ο� ���¸� �߰��ϴ� �޼���
    public void AddState(State<T> state)
    {
        state.SetMachine(this, _context);    // ���¿� ���� �ӽŰ� ���ؽ�Ʈ ����
        states[state.GetType()] = state;     // ���� ��Ͽ� �߰�
    }

    // �� ������ ���� ���� (Update���� ȣ��)
    public void OnUpdate(float deltaTime)
    {
        _elapsedTime += deltaTime;          // ���¿��� ����� �ð� ������Ʈ
        _currentState.OnUpdate(deltaTime);  // ���� ������ ������Ʈ ���� ����
    }

    // ���� ���� ������Ʈ (FixedUpdate���� ȣ��)
    public void OnFixedUpdate(float deltaTime)
    {
        _currentState.OnFixedUpdate(deltaTime); // ���� ������ ���� ������Ʈ ����
    }

    // ���� ���� �޼���
    public R ChangeState<R>() where R : State<T>
    {
        var newType = typeof(R);    // ������ ���� Ÿ�� ��������

        // ���� ���¿� ������ ���°� ������ �������� ����
        if (_currentState.GetType() == newType)
        {
            return _currentState as R;
        }

        // ���� ���� ���� ó��
        if (_currentState != null)
        {
            _currentState.OnExit();
        }

        _prevState = _currentState;     // ���� ���� ����
        _currentState = states[newType];    // ���ο� ���� ����
        _currentState.OnEnter();        // ���ο� ���� ���� ó��
        _elapsedTime = 0.0f;            // ���� ��ȯ �� �ð� �ʱ�ȭ

        OnChangedState?.Invoke();   // ���� ���� �̺�Ʈ ����

        return _currentState as R;  // ����� ���� ��ȯ
    }
}