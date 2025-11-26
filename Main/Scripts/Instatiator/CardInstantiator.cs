using UnityEngine;

namespace Mahas.ListView
{
    public class CardInstantiator : ICardInstantiator
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