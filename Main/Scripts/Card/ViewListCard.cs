using UnityEngine;
using System;

namespace Mahas.ListView
{
    public abstract class ViewListCard<TData> :  BaseListCard where TData : IListData
    {
        protected TData Data { get; private set; }
        
        //=========================================//
        // INTERNAL METHODS
        //=========================================//
        
        internal override void SetData(IListData data)
        {
            if (data is not TData cardData)
            {
                Debug.LogError($"RectListCard: Data type mismatch. Expected {typeof(TData)}, but got {data.GetType()}");
                return;
            }
            Data = cardData;
        }

        internal override Type GetDataType()
        {
            return typeof(TData);
        }
        
        //=========================================//
        // PRIVATE METHODS
        //=========================================//
        
        
        
    }
}