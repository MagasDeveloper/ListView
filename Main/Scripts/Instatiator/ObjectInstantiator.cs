using UnityEngine;

namespace Mahas.ListView
{
    public class ObjectInstantiator : IObjectInstantiator
    {
        
        //=========================================//
        // PUBLIC METHODS
        //=========================================//
        
        public TInstance Instantiate<TInstance>(Object prefab, Transform parent) where TInstance : Object
        {
            return Object.Instantiate(prefab, parent) as TInstance;
        }
        
        //=========================================//
        // PRIVATE METHODS
        //=========================================//
        
    }
}