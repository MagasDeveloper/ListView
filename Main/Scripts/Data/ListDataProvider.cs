using System.Collections.Generic;

namespace Mahas.ListView
{
    public class ListDataProvider
    {
        private readonly Dictionary<IListViewData, int> _indexedData = new();
        private readonly List<IListViewData> _items = new();

        
        public IReadOnlyList<IListViewData> Items => _items;
        
        //=========================================//
        // INTERNAL METHODS
        //=========================================//
        
        internal void SetupData(IEnumerable<IListViewData> data)
        {
            _items.Clear();
            _items.AddRange(data);
            
            _indexedData.Clear();
            for (int i = 0; i < _items.Count; i++)
            {
                _indexedData[_items[i]] = i;
            }
        }
        
        internal void Remove(IListViewData viewData)
        {
            _items.Remove(viewData);
            _indexedData.Remove(viewData);
        }
        
        internal void Clear()
        {
            _items.Clear();
            _indexedData.Clear();
        }
        
        internal int GetDataIndex(IListViewData viewData)
        {
            return _indexedData.GetValueOrDefault(viewData, -1);
        }
        
        //=========================================//
        // PROTECTED METHODS
        //=========================================//
        
        //=========================================//
        // PRIVATE METHODS
        //=========================================//
        
    }
}