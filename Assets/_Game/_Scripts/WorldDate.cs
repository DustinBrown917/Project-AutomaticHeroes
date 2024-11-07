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

        private static readonly ETimeOfDay[] m_timeOfDaySchedule = new ETimeOfDay[] {
            ETimeOfDay.Night, // 00:00
            ETimeOfDay.Night, // 01:00
            ETimeOfDay.Night, // 02:00
            ETimeOfDay.Night, // 03:00
            ETimeOfDay.Night, // 04:00
            ETimeOfDay.Dawn,  // 05:00
            ETimeOfDay.Dawn,  // 06:00
            ETimeOfDay.Dawn,  // 07:00
            ETimeOfDay.Dawn,  // 08:00
            ETimeOfDay.Dawn,  // 09:00
            ETimeOfDay.Day,   // 10:00
            ETimeOfDay.Day,   // 11:00
            ETimeOfDay.Day,   // 12:00
            ETimeOfDay.Day,   // 13:00
            ETimeOfDay.Day,   // 14:00
            ETimeOfDay.Day,   // 15:00
            ETimeOfDay.Day,   // 16:00
            ETimeOfDay.Dusk,  // 17:00
            ETimeOfDay.Dusk,  // 18:00
            ETimeOfDay.Dusk,  // 19:00
            ETimeOfDay.Dusk,  // 20:00
            ETimeOfDay.Dusk,  // 21:00
            ETimeOfDay.Night, // 22:00
            ETimeOfDay.Night, // 23:00
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
            TimeOfDay = m_timeOfDaySchedule[CurrentHour];
        }

        public ETimeOfDay CalculateTimeOfDayAtFutureOffset(float secondsIntoFuture)
        {
            float seconds = m_elapsedSecondsThisDay + (int)secondsIntoFuture;

            int dayTimeSeconds = (int)seconds % SECONDS_PER_DAY;

            int hour = dayTimeSeconds / SECONDS_PER_HOUR;

            return m_timeOfDaySchedule[hour];
        }

        public string GetDateString()
        {
            return "Day " + CurrentDay;
        }
    } 
}
