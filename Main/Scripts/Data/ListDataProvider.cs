using System.Collections.Generic;

namespace Mahas.ListView
{
    public class ListDataProvider
    {
        private readonly Dictionary<IListData, int> _indexedData = new();
        private readonly List<IListData> _data = new();

        
        public IReadOnlyList<IListData> Data => _data;
        
        //=========================================//
        // INTERNAL METHODS
        //=========================================//
        
        internal void SetupData(IEnumerable<IListData> data)
        {
            _data.Clear();
            _data.AddRange(data);
            
            _indexedData.Clear();
            for (int i = 0; i < _data.Count; i++)
            {
                _indexedData[_data[i]] = i;
            }
        }
        
        internal void Remove(IListData data)
        {
            _data.Remove(data);
        }
        
        internal void Clear()
        {
            _data.Clear();
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