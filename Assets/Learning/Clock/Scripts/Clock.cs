using System;
using UnityEngine;

namespace Learning.Clock.Scripts
{
    public class Clock : MonoBehaviour
    {
        private const float HoursToDegrees = -30f, MinutesToDegrees = -6f, SecondsToDegrees = -6f;

        [SerializeField] private Transform m_HoursPivot, m_MinutesPivot, m_SecondsPivot;

        private void Awake()
        {
            m_HoursPivot.localRotation = Quaternion.Euler(0f, 0f, HoursToDegrees * DateTime.Now.Hour);
            m_MinutesPivot.localRotation = Quaternion.Euler(0f, 0f, MinutesToDegrees * DateTime.Now.Minute);
            m_SecondsPivot.localRotation = Quaternion.Euler(0f, 0f, SecondsToDegrees * DateTime.Now.Minute);
        }
        
        // Update is called once per frame
        private void Update()
        {
            // 一秒一秒跳动
            // var time = DateTime.Now;
            // m_HoursPivot.localRotation = Quaternion.Euler(0f, 0f, hoursToDegrees * time.Hour);
            // m_MinutesPivot.localRotation = Quaternion.Euler(0f, 0f, minutesToDegrees * time.Minute);
            // m_SecondsPivot.localRotation = Quaternion.Euler(0f, 0f, secondsToDegrees * time.Second);
            
            // 连续移动
            var time = DateTime.Now.TimeOfDay;
            m_HoursPivot.localRotation = Quaternion.Euler(0f, 0f, HoursToDegrees * (float)time.TotalHours);
            m_MinutesPivot.localRotation = Quaternion.Euler(0f, 0f, MinutesToDegrees * (float)time.TotalMinutes);
            m_SecondsPivot.localRotation = Quaternion.Euler(0f, 0f, SecondsToDegrees * (float)time.TotalSeconds);
        }
    }
}
