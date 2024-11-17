using SegoLib.UI.MVVM;
using UnityEngine;

namespace Game.UI
{
    public class View_ProportionBarProportions : View_Base
    {
        [SerializeField] private ProportionBar m_proportionBar;

        protected override void UpdateFromTargetProperty()
        {
            m_proportionBar.SetProportions(GetPropertyValue<float[]>());
        }
    }
}
