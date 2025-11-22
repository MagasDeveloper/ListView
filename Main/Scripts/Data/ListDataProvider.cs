using System.Collections.Generic;

namespace Mahas.ListView
{
    public class ListDataProvider
    {
        private readonly Dictionary<IListData, int> _indexedData = new();
        private readonly List<IListData> _items = new();

        
        public IReadOnlyList<IListData> Items => _items;
        
        //=========================================//
        // INTERNAL METHODS
        //=========================================//
        
        internal void SetupData(IEnumerable<IListData> data)
        {
            _items.Clear();
            _items.AddRange(data);
            
            _indexedData.Clear();
            for (int i = 0; i < _items.Count; i++)
            {
                _indexedData[_items[i]] = i;
            }
        }
        
        internal void Remove(IListData data)
        {
            _items.Remove(data);
            _indexedData.Remove(data);
        }
        
        internal void Clear()
        {
            _items.Clear();
            _indexedData.Clear();
        }
        
        internal int GetDataIndex(IListData data)
        {
            return _indexedData.GetValueOrDefault(data, -1);
        }
        
        //=========================================//
        // PROTECTED METHODS
        //=========================================//
        
        //=========================================//
        // PRIVATE METHODS
        //=========================================//
        
    }
}