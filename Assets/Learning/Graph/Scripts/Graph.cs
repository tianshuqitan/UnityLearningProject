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
            var scale = Vector3.one * step;

            m_Points = new Transform[m_Resolution * m_Resolution];
            for (int i = 0, x = 0, z = 0; i < m_Points.Length; i++, x++)
            {
                var point = m_Points[i] = Instantiate(m_PointPrefab);
                point.localScale = scale;
                point.SetParent(transform, false);
            }
        }
        
        // Update is called once per frame
        private void Update()
        {
            var f = FunctionLibrary.GetFunction(m_FunctionName);
            var time = Time.time;
            var step = 2f / m_Resolution;
            var v = 0.5f * step - 1f;
            for (int i = 0, x = 0, z = 0; i < m_Points.Length; i++, x++) {
                if (x == m_Resolution) {
                    x = 0;
                    z += 1;
                    v = (z + 0.5f) * step - 1f;
                }
                var u = (x + 0.5f) * step - 1f;
                m_Points[i].localPosition = f(u, v, time);
            }
        }
    }
}
