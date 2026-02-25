using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

// 플레이어의 대표로서 외부와의 소통 및 어빌리티들을 관리하는 역할
public class PlayerController : MonoBehaviour, IPunObservable
{
    public PhotonView PhotonView;
    public PlayerStat Stat;
    public event Action OnStatSynced;

    public static event Action<Transform> OnLocalPlayerSpawned;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 읽기(쓰기) 모드
        if (stream.IsWriting)
        {
            // 이 PhotonView의 데이터를 보내줘야하는 상황
            stream.SendNext(Stat.CurrentHealth);  // 현재 체력과 스태미나
            stream.SendNext(Stat.CurrentStamina);
            // 보낼 값이 많아진다면 JSON이나 BinaryFormatter로 직렬화해서 보내는 방법도 있다.
            // SendNext, ReceiveNext에 쓰이는 캐스팅에 필요한 박싱 언박싱의 비용과 JsonUtility의 비용을 비교해서 선택하면 된다.
        }

        else if (stream.IsReading)
        {
            // 이 PhotonView의 데이터를 받아야하는 상황
            // ReceiveNext()는 object 타입이므로, 캐스팅 필요
            // 받는 쪽에서는, 보내는 쪽에서 보낸 순서대로 ReceiveNext()를 호출해야한다.
            Stat.CurrentHealth = (float)stream.ReceiveNext();
            Stat.CurrentStamina = (float)stream.ReceiveNext();
        }

        OnStatSynced?.Invoke();
    }

    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
        Stat.Initialize();
        NotifySpawned();
    }

    private void Update()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
    }

    public void NotifySpawned()
    {
        if (PhotonView.IsMine)
            OnLocalPlayerSpawned?.Invoke(transform);
    }

    private Dictionary<Type, PlayerAbility> _abilitiesCache = new();

    public T GetAbility<T>() where T : PlayerAbility
    {
        var type = typeof(T);

        if (_abilitiesCache.TryGetValue(type, out PlayerAbility ability))
        {
            return ability as T;
        }

        // 게으른 초기화/로딩 -> 처음에 곧바로 초기화/로딩을 하는게 아니라
        //                    필요할때만 하는.. 뒤로 미루는 기법
        ability = GetComponent<T>();

        if (ability != null)
        {
            _abilitiesCache[ability.GetType()] = ability;

            return ability as T;
        }

        throw new Exception($"어빌리티 {type.Name}을 {gameObject.name}에서 찾을 수 없습니다.");
    }
}
