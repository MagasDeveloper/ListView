using System.Collections.Generic;
using UnityEngine;
using System;

namespace Mahas.ListView
{
    public class ObjectPoolMap
    {
        private readonly Dictionary<Type, CardsPool> _poolsMap = new();
        private readonly Dictionary<Type, BaseListCard> _prefabsMap = new();
        
        internal ObjectPoolMap(PrefabListVariant[] variants, ICardInstantiator instantiator, Transform content, int defaultCapacity)
        {
            foreach (var variant in variants)
            {
                Type type = variant.Prefab.GetDataType();
                CardsPool pool = new (instantiator, variant, content, defaultCapacity);
                _prefabsMap.Add(type, variant.Prefab);
                _poolsMap.Add(type, pool);
            }
        }

        internal Vector2 GetPrefabSize(Type dataType)
        {
            if (_prefabsMap.TryGetValue(dataType, out var prefab))
            {
                return prefab.Size;
            }
            
            Debug.LogError($"ObjectsPoolMap: No prefab found for type {dataType}");
            return default;
        }

        internal BaseListCard Get(Type dataType)
        {
            if (_poolsMap.TryGetValue(dataType, out var pool))
            {
                return pool.Get();
            }
            
            Debug.LogError($"ObjectsPoolMap: No pool found for type {dataType}");
            return null;
        }

        internal void Release(BaseListCard card)
        {
            var dataType = card.GetDataType();
            if (_poolsMap.TryGetValue(dataType, out var pool))
            {
                pool.Release(card);
                return;
            }
            
            Debug.LogError($"ObjectsPoolMap: No pool found for type {dataType}");
        }
        
    }
}