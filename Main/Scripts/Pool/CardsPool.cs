using UnityEngine.Pool;
using UnityEngine;

namespace Mahas.ListView
{
    public class CardsPool
    {
        private readonly ObjectPool<BaseListCard> _pool;
        private readonly ICardInstantiator _instantiator;
        private readonly BaseListCard _prefab;
        private readonly Transform _content;

        public CardsPool(ICardInstantiator instantiator, PrefabListVariant variant, Transform content, int defaultCapacity)
        {
            _instantiator = instantiator;
            _content = content;
            _prefab = variant.Prefab;

            _pool = new ObjectPool<BaseListCard>(
                createFunc: GetCardInstance,
                actionOnGet: OnCardGet,
                actionOnRelease: OnCardRelease,
                actionOnDestroy: OnCardDestroy,
                collectionCheck: false,
                defaultCapacity: defaultCapacity);

            for (int i = 0; i < variant.InitialPoolSize; i++)
            {
                var card = GetCardInstance();
                _pool.Release(card);
            }
            
        }
        
        public BaseListCard Get()
        {
            return _pool.Get();
        }

        public void Release(BaseListCard card)
        {
            _pool.Release(card);
        }

        private void OnCardDestroy(BaseListCard card)
        {
            Object.Destroy(card.gameObject);
        }

        private void OnCardRelease(BaseListCard card)
        {
            card.InvokeRecycle();
            card.gameObject.SetActive(false);
        }

        private void OnCardGet(BaseListCard card)
        {
            card.gameObject.SetActive(true);
        }

        private BaseListCard GetCardInstance()
        {
            var instance =  _instantiator.Instantiate<BaseListCard>(_prefab, _content);
            instance.SetAsNew();
            return instance;
        }

        
        
    }
}