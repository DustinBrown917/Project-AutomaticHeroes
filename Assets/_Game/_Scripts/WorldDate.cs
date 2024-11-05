using NaughtyAttributes;
using SegoLib;
using UnityEngine;

namespace Game
{
    public class WorldDate : MonoBehaviour, WorldTime.IListener
    {
        public static int SECONDS_PER_HOUR = 3600;
        public static int SECONDS_PER_DAY = SECONDS_PER_HOUR * 24;

        public enum ETimeOfDay : byte
        {
            Night,
            Dawn,
            Day,
            Dusk
        }

        public enum EPreference : byte
        {
            None,
            Diurnal,
            Nocturnal,
        }

        private class TimeOfDayInfo
        {
            public readonly ETimeOfDay timeOfDay;
            public readonly float diurnalPreference;
            public readonly float nocturnalPreference;

            public TimeOfDayInfo(ETimeOfDay timeOfDay, float diurnalPreference, float nocturnalPreference)
            {
                this.timeOfDay = timeOfDay;
                this.diurnalPreference = diurnalPreference;
                this.nocturnalPreference = nocturnalPreference;
            }
        }

        private static readonly TimeOfDayInfo[] m_timeOfDaySchedule = new TimeOfDayInfo[] {
            new(ETimeOfDay.Night, 0.0f, 1.0f), // 00:00
            new(ETimeOfDay.Night, 0.0f, 1.0f), // 01:00
            new(ETimeOfDay.Night, 0.0f, 1.0f), // 02:00
            new(ETimeOfDay.Night, 0.0f, 1.0f), // 03:00
            new(ETimeOfDay.Night, 0.0f, 1.0f), // 04:00
            new(ETimeOfDay.Dawn, 0.8f, 0.5f),  // 05:00
            new(ETimeOfDay.Dawn, 0.8f, 0.5f),  // 06:00
            new(ETimeOfDay.Dawn, 0.8f, 0.5f),  // 07:00
            new(ETimeOfDay.Dawn, 0.8f, 0.5f),  // 08:00
            new(ETimeOfDay.Dawn, 0.8f, 0.5f),  // 09:00
            new(ETimeOfDay.Day, 1.0f, 0.0f),   // 10:00
            new(ETimeOfDay.Day, 1.0f, 0.0f),   // 11:00
            new(ETimeOfDay.Day, 1.0f, 0.0f),   // 12:00
            new(ETimeOfDay.Day, 1.0f, 0.0f),   // 13:00
            new(ETimeOfDay.Day, 1.0f, 0.0f),   // 14:00
            new(ETimeOfDay.Day, 1.0f, 0.0f),   // 15:00
            new(ETimeOfDay.Day, 1.0f, 0.0f),   // 16:00
            new(ETimeOfDay.Dusk, 0.5f, 0.8f),  // 17:00
            new(ETimeOfDay.Dusk, 0.5f, 0.8f),  // 18:00
            new(ETimeOfDay.Dusk, 0.5f, 0.8f),  // 19:00
            new(ETimeOfDay.Dusk, 0.5f, 0.8f),  // 20:00
            new(ETimeOfDay.Dusk, 0.5f, 0.8f),  // 21:00
            new(ETimeOfDay.Night, 0.0f, 1.0f), // 22:00
            new(ETimeOfDay.Night, 0.0f, 1.0f), // 23:00
            null, // Will be filled in with the element at 0 to make calculations between [0] and [23] easier.
        };

        static WorldDate()
        {
            m_timeOfDaySchedule[m_timeOfDaySchedule.Length - 1] = m_timeOfDaySchedule[0];
        }

        public static string GetTimeOfDayString(ETimeOfDay timeOfDay)
        {
            return timeOfDay.ToString();
        }

        public int CurrentDay
        {
            get => m_currentDay;
            private set => SetCurrentDay(value);
        }
        private int m_currentDay = 1;
        public event ValueChangedWithSourceEvent<WorldDate, int> currentDayChanged;

        public ETimeOfDay TimeOfDay
        {
            get => m_timeOfDay;
            private set => SetTimeOfDay(value);
        }
        [SerializeField, ReadOnly] private ETimeOfDay m_timeOfDay;
        public event ValueChangedWithSourceEvent<WorldDate, ETimeOfDay> timeOfDayChanged;

        public int CurrentHour
        {
            get => m_hour;
            set => SetCurrentHour(value);
        }
        [SerializeField, ReadOnly] private int m_hour;
        public event ValueChangedWithSourceEvent<WorldDate, int> currentHourChanged;

        public int ElapsedSecondsThisDay
        {
            get => m_elapsedSecondsThisDay;
            private set => SetElapsedSecondsThisDay(value);
        }
        [SerializeField, ReadOnly] private int m_elapsedSecondsThisDay = 18_000;
        public event ValueChangedWithSourceEvent<WorldDate, int> elapsedSecondsThisDayChanged;

        private float m_secondsRemainder;

        private void Start()
        {
            WorldState.Instance.Time.AddListener(this);
            UpdateTimeOfDay();
        }

        private void OnDestroy()
        {
            if(WorldState.Instance != null && WorldState.Instance.Time != null) {
                WorldState.Instance.Time.RemoveListener(this);
            }
        }

        public void OnWorldTimeUpdate(float deltaSeconds)
        {
            // Find seconds remainder, and how many whole seconds elapsed this update
            deltaSeconds += m_secondsRemainder;
            int elapsedWholeSeconds = (int)deltaSeconds;
            m_secondsRemainder = deltaSeconds - elapsedWholeSeconds;

            // Find how many days have elapsed
            elapsedWholeSeconds += ElapsedSecondsThisDay;
            int elapsedWholeDays = elapsedWholeSeconds / SECONDS_PER_DAY;

            // Find how many seconds have elapsed in this day
            ElapsedSecondsThisDay = elapsedWholeSeconds - (elapsedWholeDays * SECONDS_PER_DAY);

            CurrentDay += elapsedWholeDays;

            UpdateTimeOfDay();
        }

        private void SetCurrentDay(int newValue)
        {
            if (newValue < 0)
                newValue = 0;

            if (newValue == m_currentDay)
                return;

            int oldValue = m_currentDay;
            m_currentDay = newValue;

            currentDayChanged?.Invoke(this, oldValue, newValue);
        }

        private void SetTimeOfDay(ETimeOfDay newValue)
        {
            if (newValue == m_timeOfDay)
                return;

            ETimeOfDay oldValue = m_timeOfDay;
            m_timeOfDay = newValue;

            timeOfDayChanged?.Invoke(this, oldValue, newValue);
        }

        private void SetElapsedSecondsThisDay(int newValue)
        {
            if (newValue < 0)
                newValue = 0;

            if(newValue == m_elapsedSecondsThisDay) 
                return;

            int oldValue = m_elapsedSecondsThisDay;
            m_elapsedSecondsThisDay = newValue;

            elapsedSecondsThisDayChanged?.Invoke(this, oldValue, newValue);
        }

        private void SetCurrentHour(int newValue)
        {
            if(newValue < 0)
                newValue = 0;

            if (newValue == m_hour)
                return;

            int oldValue = m_hour;
            m_hour = newValue;

            currentHourChanged?.Invoke(this, oldValue, newValue);
        }

        private void UpdateTimeOfDay()
        {
            CurrentHour = m_elapsedSecondsThisDay / SECONDS_PER_HOUR;
            TimeOfDay = m_timeOfDaySchedule[CurrentHour].timeOfDay;
        }

        public ETimeOfDay CalculateTimeOfDayAtFutureOffset(float secondsIntoFuture)
        {
            float seconds = m_elapsedSecondsThisDay + (int)secondsIntoFuture;

            int dayTimeSeconds = (int)seconds % SECONDS_PER_DAY;

            int hour = dayTimeSeconds / SECONDS_PER_HOUR;

            return m_timeOfDaySchedule[hour].timeOfDay;
        }

        public float GetTimeOfDayPreference(EPreference preference)
        {
            if (preference == EPreference.Diurnal) return m_timeOfDaySchedule[CurrentHour].diurnalPreference;
            if (preference == EPreference.Nocturnal) return m_timeOfDaySchedule[CurrentHour].nocturnalPreference;
            return 1;
        }

        public string GetDateString()
        {
            return "Day " + CurrentDay;
        }
    } 
}
