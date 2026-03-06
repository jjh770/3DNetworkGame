using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablePunPrefabPool : MonoBehaviour, IPunPrefabPool
{
    private Dictionary<string, GameObject> _prefabCache = new();
    private Dictionary<string, AsyncOperationHandle<GameObject>> _handles = new();
    [SerializeField] private List<string> _fixedPreloadAddresses = new();

    public bool IsReady { get; private set; }
    public event Action OnReady;

    private async void Awake()
    {
        PhotonNetwork.PrefabPool = this;
        await Preload(_fixedPreloadAddresses);
        IsReady = true;
        OnReady?.Invoke();
    }

    public async Task Preload(List<string> addresses)
    {
        List<Task> tasks = new();
        foreach (string address in addresses)
        {
            if (_prefabCache.ContainsKey(address)) continue;
            tasks.Add(LoadSingle(address));
        }
        await Task.WhenAll(tasks);
    }

    private async Task LoadSingle(string address)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(address);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _prefabCache[address] = handle.Result;
            _handles[address] = handle;
        }
        else
            Debug.LogError($"Addressables 로드 실패: {address}");
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        if (!_prefabCache.TryGetValue(prefabId, out GameObject prefab))
        {
            Debug.LogError($"'{prefabId}'이 캐시에 없습니다.");
            return null;
        }
        return UnityEngine.Object.Instantiate(prefab, position, rotation);
    }

    public void Destroy(GameObject gameObject)
    {
        UnityEngine.Object.Destroy(gameObject);
    }

    private void OnDestroy()
    {
        foreach (AsyncOperationHandle<GameObject> handle in _handles.Values)
            Addressables.Release(handle);
    }
}
