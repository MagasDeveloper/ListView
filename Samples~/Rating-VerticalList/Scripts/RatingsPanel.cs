using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Mahas.ListView.Samples
{
    public class RatingsPanel : MonoBehaviour
    {
        [Header("List View")]
        [SerializeField] private ListView _listView;
        
        [Header("Test Data")]
        [SerializeField] private UserData[] _randomUsers;

        private List<RatingUserData> _ratingsData = new();
        
        private IEnumerator Start()
        {
            InitializeData(20);
            yield return new WaitForSeconds(2f);
            foreach (var ratingData in _ratingsData)
            {
                _listView.AddData(ratingData);
                yield return new WaitForSeconds(0.2f);
            }
            
            yield return new WaitForSeconds(1.6f);
            
            for(int i = _ratingsData.Count - 1; i >= 0; i--)
            {
                _listView.RemoveData(_ratingsData[i]);
                yield return new WaitForSeconds(0.2f);
            }
        }

        private void InitializeData(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var randomUser = _randomUsers[Random.Range(0, _randomUsers.Length)];
                var rating = Random.Range(1, 2_750_000);
                var ratingData = new RatingUserData(randomUser, rating);
                _ratingsData.Add(ratingData);
            }
            
            _ratingsData = _ratingsData.OrderByDescending(r => r.Rating).ToList();
            
            for (int i = 0; i < _ratingsData.Count; i++)
            {
                _ratingsData[i].SetPosition(i + 1);
            }
        }

    }
}