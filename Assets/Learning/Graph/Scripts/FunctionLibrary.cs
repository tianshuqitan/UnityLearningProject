using UnityEngine;
using static UnityEngine.Mathf;

namespace Learning.Graph.Scripts
{
    public static class FunctionLibrary
    {
        public delegate float Function(float x, float z, float t);

        private static readonly Function[] Functions = { Wave, MultiWave, Ripple };

        public enum FunctionName
        {
            Wave,
            MultiWave,
            Ripple,
        }

        public static Function GetFunction(FunctionName name)
        {
            return Functions[(int)name];
        }

        private static float Wave(float x, float z, float t)
        {
            return Sin(PI * (x + t));
        }

        private static float MultiWave(float x, float z, float t)
        {
            var y = Sin(PI * (x + 0.5f * t)) + Sin(2f * PI * (x + t)) * 0.5f;
            return y * (2f / 3f);
        }

        private static float Ripple(float x, float z, float t)
        {
            var d = Abs(x);
            var y = Sin(PI * (4f * d - t));
            return y / (1f + 10f * d);
        }
    }
}