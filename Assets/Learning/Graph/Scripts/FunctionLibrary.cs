using UnityEngine;
using static UnityEngine.Mathf;

namespace Learning.Graph.Scripts
{
    public static class FunctionLibrary
    {
        public delegate Vector3 Function(float u, float v, float t);

        private static readonly Function[] Functions =
        {
            Wave, MultiWave, Ripple,
            Sphere, Sphere1, Sphere2, Sphere3,
            Torus, Torus1, Torus2, Torus3
        };

        public enum FunctionName
        {
            Wave,
            MultiWave,
            Ripple,
            Sphere,
            Sphere1,
            Sphere2,
            Sphere3,
            Torus,
            Torus1,
            Torus2,
            Torus3,
        }

        public static Function GetFunction(FunctionName name)
        {
            return Functions[(int)name];
        }

        private static Vector3 Wave(float u, float v, float t)
        {
            Vector3 p;
            p.x = u;
            p.y = Sin(PI * (u + v + t));
            p.z = v;
            return p;
        }

        private static Vector3 MultiWave(float u, float v, float t)
        {
            Vector3 p;
            p.x = u;
            p.y = Sin(PI * (u + 0.5f * t));
            p.y += 0.5f * Sin(2f * PI * (v + t));
            p.y += Sin(PI * (u + v + 0.25f * t));
            p.y *= (1f / 2.5f);
            p.z = v;
            return p;
        }

        private static Vector3 Ripple(float u, float v, float t)
        {
            var d = Sqrt(u * u + v * v);
            Vector3 p;
            p.x = u;
            p.y = Sin(PI * (4f * d - t));
            p.y /= (1f + 10f * d);
            p.z = v;
            return p;
        }

        private static Vector3 Sphere(float u, float v, float t)
        {
            var r = 0.5f + 0.5f * Sin(PI * t);
            var s = r * Cos(0.5f * PI * v);
            Vector3 p;
            p.x = s * Sin(PI * u);
            p.y = r * Sin(PI * 0.5f * v);
            p.z = s * Cos(PI * u);
            return p;
        }

        private static Vector3 Sphere1(float u, float v, float t)
        {
            var r = 0.9f + 0.1f * Sin(8f * PI * u);
            var s = r * Cos(0.5f * PI * v);
            Vector3 p;
            p.x = s * Sin(PI * u);
            p.y = r * Sin(PI * 0.5f * v);
            p.z = s * Cos(PI * u);
            return p;
        }

        private static Vector3 Sphere2(float u, float v, float t)
        {
            var r = 0.9f + 0.1f * Sin(8f * PI * v);
            var s = r * Cos(0.5f * PI * v);
            Vector3 p;
            p.x = s * Sin(PI * u);
            p.y = r * Sin(PI * 0.5f * v);
            p.z = s * Cos(PI * u);
            return p;
        }

        private static Vector3 Sphere3(float u, float v, float t)
        {
            var r = 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t));
            var s = r * Cos(0.5f * PI * v);
            Vector3 p;
            p.x = s * Sin(PI * u);
            p.y = r * Sin(PI * 0.5f * v);
            p.z = s * Cos(PI * u);
            return p;
        }

        private static Vector3 Torus(float u, float v, float t)
        {
            var r = 1f;
            var s = 0.5f + r * Cos(0.5f * PI * v);
            Vector3 p;
            p.x = s * Sin(PI * u);
            p.y = r * Sin(PI * 0.5f * v);
            p.z = s * Cos(PI * u);
            return p;
        }

        private static Vector3 Torus1(float u, float v, float t)
        {
            var r = 1f;
            var s = 0.5f + r * Cos(PI * v);
            Vector3 p;
            p.x = s * Sin(PI * u);
            p.y = r * Sin(PI * v);
            p.z = s * Cos(PI * u);
            return p;
        }

        private static Vector3 Torus2(float u, float v, float t)
        {
            var r1 = 0.75f;
            var r2 = 0.25f;
            var s = r1 + r2 * Cos(PI * v);
            Vector3 p;
            p.x = s * Sin(PI * u);
            p.y = r2 * Sin(PI * v);
            p.z = s * Cos(PI * u);
            return p;
        }

        private static Vector3 Torus3(float u, float v, float t)
        {
            var r1 = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t));
            var r2 = 0.15f + 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t));
            var s = r1 + r2 * Cos(PI * v);
            Vector3 p;
            p.x = s * Sin(PI * u);
            p.y = r2 * Sin(PI * v);
            p.z = s * Cos(PI * u);
            return p;
        }
    }
}