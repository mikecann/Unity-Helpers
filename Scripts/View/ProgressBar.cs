using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UnityHelpers.View
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class ProgressBar : MonoBehaviour
    {
        public RectTransform barMask;

        [Range(0,1)]
        public float value = 1;

        private RectTransform rectTransform;

        void Update()
        {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null) return;
            barMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value * rectTransform.rect.width);
        }
    }
}
