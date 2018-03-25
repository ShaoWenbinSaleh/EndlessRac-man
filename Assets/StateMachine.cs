using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

//delegate, like function poiters in C++
public delegate void StateFuncEventHandler();
public delegate bool StateTransEventHandler();

public class State
{
    public event StateFuncEventHandler StateFunc;
    public readonly Dictionary<State, StateTransEventHandler> StateTrans;
    public string StateName { get; private set; }
    public bool IsRun = false;
    private int StateFuncDeltaTime { get; set; }
    public State(string name)
    {
        StateName = name;
        StateTrans = new Dictionary<State, StateTransEventHandler>();
        StateFuncDeltaTime = GlobalVariables.StateFuncDeltaTime;
        
        //multi-thread
        var th = new Thread(ThreadUpdate);
        th.Start();
    }

    protected void ThreadUpdate()
    {
        while (true)
        {
            if (!IsRun)
            {
                continue;
            }

            Thread.Sleep(StateFuncDeltaTime);
            if (StateFunc != null)
            {
                StateFunc();
            }
        }
    }

    public void RegisterTranslate(State target, StateTransEventHandler conditional)
    {
        StateTrans.Add(target, conditional);
    }
}

public class StateController : State
{
    public StateController(string name) : base(name)
    {
        _states = new Dictionary<string, State>();
        _stateParams = new Dictionary<string, object>();
        StateFunc += CheckTranslate;
        IsRun = true;
        var th = new Thread(ThreadUpdate);
        th.Start();
    }

    private readonly Dictionary<string, State> _states;
    private readonly Dictionary<string, object> _stateParams;
    private State _currentState;

    public string GetCurrentStateName()
    {
        return _currentState.StateName;
    }

    public State AddState(string statename)
    {
        State temp = new State(statename);
        _states.Add(statename, temp);
        SetDefault(temp);
        return temp;
    }

    public void AddState(State s)
    {
        _states.Add(s.StateName, s);
        SetDefault(s);
    }

    public void RemoveState(string statename)
    {
        _states.Remove(statename);
    }

    private void SetDefault(State s)
    {
        if (_states.Count == 1)
        {
            _currentState = s;
            s.IsRun = true;
        }
    }

    private void TranslateState(string name)
    {
        if (!_states.ContainsKey(name)) return;
        _currentState.IsRun = false;
        _currentState = _states[name];
        _states[name].IsRun = true;
    }

    public State GetState(string statename)
    {
        return _states[statename];
    }

    public void RegisterParams(string paramName, float value)
    {
        _stateParams.Add(paramName, value);
    }

    public float GetParams(string paraName)
    {
        return (float) _stateParams[paraName];
    }
    
    public void SetParams(string paraName, float value)
    {
        _stateParams[paraName] = value;
    }

    //bool
    public void RegisterParams(string paramName, bool value)
    {
        _stateParams.Add(paramName, value);
    }
    
    public bool GetBoolParams(string paraName)
    {
        return (bool) _stateParams[paraName];
    }

    public void SetParams(string paraName, bool value)
    {
        _stateParams[paraName] = value;
    }

    private void CheckTranslate()
    {
        foreach (var item in _currentState.StateTrans)
        {
            if (item.Value())
            {
                TranslateState(item.Key.StateName);
            }
        }
    }
}
