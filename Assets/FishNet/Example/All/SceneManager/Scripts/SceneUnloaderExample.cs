﻿using FishNet;
using FishNet.Managing.Scened.Data;
using FishNet.Object;
using UnityEngine;

namespace FirstGearGames.FlexSceneManager.Demos
{

    /// <summary>
    /// Unloads specified scenes when entering or exiting this trigger.
    /// </summary>
    public class SceneUnloaderExample : MonoBehaviour
    {
        /// <summary>
        /// Scenes to unload.
        /// </summary>
        [Tooltip("Scenes to unload.")]
        [SerializeField]
        private string[] _unloadScenes = null;
        /// <summary>
        /// True to only unload for the connectioning causing the trigger.
        /// </summary>
        [Tooltip("True to only unload for the connectioning causing the trigger.")]
        [SerializeField]
        private bool _connectionOnly = false;
        /// <summary>
        /// True to unload unused scenes.
        /// </summary>
        [Tooltip("True to unload unused scenes.")]
        [SerializeField]
        private bool _unloadUnused = true;
        /// <summary>
        /// True to fire when entering the trigger. False to fire when exiting the trigger.
        /// </summary>
        [Tooltip("True to fire when entering the trigger. False to fire when exiting the trigger.")]
        [SerializeField]
        private bool _onTriggerEnter = true;


        [Server]
        private void OnTriggerEnter(Collider other)
        {
            if (!_onTriggerEnter)
                return;

            UnloadScenes(other.gameObject.GetComponent<NetworkObject>());
        }

        [Server]
        private void OnTriggerExit(Collider other)
        {
            if (_onTriggerEnter)
                return;

            UnloadScenes(other.gameObject.GetComponent<NetworkObject>());
        }

        /// <summary>
        /// Unload scenes.
        /// </summary>
        /// <param name="triggeringIdentity"></param>
        private void UnloadScenes(NetworkObject triggeringIdentity)
        {
            if (!InstanceFinder.NetworkManager.IsServer)
                return;

            //NetworkObject isn't necessarily needed but to ensure its the player only run if nob is found.
            if (triggeringIdentity == null)
                return;

            AdditiveScenesData asd = new AdditiveScenesData(_unloadScenes);
            //Unload only for the triggering connection.
            if (_connectionOnly)
            {
                UnloadOptions.UnloadModes mode = (_unloadUnused) ? UnloadOptions.UnloadModes.UnloadUnused : UnloadOptions.UnloadModes.KeepUnused;
                UnloadOptions unloadOptions = new UnloadOptions
                {
                    Mode = mode
                };
                InstanceFinder.SceneManager.UnloadConnectionScenes(triggeringIdentity.Owner, asd, unloadOptions);
            }
            //Unload for all players.
            else
            {
                InstanceFinder.SceneManager.UnloadNetworkedScenes(asd);
            }
        }


    }


}