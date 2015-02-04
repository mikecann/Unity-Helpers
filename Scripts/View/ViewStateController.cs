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
        public GameObject selected;
        public List<GameObject> states;
        public UnityEvent stateChanged;

        private string currentStateName = "None";
        private int currentStateIndex = -1;

        void Awake()
        {
            if (states == null)
                states = new List<GameObject>();

            // Lets init the states
            foreach (var state in states)
            {
                if (state == selected) Enable(state);
                else Disable(state);
            }

            if (selected == null)
            {
                currentStateIndex = -1;
                currentStateName = "None";
            }
            else
            {
                currentStateIndex = states.IndexOf(selected);
                currentStateName = selected.name;
            }

        }

        public void SetNoState()
        {
            if (selected == null) return;
            foreach(var state in states)
                state.SetActive(false);
            selected = null;
            currentStateName = null;
            currentStateIndex = -1;
        }

        public void SetState(string stateName)
        {
            if (currentStateName == stateName) return;
            if (states == null) return;
            SetState(states.FirstOrDefault(s => s == null ? false : s.name == stateName));
        }

        public void SetState(int stateIndex)
        {
            if (stateIndex == currentStateIndex) return;
            if (states == null || states.Count - 1 < stateIndex || stateIndex < 0) return;
            SetState(states[stateIndex]);
        }

        public void SetState(GameObject obj)
        {
            if (obj == selected) return;

            if (!states.Contains(obj))
                throw new Exception("Cannot set state '" + obj + "' is not part of the possible states!");

            foreach(var state in states)
            {
                if (state == obj) continue;
                if (state == null) continue;
                Disable(state);
            }

            if (obj != null)
                Enable(obj);
   
            selected = obj;
            stateChanged.Invoke();
            currentStateName = obj.name;
            currentStateIndex = states.IndexOf(obj);
        }

        private void Enable(GameObject state)
        {
            //var canvas = state.GetComponent<Canvas>();
            //var canvasGroup = state.GetComponent<CanvasGroup>();
            //if (canvas != null && Application.isPlaying)
            //{
            //    state.SetActive(true);
            //    canvas.enabled = true;
            //}
            //else if (canvasGroup != null && Application.isPlaying)
            //{
            //    state.SetActive(true);
            //    canvasGroup.alpha = 1;
            //}
            //else state.SetActive(true);
            state.SetActive(true);
        }

        private void Disable(GameObject state)
        {
            //var canvas = state.GetComponent<Canvas>();
            //var canvasGroup = state.GetComponent<CanvasGroup>();
            //if (canvas != null && Application.isPlaying)
            //{
            //    state.SetActive(true);
            //    canvas.enabled = false;
            //}
            //else if (canvasGroup != null && Application.isPlaying)
            //{
            //    state.SetActive(true);
            //    canvasGroup.alpha = 0;
            //}                
            //else state.SetActive(false);
            state.SetActive(false);
        }   

        public void NextState()
        {
            if (states == null || states.Count == 0) return;
            if (selected == null) SetState(0);
            else
            {
                var indx = currentStateIndex + 1;
                if (indx > states.Count-1) indx = 0;
                SetState(indx);
            }
        }

        public void PreviousState()
        {
            if (states == null || states.Count == 0) return;
            if (selected == null) SetState(0);
            else
            {
                var indx = currentStateIndex - 1;
                if (indx < 0) indx = states.Count - 1;
                SetState(indx);
            }
        }

        public string CurrentStateName { get { return currentStateName; } }
        public int CurrentStateIndex { get { return currentStateIndex; } }

        
    }
}
