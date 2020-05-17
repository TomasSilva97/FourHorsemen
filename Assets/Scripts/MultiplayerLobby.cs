
using System.Collections.Generic;
using UnityEngine;

using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace Photon.Pun.Demo.PunBasics
{
    public class MultiplayerLobby : MonoBehaviourPunCallbacks, ILobbyCallbacks
    {

        [Tooltip("The UI Loader Anime")]
        [SerializeField]
        private LoaderAnime loaderAnime;
        public static MultiplayerLobby lobby;
        public string roomName;
        public int roomSize;
        public GameObject roomListingPrefab;
        public Transform roomsPanel;
        void Awake()
        {
            lobby = this;
        }

        void Start()
        {
            PhotonNetwork.ConnectUsingSettings();//Connects to master server
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Player has connected to the master server.");
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            base.OnRoomListUpdate(roomList);
            RemoveRoomListings();
            foreach (RoomInfo room in roomList)
            {
                ListRoom(room);
            }
        }

        void RemoveRoomListings()
        {
            while (roomsPanel.childCount != 0)
            {
                Destroy(roomsPanel.GetChild(0).gameObject);
            }
        }

        void ListRoom(RoomInfo room)
        {
            if (room.IsOpen && room.IsVisible)
            {
                GameObject tempListing = Instantiate(roomListingPrefab, roomsPanel);
                RoomButton tempButton = tempListing.GetComponent<RoomButton>();
                tempButton.roomName = room.Name;
                tempButton.roomSize = room.MaxPlayers;
                tempButton.SetRoom();
            }
        }

        public void CreateRoom()
        {
            Debug.Log("trying to create a room.");

            if (loaderAnime != null)
            {
                loaderAnime.StartLoaderAnimation();
            }

            RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };

            PhotonNetwork.CreateRoom(roomName, roomOps);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Trying to create a new room but failed, there must already be a room with that name");
        }

        public void OnRoomNameChange(string nameIn)
        {
            roomName = nameIn;
        }

        public void OnRoomSizeChange(string sizeIn)
        {
            roomSize = int.Parse(sizeIn);
        }

        public void JoinLobbyOnClick()
        {
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        public void Back(){
            SceneManager.LoadScene(0);
        }

    }
}