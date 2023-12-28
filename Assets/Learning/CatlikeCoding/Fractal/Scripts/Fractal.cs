using System;
using UnityEngine;

namespace Learning.Fractal.Scripts
{
    public class Fractal : MonoBehaviour
    {
        [SerializeField, Range(1, 8)] private int m_Depth = 4;
        
        public int Depth
        {
            get => m_Depth;
            set => m_Depth = value;
        }

        private void Start()
        {
            var child = Instantiate(this);
            child.Depth = Depth - 1;
        }
    }
}
