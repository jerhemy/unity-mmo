using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityNetcodeIO;

namespace UnityMMO.Server
{
    public class WorldConnectionManager : MonoBehaviour
    {
        //Static instance of WorldConnectionManager which allows it to be accessed by any other script.
        public static WorldConnectionManager instance = null;

        //Awake is always called before any Start functions
        void Awake()
        {
            //Check if instance already exists
            if (instance == null)

                //if not, set instance to this
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            // check for Netcode.IO extension
            // Will provide NetcodeIOSupportStatus enum, either:
            // Available, if Netcode.IO is available and the standalone helper is installed (or if in standalone),
            // Unavailable, if Netcode.IO is unsupported (direct user to install extension)
            // HelperNotInstalled, if Netcode.IO is available but the standalone helper is not installed (direct user to install the standalone helper)
            UnityNetcode.QuerySupport((supportStatus) =>
            {
                UnityNetcode.CreateClient(NetcodeIOClientProtocol.IPv4, ClientConnect);
            });
        }

        private void ClientConnect(NetcodeClient client)
        {
            var token = new byte[32];
            client.Connect(token, ClientConnectSucceedCallback, ClientConnectFailedCallback);
        }

        private void ClientConnectSucceedCallback()
        {

        }

        private void ClientConnectFailedCallback(string error)
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}