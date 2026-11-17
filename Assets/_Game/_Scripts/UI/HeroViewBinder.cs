using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// Binds every Hero created by <see cref="HeroFactory"/> to its view components, cleaning them up when the hero is retired.
    /// </summary>
    public class HeroViewBinder : MonoBehaviour
    {
        [SerializeField] private HeroFactory m_heroFactory;
        [SerializeField] private CinemachineTargetGroup m_targetGroup;
        [SerializeField] private VM_Hero m_sidebarEntryPrefab;
        [SerializeField] private Transform m_sidebarContent;

        private readonly Dictionary<Hero, VM_Hero> m_sidebarEntries = new();

        private void OnEnable()
        {
            if (m_heroFactory == null) {
                Debug.LogError($"{nameof(HeroViewBinder)} has no {nameof(HeroFactory)} assigned.", this);
                return;
            }

            m_heroFactory.HeroCreated += OnHeroCreated;
            m_heroFactory.HeroRetired += OnHeroRetired;
        }

        private void OnDisable()
        {
            if (m_heroFactory != null) {
                m_heroFactory.HeroCreated -= OnHeroCreated;
                m_heroFactory.HeroRetired -= OnHeroRetired;
            }
        }

        private void OnDestroy()
        {
            m_sidebarEntries.Clear();
            m_heroFactory = null;
            m_targetGroup = null;
            m_sidebarEntryPrefab = null;
            m_sidebarContent = null;
        }

        private void OnHeroCreated(HeroFactory sender, Hero hero)
        {
            m_targetGroup.AddMember(hero.transform, 1.0f, 2.0f);

            VM_Hero sidebarEntry = Instantiate(m_sidebarEntryPrefab, m_sidebarContent);
            sidebarEntry.SetModel(hero);
            m_sidebarEntries.Add(hero, sidebarEntry);
        }

        private void OnHeroRetired(HeroFactory sender, Hero hero)
        {
            if (m_targetGroup != null)
                m_targetGroup.RemoveMember(hero.transform);

            if (m_sidebarEntries.Remove(hero, out VM_Hero sidebarEntry) && sidebarEntry != null)
                Destroy(sidebarEntry.gameObject);
        }
    }
}
