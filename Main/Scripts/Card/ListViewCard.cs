using UnityEngine;
using System;

namespace Mahas.ListView
{
    public abstract class ListViewCard<TData> :  BaseListCard where TData : IListViewData
    {
        protected TData Data { get; private set; }
        
        //=========================================//
        // INTERNAL METHODS
        //=========================================//
        
        internal override void SetData(IListViewData viewData)
        {
            if (viewData is not TData cardData)
            {
                Debug.LogError($"RectListCard: Data type mismatch. Expected {typeof(TData)}, but got {viewData.GetType()}");
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