using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.UI
{
    public interface ITooltipDataProvider
    {
        string TooltipContentType { get; }
        Dictionary<string, object> GetTooltipData();
    }
}
