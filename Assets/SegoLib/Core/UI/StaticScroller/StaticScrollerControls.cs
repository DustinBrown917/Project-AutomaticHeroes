using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SegoLib.UI
{
    public class StaticScrollerControls : MonoBehaviour
    {
        [field: SerializeField] public Scrollbar Scrollbar { get; private set; }
        /// <summary>
        /// The button typically used for scrolling up or left, depending on orientation.
        /// </summary>
        [field: Tooltip("The button typically used for scrolling up or left, depending on orientation.")]
        [field: SerializeField] public Button DecreaseScrollIndexButton { get; private set; }
        /// <summary>
        /// The button typically used for scrolling down or right, depending on orientation.
        /// </summary>
        [field: Tooltip("The button typically used for scrolling down or right, depending on orientation.")]
        [field: SerializeField] public Button IncreaseScrollIndexButton { get; private set; }
        /// <summary>
        /// How much the ScrollIndex changes by with each press of the Increase or Decrease button.
        /// </summary>
        [field: Tooltip("How much the ScrollIndex changes by with each press of the Increase or Decrease button.")]
        [field: SerializeField] public int ScrollStep { get; private set; } = 1;
    } 
}
