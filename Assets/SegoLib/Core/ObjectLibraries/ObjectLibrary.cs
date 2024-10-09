using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;

namespace SegoLib
{
    public abstract class ObjectLibrary
    {
        private static HashSet<ObjectLibrary> m_libraries = new HashSet<ObjectLibrary>();

        public ObjectLibrary() => m_libraries.Add(this);

        public bool Dispose() => m_libraries.Remove(this);
        public abstract void Clear();
    }


    public abstract class ObjectLibrary<TId, TObject> : ObjectLibrary, IEnumerable<TObject> where TObject : class, IIdentifiable<TId>
    { 
        /// <summary>
        /// The objects.
        /// </summary>
        protected Dictionary<TId, TObject> m_library = new Dictionary<TId, TObject>();
        public IReadOnlyCollection<TId> IdCollection => m_library.Keys;

        /// <summary>
        /// Gets an indexed list of all objects.
        /// </summary>
        public IReadOnlyList<TObject> ObjectList
        {
            get {
                if (m_arrayIsOutOfDate) UpdateObjectArray();
                return m_objectsArray;
            }
        }

        /// <summary>
        /// Stores all the objects as an array for purposes such as random retrieval.
        /// </summary>
        protected TObject[] m_objectsArray;

        /// <summary>
        /// Used to track whether or not the m_objectsArray is reflecting the most recent m_library information.
        /// </summary>
        protected bool m_arrayIsOutOfDate = true;

        /// <summary>
        /// The number of objects in this library.
        /// </summary>
        public int Count => m_library.Count;

        /// <summary>
        /// Raised when an object is removed from the library.
        /// </summary>
        public event Action<TObject> OnObjectRemoved;
        /// <summary>
        /// Raised when an object is added to the library.
        /// </summary>
        public event Action<TObject> OnObjectAdded;

        /// <summary>
        /// Get the object with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TObject this[TId id] => Get(id);

        /// <summary>
        /// The buffered Id for the next object added to the library that needs a new Id.
        /// </summary>
        protected TId m_nextId;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ObjectLibrary() : base() { }

        /// <summary>
        /// Adds an object to this library.
        /// </summary>
        /// <param name="obj">The object to add to the library.</param>
        /// <param name="replace">If an object already exists with the same Id, should this object replace it?</param>
        /// <returns>True if the object was successfully registered. False if not.</returns>
        public bool Register(TObject obj, bool replace = false)
        {
            if (obj == null) {
                Debug.LogError("Object must not be null.");
                return false;
            }

            //If the object doesn't have an id, give it one.
            if (IdIsEmpty(obj.Id)) {
                ApplyNewId(obj, m_nextId);
                UpdateNextId();
            }

            bool idAlreadyExists = m_library.ContainsKey(obj.Id);
            TObject oldObject = null;

            if (idAlreadyExists) {
                oldObject = m_library[obj.Id];
            }

            //Add the object if it doesn't already exist, or if it does and should be replaced.
            if (replace || !idAlreadyExists) {
                m_library[obj.Id] = obj;
                UpdateNextId();
                m_arrayIsOutOfDate = true;
                OnObjectAdded?.Invoke(obj);
                if (oldObject != null) {
                    OnObjectRemoved?.Invoke(oldObject);
                }
                return true;
            }

            Debug.LogWarning($"Object of type {obj.GetType().Name} already exists with an Id of {obj.Id}. Skipping.");

            return false;
        }

        /// <summary>
        /// Returns true if the object passed is the default id for this library.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract bool IdIsEmpty(TId id);

        /// <summary>
        /// The method to apply an id to the object.
        /// </summary>
        /// <param name="obj">The object that needs an id.</param>
        protected abstract void ApplyNewId(TObject obj, TId id);

        /// <summary>
        /// Should update the m_nextId to be the next available Id.
        /// </summary>
        protected abstract void UpdateNextId();

        /// <summary>
        /// Clears the library.
        /// </summary>
        public override void Clear()
        {
            if (m_arrayIsOutOfDate) {
                UpdateObjectArray();
            }

            m_library.Clear();

            for (int i = 0; i < m_objectsArray.Length; i++) {
                OnObjectRemoved?.Invoke(m_objectsArray[i]);
            }

            m_objectsArray = null;
            m_arrayIsOutOfDate = true;
        }

        /// <summary>
        /// Updates the object array with the most recent data.
        /// </summary>
        protected void UpdateObjectArray()
        {
            m_objectsArray = m_library.Values.ToArray();
            m_arrayIsOutOfDate = false;
        }

        /// <summary>
        /// Returns true if the library contains an object with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Contains(TId id)
        {
            return m_library.ContainsKey(id);
        }

        /// <summary>
        /// Returns true if the library contains the given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Contains(TObject obj)
        {
            return m_library.Values.Contains(obj);
        }

        /// <summary>
        /// Gets all objects where the given predicate evaluates to true.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<TObject> GetWhere(Func<TObject, bool> predicate)
        {
            return m_library.Values.Where(predicate);
        }

        /// <summary>
        /// Get an object from the library.
        /// </summary>
        /// <param name="id">The id of the desired object.</param>
        /// <returns></returns>
        public TObject Get(TId id)
        {
            return m_library[id];
        }

        /// <summary>
        /// Attempts to get an object from the library, returning true if found, false if not.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGet(TId id, out TObject value)
        {
            return m_library.TryGetValue(id, out value);
        }

        /// <summary>
        /// Removes an object from the library.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the object was found and removed. False if it was not found in the library.</returns>
        public bool Remove(TId id)
        {
            if (m_library.TryGetValue(id, out TObject obj)) {
                m_library.Remove(id);
                m_arrayIsOutOfDate = true;
                OnObjectRemoved?.Invoke(obj);
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Gets a string representing all the objects in this library.
        /// </summary>
        /// <returns></returns>
        public string GetObjectLog()
        {
            StringBuilder sb = StringBuilderPool.Borrow();
            
            foreach(var kvp in m_library) {
                sb.Append("{ Key: ");
                sb.Append(kvp.Key.ToString());
                sb.Append(", Value: ");
                sb.Append(kvp.Value.ToString());
                sb.Append(" }\n");
            }

            string s = sb.ToString();
            StringBuilderPool.Return(sb);

            return s;
        }

        #region IEnumerable
        public IEnumerator<TObject> GetEnumerator()
        {
            return m_library.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_library.Values.GetEnumerator();
        }
        #endregion IEnumerable
    }






}
