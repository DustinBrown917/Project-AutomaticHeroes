using SegoLib;
using SegoLib.UI.MVVM;
using System.Collections.Generic;

namespace Game.UI
{
    public class VM_ILocation : VM_Base<ILocation>
    {
        public int Id
        {
            get => m_id;
            private set => PropertySetter.Set(ref m_id, value, m_id == value, OnPropertyChanged);
        }
        private int m_id;

        public string DisplayName
        {
            get => m_displayName;
            private set => PropertySetter.Set(ref m_displayName, value, m_displayName == value, OnPropertyChanged);
        }
        private string m_displayName;

        public IEnumerable<Hero> Visitors
        {
            get => m_visitors;
            private set {
                m_visitors.Clear();

                if(value != null)
                    m_visitors.AddRange(value);

                OnPropertyChanged();
            }
        }
        private readonly List<Hero> m_visitors = new();

        protected override bool IsEqual(ILocation a, ILocation b) => a == b;

        protected override void SetValuesToDefault()
        {
            Id = -1;
            DisplayName = string.Empty;
            Visitors = null;
        }

        protected override void UpdateValues()
        {
            Id = Model.Id;
            DisplayName = Model.DisplayName;
            Visitors = Model.Visitors;
        }

        protected override void OnModelChangedToNotNull()
        {
            Model.visitorsUpdated += Model_VisitorsUpdated;
        }

        protected override void OnModelChangingFromNotNull()
        {
            Model.visitorsUpdated -= Model_VisitorsUpdated;
        }

        private void Model_VisitorsUpdated(ILocation source, IReadOnlyList<Hero> newValue)
        {
            Visitors = newValue;
        }
    }

}