using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SegoLib.PersistantDataManagement
{
    [System.Serializable]
    public abstract class SaveContainer
    {
        [JsonIgnore]
        public abstract System.Type SavedType { get; }
    }

    [System.Serializable]
    public abstract class SaveContainer<T> : SaveContainer
    {
        [JsonIgnore]
        public override System.Type SavedType => typeof(T);

        public abstract T Unbox();

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
