using UnityEngine;
using System;

namespace Mahas.ListView
{
    [Serializable]
    public struct ContentSettings
    {
        public ContentDirectionType Direction;
        public RectOffset Paddings;
        public float Spacing;
    }
}