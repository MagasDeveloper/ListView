using UnityEngine;

namespace Mahas.ListView
{
    public interface ICardInstantiator
    {
        TInstance Instantiate<TInstance>(Object prefab, Transform parent) where TInstance : Object;
    }
}