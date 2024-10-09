

namespace SegoLib.PersistantDataManagement
{
    public interface ISavable
    {

    }

    public interface ISavable<TSaveContainer, TSavedType> : ISavable where TSaveContainer : SaveContainer<TSavedType>
    {
        public TSaveContainer GetSaveContainer();
    }
}
