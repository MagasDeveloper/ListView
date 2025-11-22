namespace Mahas.ListView
{
    public class ViewListElement
    {
        internal BaseListCard Card { get; private set; }
        internal IListData Data { get; private set; }
        
        public int Index { get; private set; }
        
        //=========================================//
        // INTERNAL METHODS
        //=========================================//
        
        internal void Initialize(BaseListCard card, IListData data, int index)
        {
            Card = card;
            Data = data;
            Index = index;
        }
        
        //=========================================//
        // PUBLIC METHODS
        //=========================================//
        
        /// <summary>
        /// Retrieves the data associated with this ListElement, cast to the specified type <typeparamref name="TData"/>.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public TData GetData<TData>() where TData : IListData
        {
            if (Data is not TData typedData)
            {
                UnityEngine.Debug.LogError($"ListElement: Data type mismatch. Expected {typeof(TData)}, but got {Data.GetType()}");
                return default;
            }
            return typedData;
        }
        
        /// <summary>
        /// Retrieves the card associated with this ListElement, cast to the specified type <typeparamref name="TCard"/>.
        /// </summary>
        /// <typeparam name="TCard"></typeparam>
        /// <returns></returns>
        public TCard GetCard<TCard>() where TCard : BaseListCard
        {
            if (Card is not TCard typedCard)
            {
                UnityEngine.Debug.LogError($"ListElement: Card type mismatch. Expected {typeof(TCard)}, but got {Card.GetType()}");
                return default;
            }
            return typedCard;
        }
        
        /// <summary>
        /// Attempts to retrieve the card associated with this ListElement cast to <typeparamref name="TCard"/>.
        /// </summary>
        /// <typeparam name="TCard">Target card type.</typeparam>
        /// <param name="result">When this method returns, contains the cast card if successful; otherwise the default value for <typeparamref name="TCard"/>.</param>
        /// <returns>True if the card is of type <typeparamref name="TCard"/>; otherwise false.</returns>
        public bool TryGetCard<TCard>(out TCard result) where TCard : BaseListCard
        {
            result = default;
            if (Card is TCard typedCard)
            {
                result = typedCard;
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Attempts to retrieve the data associated with this ListElement cast to <typeparamref name="TData"/>.
        /// </summary>
        /// <typeparam name="TData">Target data type.</typeparam>
        /// <param name="result">On return, contains the cast data if successful; otherwise the default value for <typeparamref name="TData"/>.</param>
        /// <returns>True if the data is of type <typeparamref name="TData"/>; otherwise false.</returns>
        public bool TryGetData<TData>(out TData result) where TData : IListData
        {
            result = default;
            if (Data is TData typedData)
            {
                result = typedData;
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Checks if the current <see cref="Data"/> instance is of type <typeparamref name="TData"/>.
        /// </summary>
        /// <typeparam name="TData">Target data type implementing <see cref="IListData"/>.</typeparam>
        /// <returns>True if <see cref="Data"/> is of type <typeparamref name="TData"/>; otherwise false.</returns>
        public bool IsDataType<TData>() where TData : IListData
        {
            return Data is TData;
        }
        
        /// <summary>
        /// Checks if the current <see cref="Card"/> instance is of type <typeparamref name="TCard"/>.
        /// </summary>
        /// <typeparam name="TCard">Target card type implementing <see cref="BaseListCard"/>.</typeparam>
        /// <returns>True if <see cref="Card"/> is of type <typeparamref name="TCard"/>; otherwise false.</returns>
        public bool IsCardType<TCard>() where TCard : BaseListCard
        {
            return Card is TCard;
        }
        
        //=========================================//
        // PRIVATE METHODS
        //=========================================//
        
    }
}