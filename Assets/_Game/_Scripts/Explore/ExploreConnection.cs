using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ExploreConnection : MonoBehaviour
    {
        [field: SerializeField] public ExploreNode ConnectedNode { get; private set; }
    } 
}
