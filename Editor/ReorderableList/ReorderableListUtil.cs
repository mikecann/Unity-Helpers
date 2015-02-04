using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UnityHelpers
{
    public class ReorderableListUtil
    {
        public static void SetColumns<T>(ReorderableList list, List<ReorderableListColumn<T>> columns)
        {
            var widthRequired = columns.Where(c => c.Width > -1).Sum(c => c.Width);

            list.drawHeaderCallback = (Rect rect) =>
            {
                var x = rect.x + 14;
                var y = rect.y;
                var ratioWidth = Math.Max(0, rect.width - widthRequired);

                foreach (var col in columns)
                {
                    var width = col.Width;
                    if (width < 0) width = (int)(ratioWidth * col.WidthRatio);
                    EditorGUI.LabelField(new Rect(x, y, width, EditorGUIUtility.singleLineHeight), col.Name);
                    x += width;
                }    
            };

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var obj = (T)list.list[index];
                var x = rect.x;
                var y = rect.y;
                var ratioWidth = Math.Max(0,rect.width - widthRequired);

                foreach (var col in columns)
                {
                    var width = col.Width;
                    if (width < 0) width = (int)(ratioWidth * col.WidthRatio);

                    col.ItemRenderer(obj, new Rect(x, y, width, EditorGUIUtility.singleLineHeight), index, isActive, isFocused);
                    x += width;
                }
            };
        }


    }
}
