using Photon.Pun;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    private void Start()
    {
        // 리소스 폴더에서 "Player" 프리팹을 찾아 생성(인스턴스화)하도록 한다. + 서버에 등록도 함.
        // -> 리소스 폴더는 잘 쓰이지 않음. 다른 방법으로 해결하기
        Transform spawnPoint = SpawnManager.Instance.SpawnPlayer();
        PhotonNetwork.Instantiate("Player", spawnPoint.position, spawnPoint.rotation);
    }
}
