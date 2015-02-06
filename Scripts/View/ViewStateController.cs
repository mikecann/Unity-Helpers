using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityHelpers.View
{
    [ExecuteInEditMode]
    public class ViewStateController : MonoBehaviour
    {
        public List<GameObject> states = new List<GameObject>();
        public ViewStateControllerStateChangedEvent stateChanged = new ViewStateControllerStateChangedEvent();

        public Action<GameObject> enableHandler = obj => obj.SetActive(true);
        public Action<GameObject> disableHandler = obj => obj.SetActive(false);

        private GameObject currentState;

        void Awake()
        {
            // Lets find the current state (and allow each state to Awake() );
            foreach (var state in states)
            {
                var before = state.activeSelf;
                if (before) currentState = state;
                state.SetActive(true);
                state.SetActive(before);
            }
        }

        public void SetNoState()
        {
            GameObject obj = null;
            SetState(obj);
        }

        public GameObject GetState(string stateName)
        {
            return states.FirstOrDefault(s => s == null ? false : s.name == stateName);
        }

        public T GetState<T>() where T : Component
        {
            foreach (var state in states)
                if (state.Has<T>())
                    return state.Get<T>();
            return null;
        }

        public void SetState(string stateName)
        {
            if (CurrentStateName == stateName) return;
            if (states == null) return;
            SetState(GetState(stateName));
        }

        public void SetState(int stateIndex)
        {
            if (stateIndex == CurrentStateIndex) return;
            if (states == null || states.Count - 1 < stateIndex || stateIndex < 0) return;
            SetState(states[stateIndex]);
        }

        public void SetState(GameObject obj)
        {
            if (obj == CurrentState) return;

            if (obj !=null && !states.Contains(obj))
                throw new Exception("Cannot set state '" + obj + "' is not part of the possible states!");

            foreach(var state in states)
            {
                if (state == obj) continue;
                if (state == null) continue;
                if(disableHandler!=null)
                    disableHandler(state);
            }

            if (obj != null)
                if (enableHandler != null) 
                    enableHandler(obj);

            var lastState = CurrentState;
            currentState = obj;
            stateChanged.Invoke(lastState, currentState);
        }

        public void NextState()
        {
            if (states == null || states.Count == 0) return;
            if (CurrentState == null) SetState(0);
            else
            {
                var indx = CurrentStateIndex + 1;
                if (indx > states.Count-1) indx = 0;
                SetState(indx);
            }
        }

        public void PreviousState()
        {
            if (states == null || states.Count == 0) return;
            if (CurrentState == null) SetState(0);
            else
            {
                var indx = CurrentStateIndex - 1;
                if (indx < 0) indx = states.Count - 1;
                SetState(indx);
            }
        }

        public GameObject CurrentState 
        {
            get 
            { 
                return currentState; 
            } 
        }

        public string CurrentStateName 
        {
            get 
            {
                if (CurrentState == null) return null;
                return CurrentState.name; 
            } 
        }

        public int CurrentStateIndex 
        {
            get 
            {
                if (CurrentState == null) return -1;
                return states.IndexOf(CurrentState); 
            }
        }       
        
    }

   

    public class ViewStateControllerStateChangedEvent : UnityEvent<GameObject, GameObject> { }
}
