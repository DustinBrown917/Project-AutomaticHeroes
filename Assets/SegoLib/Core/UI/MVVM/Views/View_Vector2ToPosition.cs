using UnityEngine;

namespace SegoLib.UI.MVVM
{
    public class View_Vector2ToPosition : View_Base
    {
        [Tooltip("If left null, will default to this.transform.")]
        [SerializeField] private Transform m_transformToMove;

        protected override void Awake()
        {
            base.Awake();
            if(m_transformToMove == null)
                m_transformToMove = transform;
        }

        protected override void UpdateFromTargetProperty()
        {
            Vector2 vec = (Vector2)m_targetProperty.GetValue(m_observedViewModel);
            m_transformToMove.position = new Vector3(vec.x, vec.y, transform.position.z);
        }
    }
}
