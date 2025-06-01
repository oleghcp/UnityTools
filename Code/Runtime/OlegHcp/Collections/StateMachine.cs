using System;
using System.Collections.Generic;
using System.Linq;
using OlegHcp.CSharp.Collections;
using OlegHcp.Tools;

namespace OlegHcp.Collections
{
    public interface IState
    {
        int Id { get; }
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

        public void Run()
        {
            if (_currentNode != null)
                return;

            _currentNode = _startNode;
            _currentNode.State.Begin();
        }

        public void Run(TState state)
        {
            if (state == null)
                throw ThrowErrors.NullParameter(nameof(state));

            TState prevState = _currentNode?.State;
            prevState?.End();

            _currentNode = _nodes[state];
            state.Begin();

            if (prevState != null)
                OnStateChanged_Event?.Invoke(prevState, state);
        }

        public void Run(int stateId)
        {
            Node targetNode = _nodes.Values.First(item => item.State.Id == stateId);
            Run(targetNode.State);
        }

        public void Stop()
        {
            if (_currentNode == null)
                return;

            TState prevState = _currentNode.State;
            _currentNode = null;
            prevState.End();
            OnFinished_Event?.Invoke(prevState);
        }

        public void SetAsStart(TState startState)
        {
            if (startState == null)
                throw ThrowErrors.NullParameter(nameof(startState));

            _startNode = GetStateNode(startState);
        }

        public void AddTransition(TState from, Func<TState, TData, bool> condition, TState to = null)
        {
            if (from == null)
                throw ThrowErrors.NullParameter(nameof(from));

            if (condition == null)
                throw ThrowErrors.NullParameter(nameof(condition));

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
