using System;
using System.Collections.Generic;
using OlegHcp.CSharp.Collections;
using OlegHcp.NodeBased.Service;
using OlegHcp.Tools;

namespace OlegHcp.Collections
{
    public interface IState
    {
        void OnEnter();
        void OnExit();
    }

    public interface ICondition<TData>
    {
        bool Check(TData data);
    }

    [Serializable]
    public class StateMachine<TState, TData> where TState : class, IState
    {
        public event Action<TState, TState> OnStateChanged_Event;
        public event Action<TState> OnFinished_Event;

        private Dictionary<TState, List<Transition>> _states = new Dictionary<TState, List<Transition>>();
        private TState _startState;
        private TState _currentState;

        public bool IsAlive => _currentState != null;
        public bool AtStart => _startState == _currentState;
        public TState CurrentState => _currentState;

        public StateMachine()
        {

        }

        public StateMachine(RawGraph graph)
        {
            graph.InitializeStateMachine(this);
        }

        public void Run()
        {
            if (_currentState != null)
                return;

            _currentState = _startState;
            _currentState.OnEnter();
        }

        public void Run(TState state)
        {
            if (state == null)
                throw ThrowErrors.NullParameter(nameof(state));

            TState prevState = _currentState;
            prevState?.OnExit();

            _currentState = state;
            state.OnEnter();

            if (prevState != null)
                OnStateChanged_Event?.Invoke(prevState, state);
        }

        public void Stop()
        {
            if (_currentState == null)
                return;

            TState prevState = _currentState;
            _currentState = null;
            prevState.OnExit();
            OnFinished_Event?.Invoke(prevState);
        }

        public void SetAsStart(TState startState)
        {
            if (startState == null)
                throw ThrowErrors.NullParameter(nameof(startState));

            TryCacheState(startState);
            _startState = startState;
        }

        public void AddTransition(TState from, Func<TData, bool> condition, TState to = null)
        {
            AddTransition(from, new InnerCondition(condition), to);
        }

        public void AddTransition(TState from, ICondition<TData> condition, TState to = null)
        {
            if (from == null)
                throw ThrowErrors.NullParameter(nameof(from));

            if (condition == null)
                throw ThrowErrors.NullParameter(nameof(condition));

            if (to != null)
            {
                TryCacheState(to);
            }

            Transition transition = new Transition()
            {
                Next = to,
                Condition = condition,
            };

            TryCacheState(from).Add(transition);
        }

        public bool CheckConditions(TData data)
        {
            if (_currentState == null)
                return false;

            foreach (Transition transition in _states[_currentState])
            {
                if (transition.Condition.Check(data))
                {
                    TState prevState = _currentState;
                    prevState.OnExit();
                    _currentState = transition.Next;

                    if (_currentState == null)
                    {
                        OnFinished_Event?.Invoke(prevState);
                    }
                    else
                    {
                        TState curState = _currentState;
                        curState.OnEnter();
                        OnStateChanged_Event?.Invoke(prevState, curState);
                    }

                    return true;
                }
            }

            return false;
        }

        private List<Transition> TryCacheState(TState state)
        {
            if (_states.TryGetValue(state, out var transitions))
                return transitions;

            return _states.Place(state, new List<Transition>());
        }

        #region Entities
        [Serializable]
        private struct Transition
        {
            public TState Next;
            public ICondition<TData> Condition;
        }

        [Serializable]
        private class InnerCondition : ICondition<TData>
        {
            private Func<TData, bool> _condition;

            public InnerCondition(Func<TData, bool> condition)
            {
                _condition = condition;
            }

            public bool Check(TData data)
            {
                return _condition(data);
            }
        }
        #endregion
    }
}
