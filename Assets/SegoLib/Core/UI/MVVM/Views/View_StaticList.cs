using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.UI.MVVM
{
    public class View_StaticList : View_Base
    {
        [field: SerializeField] public StaticScroller StaticScroller { get; private set; }
        protected override void UpdateFromTargetProperty()
        {
            object propertyValue = GetPropertyValue();

            if (propertyValue == null) {
                StaticScroller.Show(System.Array.Empty<object>());
                return;
            }

            if (propertyValue is IEnumerable enumerable) {
                StaticScroller.Show(enumerable);
                return;
            }

            if (propertyValue is object item) {
                StaticScroller.Show(new object[] { item });
                return;
            }

            StaticScroller.Show(System.Array.Empty<object>());
        }
    }

    public abstract class View_StaticList<T> : View_Base
    {
        [field: SerializeField] public StaticScroller<T> StaticScroller { get; private set; }
        protected override void UpdateFromTargetProperty()
        {
            object propertyValue = GetPropertyValue();

            if(propertyValue == null) {
                StaticScroller.Show(System.Array.Empty<T>());
                return;
            }

            if(propertyValue is IEnumerable<T> enumerable) {
                StaticScroller.Show(enumerable);
                return;
            }

            if(propertyValue is T item) {
                StaticScroller.Show(new T[] { item });
                return;
            }

            StaticScroller.Show(System.Array.Empty<T>());
        }
    }
}
