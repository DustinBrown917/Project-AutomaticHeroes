using SegoLib;
using SegoLib.UI.MVVM;

namespace Game
{
    public class VM_WorldDate : VM_Base<WorldDate>
    {
        public WorldDate.ETimeOfDay TimeOfDay
        {
            get => m_timeOfDay;
            set => PropertySetter.Set(ref m_timeOfDay, value, m_timeOfDay == value, OnPropertyChanged);
        }
        private WorldDate.ETimeOfDay m_timeOfDay;

        public string TimeOfDayString
        {
            get => m_timeOfDayString;
            set => PropertySetter.Set(ref m_timeOfDayString, value, m_timeOfDayString == value, OnPropertyChanged);
        }
        private string m_timeOfDayString;

        public string DateString
        {
            get => m_dateString;
            set => PropertySetter.Set(ref m_dateString, value, m_dateString == value, OnPropertyChanged);
        }
        private string m_dateString;

        protected override bool IsEqual(WorldDate a, WorldDate b) => a == b;

        protected override void SetValuesToDefault()
        {
            TimeOfDay = WorldDate.ETimeOfDay.Night;
            TimeOfDayString = string.Empty;
            DateString = string.Empty;
        }

        protected override void UpdateValues()
        {
            TimeOfDay = Model.TimeOfDay;
            TimeOfDayString = WorldDate.GetTimeOfDayString(Model.TimeOfDay);
            DateString = Model.GetDateString();
        }

        protected override void OnModelChangedToNotNull()
        {
            Model.timeOfDayChanged += Model_TimeOfDayChanged;
            Model.currentDayChanged += Model_CurrentDayChanged;
        }

        protected override void OnModelChangingFromNotNull()
        {
            Model.timeOfDayChanged -= Model_TimeOfDayChanged;
            Model.currentDayChanged -= Model_CurrentDayChanged;
        }

        private void Model_CurrentDayChanged(WorldDate source, int oldValue, int newValue)
        {
            DateString = Model.GetDateString();
        }

        private void Model_TimeOfDayChanged(WorldDate source, WorldDate.ETimeOfDay oldValue, WorldDate.ETimeOfDay newValue)
        {
            TimeOfDay = newValue;
            TimeOfDayString = WorldDate.GetTimeOfDayString(newValue);
        }
    } 
}
