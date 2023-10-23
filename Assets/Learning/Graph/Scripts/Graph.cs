using System;
using UnityEngine;

namespace Learning.Graph.Scripts
{
    public class Graph : MonoBehaviour
    {
        private enum TransitionMode
        {
            Cycle,
            Random,
        }

        [SerializeField] private Transform m_PointPrefab;

        [SerializeField, Range(10, 200)] private int m_Resolution = 10;

        [SerializeField] private FunctionLibrary.FunctionName m_FunctionName = 0; // 当前执行的函数名称

        [SerializeField, Min(0f)] private float m_FunctionDuration = 1f; // 函数执行时间
        [SerializeField, Min(0f)] private float m_TransitionDuration = 1f; // 函数转换到另一个的转换时间
        
        [SerializeField] private TransitionMode m_TransitionMode; // 函数切换模式
        
        private Transform[] m_Points;
        private float m_Duration;
        
        private bool m_Transitioning; // 是否在转换中
        private FunctionLibrary.FunctionName m_TransitionFunctionName; // 上一个的执行函数名称(from)

        private void Awake()
        {
            // 总的波长为 2f
            var step = 2f / m_Resolution;
            var scale = Vector3.one * step;
            
            m_Points = new Transform[m_Resolution * m_Resolution];
            for (var i = 0; i < m_Points.Length; i++)
            {
                var point = m_Points[i] = Instantiate(m_PointPrefab);
                point.localScale = scale;
                point.SetParent(transform, false);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            m_Duration += Time.deltaTime;

            if (m_Transitioning)
            {
                if (m_Duration >= m_TransitionDuration)
                {
                    m_Duration -= m_TransitionDuration;
                    m_Transitioning = false;
                }
            }
            else if (m_Duration >= m_FunctionDuration)
            {
                m_Duration -= m_FunctionDuration;
                m_Transitioning = true;
                m_TransitionFunctionName = m_FunctionName;
                PickNextFunction();
            }

            if (m_Transitioning)
            {
                UpdateFunctionTransition();
            }
            else
            {
                UpdateFunction();
            }
        }

        private void UpdateFunction()
        {
            var f = FunctionLibrary.GetFunction(m_FunctionName);
            var time = Time.time;
            var step = 2f / m_Resolution;
            var v = 0.5f * step - 1f;
            for (int i = 0, x = 0, z = 0; i < m_Points.Length; i++, x++)
            {
                if (x == m_Resolution)
                {
                    x = 0;
                    z += 1;
                    v = (z + 0.5f) * step - 1f;
                }

                var u = (x + 0.5f) * step - 1f;
                m_Points[i].localPosition = f(u, v, time);
            }
        }

        private void PickNextFunction()
        {
            m_FunctionName = m_TransitionMode == TransitionMode.Cycle
                ? FunctionLibrary.GetNextFunctionName(m_FunctionName)
                : FunctionLibrary.GetRandomFunctionName(m_FunctionName);
        }

        private void UpdateFunctionTransition()
        {
            var from = FunctionLibrary.GetFunction(m_TransitionFunctionName);
            var to = FunctionLibrary.GetFunction(m_FunctionName);
            var progress = m_Duration / m_TransitionDuration;
            var time = Time.time;
            var step = 2f / m_Resolution;
            var v = 0.5f * step - 1f;
            for (int i = 0, x = 0, z = 0; i < m_Points.Length; i++, x++)
            {
                if (x == m_Resolution)
                {
                    x = 0;
                    z += 1;
                    v = (z + 0.5f) * step - 1f;
                }

                var u = (x + 0.5f) * step - 1f;
                m_Points[i].localPosition = FunctionLibrary.Morph(
                    u, v, time, from, to, progress
                );
            }
        }
    }
}
