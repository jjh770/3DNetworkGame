using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RoomSetting : MonoBehaviour
{
    private EPlayerGenderType _genderType;
    [SerializeField] private Button _maleButton;
    [SerializeField] private Button _femaleButton;
    [SerializeField] private GameObject _femaleCharacter;
    [SerializeField] private GameObject _maleCharacter;

    [SerializeField] private TMP_InputField _nicknameInputField;
    [SerializeField] private TMP_InputField _roomnameInputField;
    [SerializeField] private Button _createRoomButton;

    private void Start()
    {
        _maleCharacter.SetActive(false);
        _femaleCharacter.SetActive(true);

        _maleButton.onClick.AddListener(() => SetMale());
        _femaleButton.onClick.AddListener(() => SetFemale());
        _createRoomButton.onClick.AddListener(() => MakeRoom());

        _nicknameInputField.onValueChanged.AddListener(nickname =>
        {
            PhotonNetwork.NickName = nickname;
        });
    }
    private void MakeRoom()
    {
        string nickname = _nicknameInputField.text;
        string roomName = _roomnameInputField.text;
        // todo 도메인으로 빼서 규칙을 만들자 (닉네임도 값 객체로 만들고 규칙 생성)
        if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(roomName))
        {
            Debug.Log("닉네임과 방 이름을 입력해주세요.");
            return;
        }

        PhotonNetwork.NickName = nickname;

        // 커스텀 룸 옵션 정의
        var roomOptions = new RoomOptions
        {
            MaxPlayers = 20,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
                { PhotonRoomManager.MasterNickname, PhotonNetwork.NickName }
            },
            IsVisible = true,
            IsOpen = true,
            CustomRoomPropertiesForLobby = new string[] { PhotonRoomManager.MasterNickname },
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { "gender", _genderType.ToString() }
        });
        PhotonNetwork.CreateRoom(roomName, roomOptions);

        // 룸 옵션 정의
        //RoomOptions roomOptions = new RoomOptions();
        //roomOptions.MaxPlayers = 20;  // 룸 최대 접속자 수
        //roomOptions.IsVisible = true; // 로비에서 룸을 보여줄 것인지?
        //roomOptions.IsOpen = true;    // 룸 오픈 여부 

        //// 룸 만들기
        //PhotonNetwork.CreateRoom(roomName, roomOptions);
    }


    private void SetMale() => OnClickGenderButton(EPlayerGenderType.Male);
    private void SetFemale() => OnClickGenderButton(EPlayerGenderType.Female);

    private void OnClickGenderButton(EPlayerGenderType genderType)
    {
        _genderType = genderType;

        _maleCharacter.SetActive(_genderType == EPlayerGenderType.Male);
        _femaleCharacter.SetActive(_genderType == EPlayerGenderType.Female);
    }
}
