using TMPro;
using UnityEngine;

namespace Learning.FPS
{
    public class FrameRateCounter : MonoBehaviour
    {
        private enum DisplayMode
        {
            FPS,
            MS
        }

        [SerializeField] DisplayMode displayMode = DisplayMode.FPS;
        [SerializeField] private TextMeshProUGUI display;

        private int frames;
        private float duration;
        private float bestDuration = float.MaxValue;
        private float worstDuration = 0f;

        [SerializeField, Range(0.1f, 2f)] private float sampleDuration = 1f;


        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            var frameDuration = Time.unscaledDeltaTime;
            frames += 1;
            duration += frameDuration;

            if (frameDuration < bestDuration)
            {
                bestDuration = frameDuration;
            }

            if (frameDuration > worstDuration)
            {
                worstDuration = frameDuration;
            }

            if (duration >= sampleDuration)
            {
                if (displayMode == DisplayMode.FPS)
                {
                    display.SetText("FPS: AVG {0:0} BEST {1:0} WORSE {2:0}", frames / duration, 1f / bestDuration, 1f / worstDuration);
                }
                else
                {
                    display.SetText("MS: AVG {0:1} BEST {1:1} WORSE {2:1}", 1000f * duration / frames, 1000f * bestDuration,
                        1000f * worstDuration);
                }
                
                frames = 0;
                duration = 0f;
                bestDuration = float.MaxValue;
                worstDuration = 0f;
            }
        }
    }
}