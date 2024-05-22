using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _Scripts.Utility
{
    public static class Extensions
    {
        public static Vector3 ClampX(this Vector3 source, float min, float max)
        {
            source.x = Mathf.Clamp(source.x, min, max);
            return source;
        }

        public static Vector3 OnlyX(this Vector3 source)
        {
            return new Vector3(source.x, 0f, 0f);
        }  
        
        public static Vector3 OnlyY(this Vector3 source)
        {
            return new Vector3(0f, source.y, 0f);
        }
        
        public static Vector3 OverrideY(this Vector3 source, float y)
        {
            return new Vector3(source.x, y, source.z);
        } 
        public static Vector3 ModifyY(this Vector3 source, float y)
        {
            return new Vector3(source.x, source.y + y, source.z);
        }

        public static string ToTime(this float seconds)
        {
            var timeSpan = TimeSpan.FromSeconds(seconds);
            return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }

        public static List<(float, string)> GetAllScores()
        {
            var scores = GetRawScores();

            return scores?.Split('|').Select(s =>
            {
                var record = s.Split('-');
                return (float.Parse(record[1]), record[0]);
            }).ToList();
        }

        private static string GetRawScores()
        {
            var path = Path.Combine(Application.dataPath, "HighScore", "results.txt");

            try
            {
                return File.ReadAllText(path);
            }
            catch
            {
                return null;
            }
        }

        public static void SaveScore(this float score)
        {
            var path = Path.Combine(Application.dataPath, "HighScore");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var all = GetRawScores() ?? string.Empty;
            var separator = all == string.Empty ? string.Empty : "|";
            File.WriteAllText(Path.Combine(path,  "results.txt"), all + $"{separator}{PlayerPrefs.GetString("PlayerName", "Bestia")}-{score}");
    }

        public static (float,string) GetTheHighestScore()
        {
            return GetAllScores()?.OrderBy(a => a.Item1).FirstOrDefault() ?? default;
        }

        public static Vector3 FindRandomPositionOnNavMesh(this Vector3 currentPosition, float radius)
        {
            var randomDirection = Random.insideUnitSphere * radius;
            randomDirection += currentPosition;
            return NavMesh.SamplePosition(randomDirection, out var hit, radius, 1) ? hit.position : Vector3.zero;
        }
    }
}