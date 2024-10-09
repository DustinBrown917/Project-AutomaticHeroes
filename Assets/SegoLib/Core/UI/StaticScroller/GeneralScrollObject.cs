using System;
using UnityEngine;

namespace SegoLib.UI
{
    public class GeneralScrollObject
    {
        public object Obj { get; }

        public string DisplayName => GetDisplayName?.Invoke(Obj);
        public string Description => GetDescription?.Invoke(Obj);
        public Sprite DisplaySprite => GetDisplaySprite?.Invoke(Obj);

        public event Action onDisplayNameChanged;
        public event Action onDescriptionChanged;
        public event Action onDisplaySpriteChanged;

        private Func<object, string> GetDisplayName { get; }
        private Func<object, string> GetDescription { get; }
        private Func<object, Sprite> GetDisplaySprite { get; }
        private Action<object, GeneralScrollObject> HookUpEvents { get; }
        private Action<object, GeneralScrollObject> UnHookEvents { get; }

        public GeneralScrollObject(object obj, Func<object, string> getDisplayName, Func<object, string> getDescription, Func<object, Sprite> getDisplaySprite, Action<object, GeneralScrollObject> hookUpEvents, Action<object, GeneralScrollObject> unHookEvents)
        {
            Obj = obj;
            GetDisplayName = getDisplayName;
            GetDescription = getDescription;
            GetDisplaySprite = getDisplaySprite;
            UnHookEvents = unHookEvents;
            HookUpEvents = hookUpEvents;

            HookUpEvents?.Invoke(Obj, this);
        }

        public void Dispose()
        {
            UnHookEvents?.Invoke(Obj, this);
        }



        public void RaiseOnDisplayNameChanged() => onDisplayNameChanged?.Invoke();
        public void RaiseOnDisplayNameChanged(object newVal) => RaiseOnDisplayNameChanged();
        public void RaiseOnDisplayNameChanged(object oldVal, object newVal) => RaiseOnDisplayNameChanged();

        public void RaiseOnDescriptionChanged() => onDescriptionChanged?.Invoke();
        public void RaiseOnDescriptionChanged(object newVal) => RaiseOnDescriptionChanged();
        public void RaiseOnDescriptionChanged(object newVal, object oldVal) => RaiseOnDescriptionChanged();

        public void RaiseOnDisplaySpriteChanged() => onDisplaySpriteChanged?.Invoke();
        public void RaiseOnDisplaySpriteChanged(object newVal) => RaiseOnDisplaySpriteChanged();
        public void RaiseOnDisplaySpriteChanged(object newVal, object oldVal) => RaiseOnDisplaySpriteChanged();

    }
}
