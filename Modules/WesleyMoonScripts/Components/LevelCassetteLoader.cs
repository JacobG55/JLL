using GameNetcodeStuff;
using JLL.API;
using JLL.API.Compatability;
using LethalLevelLoader;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Video;

namespace WesleyMoonScripts.Components
{
    public class LevelCassetteLoader : NetworkBehaviour
    {
        [Header("Cassette Loader")]
        public string LoaderName = "Galetry";
        public VideoPlayer screenPlayer;
        public VideoClip awakeClip;
        public AudioSource audioPlayer;
        public Animator animator;

        public InteractEvent OnTapeLoaded = new InteractEvent();

        public float tapeEndOffset = -2f;
        public bool isTapePlaying = false;
        public float tapeLength = 0;
        private string unlockedLevelName = string.Empty;

        private PlayerControllerB lastPlayerUsed;

        public InteractEvent OnTapeFinished = new InteractEvent();
        public TapeFinishedEvent[] TapeFinishedEvents = new TapeFinishedEvent[0];

        [System.Serializable]
        public class TapeFinishedEvent
        {
            public string clipName;
            public InteractEvent FinishedEvent = new InteractEvent();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (screenPlayer)
            {
                if (awakeClip != null)
                {
                    screenPlayer.clip = awakeClip;
                    screenPlayer.Play();
                }
                if (audioPlayer)
                {
                    screenPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
                    screenPlayer.EnableAudioTrack(0, true);
                    screenPlayer.SetTargetAudioSource(0, audioPlayer);
                    screenPlayer.controlledAudioTrackCount = 1;
                }
            }
        }

        public void StartLoadingCassette(PlayerControllerB player)
        {
            LoadCassetteServerRpc(player.Index());
        }

        [ServerRpc(RequireOwnership = false)]
        private void LoadCassetteServerRpc(int playerWhoSent)
        {
            LoadCassetteClientRpc(playerWhoSent);
        }

        [ClientRpc]
        private void LoadCassetteClientRpc(int playerWhoSent)
        {
            PlayerControllerB player = RoundManager.Instance.playersManager.allPlayerScripts[playerWhoSent];

            if (player.currentlyHeldObjectServer == null) return;

            bool destroyTape = false;
            foreach (var cassetteItem in player.currentlyHeldObjectServer.GetComponents<LevelCassetteItem>())
            {
                if (cassetteItem.LoaderName == LoaderName)
                {
                    lastPlayerUsed = player;
                    LoadCassette(cassetteItem);
                    OnTapeLoaded.Invoke(lastPlayerUsed);
                    if (cassetteItem.DestroyAfterUse)
                    {
                        destroyTape = true;
                    }
                }
            }
            if (destroyTape)
            {
                player.DestroyItemInSlot(player.currentItemSlot);
            }
        }

        public void LoadCassette(LevelCassetteItem cassette)
        {
            TapeEnded();
            isTapePlaying = true;
            animator?.SetBool("playing", true);

            if (screenPlayer && cassette.Video != null)
            {
                WesleyScripts.Instance.mls.LogInfo($"Attempting to play video.");
                screenPlayer.Stop();
                screenPlayer.clip = cassette.Video;
                screenPlayer.Play();
                tapeLength = (float)cassette.Video.length + tapeEndOffset;

            }

            cassette.AudioLog?.Play();
            cassette.OnCollect.Invoke();

            if (cassette.GetExtendedLevel(out ExtendedLevel level))
            {
                unlockedLevelName = level.SelectableLevel.PlanetName;
                WesleyScripts.Instance.mls.LogInfo($"Unlocking {unlockedLevelName} | Tape Length: {tapeLength}");
                if (JCompatabilityHelper.IsLoaded(JCompatabilityHelper.CachedMods.LethalMoonUnlocks))
                {
                    LMUHelper.ReleaseLock(level.NumberlessPlanetName);
                }
                else
                {
                    level.IsRouteLocked = !cassette.unlockLevel;
                    level.IsRouteHidden = !cassette.unlockLevel;
                }
            }
        }

        void Update()
        {
            if (isTapePlaying)
            {
                tapeLength -= Time.deltaTime;
                if (tapeLength <= 0)
                {
                    TapeEnded();
                }
            }
        }

        private void TapeEnded()
        {
            isTapePlaying = false;
            animator?.SetBool("playing", false);
            tapeLength = 0;
            if (unlockedLevelName != string.Empty)
            {
                JHudHelper.QueueDisplayTip("Route Discovered!", $"Location: {unlockedLevelName}");
                unlockedLevelName = string.Empty;
            }
            OnTapeFinished.Invoke(lastPlayerUsed);
            if (awakeClip != null)
            {
                screenPlayer.clip = awakeClip;
                screenPlayer.Play();
            }
            if (TapeFinishedEvents != null && screenPlayer != null && screenPlayer.clip != null)
            {
                foreach (TapeFinishedEvent tapeEvent in TapeFinishedEvents)
                {
                    if (tapeEvent.clipName == screenPlayer.clip.name)
                    {
                        tapeEvent.FinishedEvent.Invoke(lastPlayerUsed);
                        break;
                    }
                }
            }
        }
    }
}
