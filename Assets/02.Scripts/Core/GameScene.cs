using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    [SerializeField] private AddressablePunPrefabPool _prefabPool;

    private void Awake()
    {
        if (_prefabPool.IsReady)
            SpawnPlayer();
        else
            _prefabPool.OnReady += SpawnPlayer;
    }

    private async void SpawnPlayer()
    {
        List<string> playerAddresses = new();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("gender", out object gender))
            {
                string address = $"Player_{gender}";
                if (!playerAddresses.Contains(address))
                {
                    playerAddresses.Add(address);
                }
            }
        }
        await _prefabPool.Preload(playerAddresses);

        string myGender = PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("gender", out object g)
            ? g.ToString()
            : EPlayerGenderType.Male.ToString(); // 기본값

        Transform spawnPoint = SpawnManager.Instance.SpawnPlayer();
        PhotonNetwork.Instantiate($"Player_{myGender}", spawnPoint.position, spawnPoint.rotation);
    }


    //private void Start()
    //{
    //    // 리소스 폴더에서 "Player" 프리팹을 찾아 생성(인스턴스화)하도록 한다. + 서버에 등록도 함.
    //    // -> 리소스 폴더는 잘 쓰이지 않음. 다른 방법으로 해결하기
    //    Transform spawnPoint = SpawnManager.Instance.SpawnPlayer();
    //    PhotonNetwork.Instantiate("Player", spawnPoint.position, spawnPoint.rotation);
    //}
}
