using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace SegoLib.PersistantDataManagement
{
    public abstract class SaveTranslator
    {
        public static Dictionary<Type, SaveTranslator> m_translators = new Dictionary<Type, SaveTranslator>();

        public static void RegisterSaveTranslator(SaveTranslator saveTranslator, Type typeOfObjectSaved)
        {
            m_translators[typeOfObjectSaved] = saveTranslator;
        }

        public static string ToJSON<T>(T obj)
        {
            Type t = typeof(T);

            if(m_translators.TryGetValue(t, out SaveTranslator translator) && translator is SaveTranslator<T> translatorT) {
                return translatorT.ToJSON(obj);
            }

            return JsonConvert.SerializeObject(obj);
        }

        public static T FromJSON<T>(string json)
        {
            Type t = typeof(T);

            if (m_translators.TryGetValue(t, out SaveTranslator translator) && translator is SaveTranslator<T> translatorT) {
                return translatorT.FromJSON(json);
            }

            return JsonConvert.DeserializeObject<T>(json);
        }
    } 

    public abstract class SaveTranslator<T> : SaveTranslator
    {
        public abstract string ToJSON(T obj);
        public abstract T FromJSON(string json);
    }


    #region Example Usage
#if UNITY_EDITOR

    // EXAMPLE USAGE//

    public class ObjectToSave
    {
        private class OTSSaveTranslator : SaveTranslator<ObjectToSave>
        {
            private class SaveData
            {
                public int myInt;
                public bool myBool;
                public string myString;

                public SaveData(ObjectToSave obj)
                {
                    myInt = obj.m_myInt;
                    myBool = obj.m_myBool;
                    myString = obj.myString;
                }
            }

            [RuntimeInitializeOnLoadMethod]
            private static void Register() => RegisterSaveTranslator(new OTSSaveTranslator(), typeof(ObjectToSave));

            public override ObjectToSave FromJSON(string json)
            {
                SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
                return new ObjectToSave() {
                    m_myInt = saveData.myInt,
                    m_myBool = saveData.myBool,
                    myString = saveData.myString
                };
            }

            public override string ToJSON(ObjectToSave obj)
            {
                SaveData saveData = new SaveData(obj);
                return JsonConvert.SerializeObject(saveData);
            }
        }

        public static ObjectToSave LoadFromString(string str)
        {
            return PersistantDataManagement.SaveTranslator.FromJSON<ObjectToSave>(str);
        }

        private int m_myInt = 11917;
        private bool m_myBool = false;
        private char m_myDataThatDoesntNeedToBeSaved = 'x';
        public string myString = "save me!";

        public string GetSaveString()
        {
            return PersistantDataManagement.SaveTranslator.ToJSON(this);
        }

        public char JustToAvoidWarning() => m_myDataThatDoesntNeedToBeSaved;
    }

#endif 
    #endregion
}
