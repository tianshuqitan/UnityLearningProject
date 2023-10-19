using System;
using UnityEngine;

namespace Learning.Graph.Scripts
{
    public class Graph : MonoBehaviour
    {
        [SerializeField] private Transform m_PointPrefab;

        [SerializeField, Range(10, 100)] private int m_Resolution = 10;

        [SerializeField] private FunctionLibrary.FunctionName m_FunctionName = 0;

        private Transform[] m_Points;

        private void Awake()
        {
            var step = 2f / m_Resolution;
            var position = Vector3.zero;
            var scale = Vector3.one * step;

            m_Points = new Transform[m_Resolution * m_Resolution];
            for (var i = 0; i < m_Points.Length; i++)
            {
                var point = m_Points[i] = Instantiate(m_PointPrefab);
                position.x = (i + 0.5f) * step - 1f;
                // position.y = position.x * position.x * position.x;
                point.localPosition = position;
                point.localScale = scale;
                point.SetParent(transform, false);
            }
        }
        
        // Update is called once per frame
        private void Update()
        {
            var f = FunctionLibrary.GetFunction(m_FunctionName);
            var time = Time.time;
            foreach (var point in m_Points)
            {
                var position = point.localPosition;
                position.y = f(position.x,  position.z, time);
                point.localPosition = position;
            }
        }
    }
}
