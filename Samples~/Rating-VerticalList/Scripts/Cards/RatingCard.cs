using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Mahas.ListView.Samples
{
    public class RatingCard : ListViewCard<RatingUserData>
    {
        [SerializeField] private TextMeshProUGUI _positionText; 
        [SerializeField] private TextMeshProUGUI _nicknameText;
        [SerializeField] private TextMeshProUGUI _ratingText;
        [SerializeField] private Image _avatarImage;

        protected override void OnCreate()
        {
            _avatarImage.color = Color.white;
        }
        
        protected override void OnSpawn()
        {
            _positionText.text = $"{Data.Position}.";
            _nicknameText.text = Data.User.Nickname;
            _ratingText.text = Data.Rating.ToString("N0");
            _avatarImage.sprite = Data.User.Avatar;
        }

        protected override async Task ProcessRecycle(CancellationToken cancellationToken)
        {
            await Task.Delay(1 * 1000, cancellationToken);
        }
    }
}