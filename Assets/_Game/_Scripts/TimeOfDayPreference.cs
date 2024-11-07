using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Time of Day Preference", menuName = "Game/Time of Day Preference")]
    public class TimeOfDayPreference : ScriptableObject
    {
        [field: SerializeField] public float Night { get; private set; }
        [field: SerializeField] public float Dawn { get; private set; }
        [field: SerializeField] public float Day { get; private set; }
        [field: SerializeField] public float Dusk { get; private set; }


        public float GetPreference(WorldDate.ETimeOfDay timeOfDay)
        {
            switch (timeOfDay) {
                case WorldDate.ETimeOfDay.Night: return Night;
                case WorldDate.ETimeOfDay.Dawn: return Dawn;
                case WorldDate.ETimeOfDay.Day: return Day;
                case WorldDate.ETimeOfDay.Dusk: return Dusk;
                default: return 0.5f;
            }
        }

        public float GetPreferenceForCurrentTime()
        {
            return GetPreference(WorldState.Instance.Date.TimeOfDay);
        }
    } 
}
