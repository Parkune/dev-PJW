using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.Events;

public class RoomData : MonoBehaviour
{
    private TMP_Text roomInfoText;
    private RoomInfo _roomInfo;

    // C# 프로퍼티 지정
    public RoomInfo RoomInfo
    {
        get
        {
            return _roomInfo;
        }
        set
        {
            _roomInfo = value;
            // 룸이름 (10/100)
            roomInfoText.text = $"{_roomInfo.Name} ({_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers})";
            // 버튼 클릭 이벤트를 연결
            this.gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnEnterRoom(_roomInfo.Name));
            //this.gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { OnEnterRoom(_roomInfo.Name); });
        }
   
    }

    void Awake()
    {
        roomInfoText = GetComponentInChildren<TMP_Text>();
    }

    void OnEnterRoom(string roomName)
    {
        PhotonNetwork.NickName = PlayerPrefs.GetString("USER_ID");
        PhotonNetwork.JoinRoom(roomName);
    }
}
