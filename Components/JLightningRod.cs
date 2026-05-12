using JLL.API;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class JLightningRod : MonoBehaviour
    {
        public static List<JLightningRod> All = [];

        [Header("Striking")]
        [Tooltip("Time in seconds between lighting strikes | negative number to disable")]
        public float strikeInterval = -1f;
        private float strikeTimer = 0;
        [Tooltip("Valid positions for lighting strikes. Empty strikes self")]
        public Transform[] strikePos = [];
        public float killRange = 2.4f;
        public float damageRange = 5f;
        [Tooltip("Ignore ceiling checks allowing lightning to strike through objects to hit its target.")]
        public bool ignorePath = false;
        public float boltHeight = 80f;
        public int boltWidth = 32;
        [Tooltip("Default: only strike on stormy weather | Enable to strike reguardless of weather")]
        public bool ignoreCurrentWeather = false;

        [Header("Detection")]
        [Tooltip("Distance a lightning strike has to be from transform position to trigger event")]
        public float detectDist = 1f;
        public UnityEvent onStrike = new();

        public void OnEnable()
        {
            All.Add(this);
            strikeTimer = strikeInterval;
        }

        public void Update()
        {
            if (!RoundManager.Instance.IsOwner || strikeInterval <= 0) return;

            if (ignoreCurrentWeather || TimeOfDay.Instance.currentLevelWeather == LevelWeatherType.Stormy)
            {
                strikeTimer -= Time.deltaTime;
                if (strikeTimer < 0)
                {
                    strikeTimer = strikeInterval;
                    StrikeRandom();
                }
            }
        }

        public void StrikeRandom()
        {
            StrikeAt(strikePos.Length == 0 ? transform.position : strikePos[Random.Range(0, strikePos.Length)].position);
        }

        public void StrikeAt(Vector3 pos)
        {
            if (JLLNetworkManager.Instance.IsOwner)
            {
                JLLNetworkManager.Instance.LightningStrikeRpc(pos, Random.Range(0, 1000000), killRange, damageRange, ignorePath, boltHeight, boltWidth);
            }
        }

        public void OnDisable()
        {
            All.Remove(this);
        }
    }
}
