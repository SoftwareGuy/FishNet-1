﻿using FishNet.Connection;
using FishNet.Observing;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FishNet.Object
{
    public partial class NetworkObject : MonoBehaviour
    {
        #region Public.
        /// <summary>
        /// Called when this NetworkObject losses all observers or gains observers while previously having none.
        /// </summary>
        public event Action<NetworkObject> OnObserversActive;
        //public bool UsingObservers => (NetworkObserver != null);
        /// <summary>
        /// NetworkObserver on this object. May be null if not using observers.
        /// </summary>
        [HideInInspector]
        public NetworkObserver NetworkObserver = null;
        /// <summary>
        /// Clients which can get messages from this NetworkObject.
        /// </summary>
        public HashSet<NetworkConnection> Observers = new HashSet<NetworkConnection>();
        #endregion


        /// <summary>
        /// Initializes this script for use.
        /// </summary>
        private void PreInitializeObservers()
        {
            NetworkObserver = GetComponent<NetworkObserver>();
            if (NetworkObserver != null)
                NetworkObserver.PreInitialize(this);
        }

        /// <summary>
        /// Removes a connection from observers for this object.
        /// </summary>
        /// <param name="connection"></param>
        internal bool RemoveObserver(NetworkConnection connection)
        {
            int startCount = Observers.Count;
            bool removed = Observers.Remove(connection);

            if (removed)
                TryInvokeOnObserversActive(startCount);

            return removed;
        }

        /// <summary>
        /// Adds the connection to observers if conditions are met.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns>True if added to Observers.</returns>
        internal ObserverStateChange RebuildObservers(NetworkConnection connection)
        {
            int startCount = Observers.Count;
            //Not using observer system, this object is seen by everything.
            if (NetworkObserver == null)
            {
                bool added = Observers.Add(connection);
                if (added)
                    TryInvokeOnObserversActive(startCount);

                return (added) ? ObserverStateChange.Added : ObserverStateChange.Unchanged;
            }
            else
            {
                ObserverStateChange osc = NetworkObserver.RebuildObservers(connection);
                if (osc == ObserverStateChange.Added)
                    Observers.Add(connection);
                else if (osc == ObserverStateChange.Removed)
                    Observers.Remove(connection);

                if (osc != ObserverStateChange.Unchanged)
                    TryInvokeOnObserversActive(startCount);

                return osc;
            }

        }

        /// <summary>
        /// Invokes OnObserversActive if observers are now 0 but previously were not, or if was previously 0 but now has observers.
        /// </summary>
        /// <param name="startCount"></param>
        private void TryInvokeOnObserversActive(int startCount)
        {
            if ((Observers.Count > 0 && startCount == 0) ||
                Observers.Count == 0 && startCount > 0)
                OnObserversActive?.Invoke(this);
        }

    }

}

