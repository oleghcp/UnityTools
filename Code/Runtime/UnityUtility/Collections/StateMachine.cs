using System;
using System.Collections.Generic;

namespace OlegHcp.Collections
{
    public interface IState
    {
        void Begin();
        void End();
    }

    [Serializable]
    public class StateMachine<TState, TData> where TState : class, IState
    {
        public event Action OnFinished_Event;
        public event Action<TState, TState> OnStateChanging_Event;

        private Node _startNode;
        private Node _currentNode;
        private Dictionary<TState, Node> _nodes = new Dictionary<TState, Node>();

        public TState CurrentState => _currentNode?.State;
        public bool IsAlive => _currentNode != null;
        public bool AtStart => _startNode == _currentNode;

        public StateMachine() { }

        //public StateMachine(RawGraph graph)
        //{
        //    graph.InitializeMachine(this);
        //}

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

        public void AddTransition(TState from, Func<TState, TData, bool> condition, TState to = null)
        {
            Transition transition = new Transition
            {
                Next = to == null ? null : _nodes[to],
                Condition = condition,
            };

            _nodes[from].Transitions.Add(transition);
        }

        public void SetAsStart(TState startState)
        {
            _startNode = _nodes[startState];
        }

        public void CheckConditions(TData data)
        {
            if (_currentNode == null)
                return;

            foreach (Transition transition in _currentNode.Transitions)
            {
                if (transition.Condition(_currentNode.State, data))
                {
                    _currentNode.State.End();
                    Node prevNode = _currentNode;
                    _currentNode = transition.Next;

                    if (_currentNode == null)
                    {
                        OnFinished_Event?.Invoke();
                        break;
                    }

                    OnStateChanging_Event?.Invoke(prevNode.State, _currentNode.State);
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
            public Func<TState, TData, bool> Condition;
        }
        #endregion
    }
}
