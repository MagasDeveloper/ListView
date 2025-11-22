using UnityEngine;

namespace Mahas.ListView
{
    public interface IObjectInstantiator
    {
        TInstance Instantiate<TInstance>(Object prefab, Transform parent) where TInstance : Object;
    }
}