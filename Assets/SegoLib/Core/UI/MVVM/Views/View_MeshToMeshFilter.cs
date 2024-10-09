using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.UI.MVVM
{
    public class View_MeshToMeshFilter : View_Base
    {
        [SerializeField] private MeshFilter m_meshFilter;

        protected override void UpdateFromTargetProperty()
        {
            Mesh mesh = (Mesh)m_targetProperty.GetValue(m_observedViewModel);
            m_meshFilter.mesh = mesh;
        }
    }
}
