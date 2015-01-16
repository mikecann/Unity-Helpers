using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityHelpers;

namespace UnityHelpers.View
{
    [ExecuteInEditMode]
    public class PageIndicator : MonoBehaviour
    {
        public ViewStateController viewStateController;
        public GameObject togglePrefab;

        private int lastStateCount;
        private int lastStateIndex;

        void Awake()
        {
            if (viewStateController==null) return;
            lastStateCount = -1;
            lastStateIndex = -1;
            Update();
        }
        
        void Update()
        {
            if(!viewStateController) return;

            if(viewStateController.states.Count!=lastStateCount)
                RebuildIndicators();

            if (viewStateController.CurrentStateIndex != lastStateIndex)
                UpdateTogglesStatus();
        }

        private void RebuildIndicators()
        {
            if (!viewStateController) return;
            if (!togglePrefab) return;

            // Destroy old ones
            while (transform.childCount != 0)
                DestroyImmediate(transform.GetChild(0).gameObject);

            // Add new ones
            for (int i = 0; i < viewStateController.states.Count; i++)
            {                
                var toggle = UnityUtils.Instantiate<Toggle>(togglePrefab);
                toggle.transform.SetParent(transform, false);
                toggle.onValueChanged.AddListener(b => OnToggleValueChanged(b, toggle));
            }

            // Remember this
            lastStateCount = viewStateController.states.Count;
        }

        private void OnToggleValueChanged(bool value, Toggle toggle)
        {
            if (value)
                viewStateController.SetState(toggle.transform.GetSiblingIndex());
        }

        private void UpdateTogglesStatus()
        {
            if (viewStateController.CurrentStateIndex == -1) return;
      
            foreach (var toggle in GetComponentsInChildren<Toggle>())
                toggle.isOn = false;

            var t = transform.GetChild(viewStateController.CurrentStateIndex).GetComponent<Toggle>();
            t.isOn = true;

            lastStateIndex = viewStateController.CurrentStateIndex;
        }
    }


}
