using System;
using System.Collections.Generic;

namespace UnityUtility.Collections
{
    public interface IState
    {
        void Begin();
        void End();
    }

    [Serializable]
    public class StateMachine<TState> where TState : class, IState
    {
        private Node _startNode;
        private Node _currentNode;
        private Dictionary<TState, Node> _nodes;
        private Action<TState, TState> _onStateChanging;
        private Action _final;

        public TState CurrentState => _currentNode?.State;
        public bool IsAlive => _currentNode != null;

        public StateMachine()
        {
            _nodes = new Dictionary<TState, Node>();
        }

        public StateMachine(Action<TState, TState> onStateChanging = null, Action finalCallback = null) : this()
        {
            _onStateChanging = onStateChanging;
            _final = finalCallback;
        }

        public void Start()
        {
            if (_currentNode != null)
                return;

            _currentNode = _startNode;
            _currentNode.State.Begin();
        }

        public void AddState(TState state, bool startState = false)
        {
            Node node = new Node
            {
                State = state,
            };

            if (startState)
                _startNode = node;

            _nodes[state] = node;
        }

        public void AddTransition(TState from, TState to, Func<TState, bool> condition)
        {
            Transition transition = new Transition
            {
                Next = _nodes[to],
                Condition = condition,
            };

            _nodes[from].Transitions.Add(transition);
        }

        public void SetAsStart(TState startState)
        {
            _startNode = _nodes[startState];
        }

        public void AddExit(TState from, Func<TState, bool> condition)
        {
            Transition transition = new Transition
            {
                Condition = condition,
            };

            _nodes[from].Transitions.Add(transition);
        }

        public void CheckConditions()
        {
            if (_currentNode == null)
                return;

            foreach (Transition transition in _currentNode.Transitions)
            {
                if (transition.Condition(_currentNode.State))
                {
                    _currentNode.State.End();
                    Node prevNode = _currentNode;
                    _currentNode = transition.Next;

                    if (_currentNode == null)
                    {
                        _final?.Invoke();
                        break;
                    }

                    _onStateChanging?.Invoke(prevNode.State, _currentNode.State);
                    _currentNode.State.Begin();
                    break;
                }
            }
        }

        #region Entities
        [Serializable]
        private class Node
        {
            public List<Transition> Transitions;
            public TState State;

            public Node()
            {
                Transitions = new List<Transition>();
            }
        }

        [Serializable]
        private class Transition
        {
            public Node Next;
            public Func<TState, bool> Condition;
        }
        #endregion
    }
}
