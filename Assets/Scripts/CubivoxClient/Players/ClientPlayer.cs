﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using CubivoxCore;
using CubivoxCore.BaseGame;
using CubivoxCore.Commands;
using CubivoxCore.Players;
using CubivoxCore.Exceptions;
using CubivoxClient.Protocol.ServerBound;

namespace CubivoxClient.Players
{
    public class ClientPlayer : MonoBehaviour, Player
    {
        public string Username { get; internal set; }
        public Guid Uuid { get; internal set; }

        /// <summary>
        /// If the Capsule is a local player.
        /// </summary>
        public bool IsLocalPlayer = false;

        private Rigidbody rigidbody;

        void Start()
        {
            ClientCubivox.GetClientInstance().GetPlayers().Add(this);

            if(!IsLocalPlayer)
            {
                // Initalize the username billboard.
                GetComponentInChildren<TextMesh>().text = Username;
            }

            // Send the position packet 20 times per second.
            if(IsLocalPlayer)
            {
                InvokeRepeating("SendPositionPacket", 1, 0.03f);
            }

            rigidbody = GetComponent<Rigidbody>();
        }


        public Entity GetEntityType()
        {
            return null;
        }

        public Location GetLocation()
        {
            if (!IsLocalPlayer)
            {
                throw new InvalidEnvironmentException("You can only obtain the location of the local player!");
            }

            return LocationUtils.VectorToLocation(transform.position);
        }

        public string GetName()
        {
            return Username;
        }

        public Guid GetUUID()
        {
            return Uuid;
        }

        public bool IsLiving()
        {
            return true;
        }

        public new void SendMessage(string message)
        {
            if (!IsLocalPlayer)
            {
                throw new InvalidEnvironmentException("You can only send a message to the local player!");
            }
        }

        void Update()
        {
            if(!IsLocalPlayer)
            {
                // Update the Username billboard rotation.
                transform.GetChild(0).transform.rotation = Camera.main.transform.rotation;
                rigidbody.velocity = Vector3.zero;
            }
        }

        private void SendPositionPacket() {
            if (!IsLocalPlayer) return;

            if (ClientCubivox.GetClientInstance().CurrentState != GameState.PLAYING) return;

            if (transform.hasChanged)
            {
                ClientCubivox.GetClientInstance().SendPacketToServer(new UpdatePlayerPositionPacket(LocationUtils.VectorToLocation(transform.position)));
                transform.hasChanged = false;
            }
        }

        public void Kick(string reason)
        {
            if(!IsLocalPlayer)
            {
                throw new InvalidEnvironmentException("Only the local player can be kicked from the client.");
            }
            ClientCubivox.GetClientInstance().DisconnectClient(reason);
        }

        public void SetLocation(Location location)
        {
            if (!IsLocalPlayer)
            {
                throw new InvalidEnvironmentException("Only the local player's position can be modified.");
            }
            transform.position = LocationUtils.LocationToVector(location);
        }
    }
}
