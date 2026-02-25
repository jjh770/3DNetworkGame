using Photon.Pun;
using UnityEngine;

public class PlayerItemDropAbility : PlayerAbility
{
    protected override void OnUpdate()
    {
    }
    public void DropItems(Vector3 position)
    {
        int randomCount = UnityEngine.Random.Range(3, 5);
        for (int i = 0; i < randomCount; i++)
        {
            PhotonNetwork.Instantiate("ScoreItem", position, Quaternion.identity, 0,
                new object[] { PhotonNetwork.LocalPlayer.ActorNumber });
        }
    }
}
