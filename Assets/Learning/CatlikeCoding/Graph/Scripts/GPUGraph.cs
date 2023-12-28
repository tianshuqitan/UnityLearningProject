using System;
using UnityEngine;

namespace Learning.Graph.Scripts
{
    public class GPUGraph : MonoBehaviour
    {
        private enum TransitionMode
        {
            Cycle,
            Random,
        }

        private const int kMaxResolution = 1000;

        static readonly int POSITIONS_ID = Shader.PropertyToID("_Positions");
        static readonly int RESOLUTION_Id = Shader.PropertyToID("_Resolution");
        static readonly int STEP_ID = Shader.PropertyToID("_Step");
        static readonly int TIME_ID = Shader.PropertyToID("_Time");
        static readonly int TRANSITIONPROGRESS_ID = Shader.PropertyToID("_TransitionProgress");
        
        [SerializeField, Range(10, kMaxResolution)] private int m_Resolution = 10;

        [SerializeField] private FunctionLibrary.FunctionName m_FunctionName = 0; // 当前执行的函数名称

        [SerializeField, Min(0f)] private float m_FunctionDuration = 1f; // 函数执行时间
        [SerializeField, Min(0f)] private float m_TransitionDuration = 1f; // 函数转换到另一个的转换时间

        [SerializeField] private TransitionMode m_TransitionMode; // 函数切换模式

        private float m_Duration;

        private bool m_Transitioning; // 是否在转换中
        private FunctionLibrary.FunctionName m_TransitionFunctionName; // 上一个的执行函数名称(from)

        // 无法在热重载的时候保存, 如果在 play 模式下更改代码, 信息会丢失
        private ComputeBuffer m_PositionBuffer;

        [SerializeField] private ComputeShader m_ComputeShader;
        [SerializeField] private Material material;
        [SerializeField] private Mesh mesh;

        private void Awake()
        {
        }

        private void OnEnable()
        {
            // Vector3(x,y,z) float 3*4
            m_PositionBuffer = new ComputeBuffer(kMaxResolution * kMaxResolution, 3 * 4);
        }

        private void OnDisable()
        {
            m_PositionBuffer.Release();
            m_PositionBuffer = null;
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

            UpdateFunctionOnGPU();
        }

        private void PickNextFunction()
        {
            m_FunctionName = m_TransitionMode == TransitionMode.Cycle
                ? FunctionLibrary.GetNextFunctionName(m_FunctionName)
                : FunctionLibrary.GetRandomFunctionName(m_FunctionName);
        }

        private void UpdateFunctionOnGPU()
        {
            var step = 2f / m_Resolution;
            m_ComputeShader.SetInt(RESOLUTION_Id, m_Resolution);
            m_ComputeShader.SetFloat(STEP_ID, step);
            m_ComputeShader.SetFloat(TIME_ID, Time.time);
            if (m_Transitioning)
            {
                m_ComputeShader.SetFloat(TRANSITIONPROGRESS_ID, Mathf.SmoothStep(0f, 1f, m_Duration/m_TransitionDuration));
            }
            
            var kernelIndex = (int)m_FunctionName + (int)(m_Transitioning ? m_TransitionFunctionName : m_FunctionName) * FunctionLibrary.FunctionCount;
            m_ComputeShader.SetBuffer(kernelIndex, POSITIONS_ID, m_PositionBuffer);

            var groups = Mathf.CeilToInt(m_Resolution / 8f);
            m_ComputeShader.Dispatch(kernelIndex, groups, groups, 1);
            
            material.SetBuffer(POSITIONS_ID, m_PositionBuffer);
            material.SetFloat(STEP_ID, step);
            
            var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / m_Resolution));
            Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, m_Resolution * m_Resolution);
        }
    }
}