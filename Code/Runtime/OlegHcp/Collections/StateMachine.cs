using System;
using System.Collections.Generic;
using OlegHcp.CSharp.Collections;

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
        public event Action<TState, TState> OnStateChanged_Event;
        public event Action<TState> OnFinished_Event;

        private Dictionary<TState, Node> _nodes = new Dictionary<TState, Node>();
        private Node _startNode;
        private Node _currentNode;

        public TState CurrentState => _currentNode?.State;
        public bool IsAlive => _currentNode != null;
        public bool AtStart => _startNode == _currentNode;

        //public StateMachine() { }

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

        public void SetAsStart(TState startState)
        {
            _startNode = GetStateNode(startState);
        }

        public void AddTransition(TState from, Func<TState, TData, bool> condition, TState to = null)
        {
            Transition transition = new Transition()
            {
                Next = to == null ? null : GetStateNode(to),
                Condition = condition,
            };

            GetStateNode(from).Transitions.Add(transition);
        }

        public void CheckConditions(TData data)
        {
            if (_currentNode == null)
                return;

            foreach (Transition transition in _currentNode.Transitions)
            {
                if (transition.Condition(_currentNode.State, data))
                {
                    TState prevState = _currentNode.State;
                    prevState.End();

                    _currentNode = transition.Next;
                    if (_currentNode == null)
                    {
                        OnFinished_Event?.Invoke(prevState);
                        break;
                    }

                    TState curState = _currentNode.State;
                    curState.Begin();
                    OnStateChanged_Event?.Invoke(prevState, curState);
                    break;
                }
            }
        }

        private Node GetStateNode(TState state)
        {
            if (_nodes.TryGetValue(state, out Node node))
                return node;

            return _nodes.Place(state, new Node { State = state });
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
