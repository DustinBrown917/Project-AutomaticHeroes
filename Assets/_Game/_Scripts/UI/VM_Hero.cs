using SegoLib;
using SegoLib.UI.MVVM;
using System;
using UnityEngine;

namespace Game.UI
{
    public class VM_Hero : VM_Base<Hero>
    {
        protected override Func<Hero, Hero, bool> ModelEquals => (x, y) => x == y;

        protected override void SetValuesToDefault()
        {

        }

        protected override void UpdateValues()
        {

        }

        protected override void OnModelChangedToNotNull()
        {

        }

        protected override void OnModelChangingFromNotNull()
        {

        }

    }

}