using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SegoLib
{
    /// <summary>
    /// This class provides a way to offer ordered initialization.
    /// </summary>
    public class SceneInitializer : MonoBehaviour
    {
        public bool IsInitialized { get; private set; }
        public bool IsInitializing { get; private set; }
        private bool m_initializablesUpdated = false;

        private List<IInitializable> m_initializables = new List<IInitializable>();

        public void Initialize()
        {
            //If we aren't already initializing, update the initializables collection.
            if (!IsInitializing) {
                UpdateInitializables();
            }

            //We are now initializing.
            IsInitializing = true;

            //Loop through the initializables from highest to lowest priority.
            for (int i = m_initializables.Count - 1; i >= 0; i--) {
                IInitializable initializable = m_initializables[i];

                //Remove the object from the collection and initialize it.
                m_initializables.RemoveAt(i);
                initializable?.Initialize();

                //If the collection got updated mid loop, break.
                //This could happen if part of the object's initialization involves registering more IInitializables.
                if (m_initializablesUpdated) {
                    break;
                }
            }

            //If the collection was updated during initialization, restart the initialization.
            if (m_initializablesUpdated) {
                m_initializablesUpdated = false;
                Initialize();
                return;
            }

            //We are no longer initializing.
            IsInitializing = false;

            m_initializables.Clear();

            //We are now initialized.
            IsInitialized = true;
        }

        /// <summary>
        /// Register an initializable with this initializer.
        /// </summary>
        /// <param name="initializable"></param>
        public void Register(IInitializable initializable)
        {
            //If this has already been initialized, then init the new initializable immediately.
            if (IsInitialized) {
                initializable.Initialize();
                return;
            }

            bool added = false;
            for (int i = 0; i < m_initializables.Count; i++) {
                if (initializable.InitPriority < m_initializables[i].InitPriority) {
                    m_initializables.Insert(i, initializable);
                    added = true;
                    break;
                }
            }

            if (!added) m_initializables.Add(initializable);

            if (IsInitializing) {
                m_initializablesUpdated = true;
            }
        }

        /// <summary>
        /// Gather all objects in the scene that are initializable.
        /// </summary>
        private void UpdateInitializables()
        {
            IEnumerable<Component> foundInitializables = FindObjectsOfType<Component>().Where(c => c is IInitializable);

            foreach (Component c in foundInitializables) {
                Register((IInitializable)c);
            }
        }
    }
}
