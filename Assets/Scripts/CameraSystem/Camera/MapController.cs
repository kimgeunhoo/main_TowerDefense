using System.Collections.Generic;
using IGameInterface;
using UnityEngine;

public class MapController : MonoBehaviour, IMapService, IAutoSceneService
{
    [Header("맵 정보 프로바이더 초기화")]
    [SerializeField] MonoBehaviour[] initialProviders;

    readonly List<IMapInfoProvider> mapInfoProviders = new();
    readonly List<ITowerInfoProvider> towerInfoProviders = new();
    readonly List<IEnemyInfoProvider> enemyInfoProviders = new();

    readonly List<TowerInfo> towers = new();
    readonly List<EnemyInfo> enemies = new();

    public Bounds MapBounds { get; private set; }
    public Bounds CameraBounds { get; private set; }
    public bool HasBounds { get; private set; }

    public IReadOnlyList<TowerInfo> Towers => towers;
    public IReadOnlyList<EnemyInfo> Enemies => enemies;

    #region 생명 주기
    void Awake()
    {
        ((IAutoSceneService)this).RegisterSceneServices();
        RegisterInitialProviders();
        RefreshAll();
    }

    void LateUpdate()
    {
        RefreshAll();
    }

    void OnDestroy()
    {
        ((IAutoSceneService)this).UnregisterSceneServices();
    }
    #endregion

    #region 등록
    public void Register(IMapInfoProvider provider)
    {
        if (provider == null || mapInfoProviders.Contains(provider)) return;
        mapInfoProviders.Add(provider);
        RefreshMapInfo();
    }

    public void Unregister(IMapInfoProvider provider)
    {
        if (provider == null) return;
        mapInfoProviders.Remove(provider);
        RefreshMapInfo();
    }

    public void Register(ITowerInfoProvider provider)
    {
        if (provider == null || towerInfoProviders.Contains(provider)) return;
        towerInfoProviders.Add(provider);
        RefreshTowerInfo();
    }

    public void Unregister(ITowerInfoProvider provider)
    {
        if (provider == null) return;
        towerInfoProviders.Remove(provider);
        RefreshTowerInfo();
    }

    public void Register(IEnemyInfoProvider provider)
    {
        if (provider == null || enemyInfoProviders.Contains(provider)) return;
        enemyInfoProviders.Add(provider);
        RefreshEnemyInfo();
    }

    public void Unregister(IEnemyInfoProvider provider)
    {
        if (provider == null) return;
        enemyInfoProviders.Remove(provider);
        RefreshEnemyInfo();
    }
    #endregion

    #region 조회
    public Vector3 ClampCameraPosition(Vector3 position)
    {
        if (!HasBounds) return position;

        position.x = Mathf.Clamp(position.x, CameraBounds.min.x, CameraBounds.max.x);
        position.z = Mathf.Clamp(position.z, CameraBounds.min.z, CameraBounds.max.z);
        return position;
    }

    public bool ContainsWorldPosition(Vector3 worldPos)
        => HasBounds && MapBounds.Contains(worldPos);
    #endregion

    #region 내부 함수
    void RegisterInitialProviders()
    {
        if (initialProviders == null) return;

        foreach (MonoBehaviour provider in initialProviders)
        {
            if (provider is IMapInfoProvider mapProvider) Register(mapProvider);
            if (provider is ITowerInfoProvider towerProvider) Register(towerProvider);
            if (provider is IEnemyInfoProvider enemyProvider) Register(enemyProvider);
        }
    }

    void RefreshAll()
    {
        RefreshMapInfo();
        RefreshTowerInfo();
        RefreshEnemyInfo();
    }

    void RefreshMapInfo()
    {
        HasBounds = false;
        MapBounds = default;
        CameraBounds = default;

        foreach (IMapInfoProvider provider in mapInfoProviders)
        {
            if (provider == null || !provider.TryGetMapInfo(out MapInfo info) || !info.HasBounds)
                continue;

            MapBounds = info.MapBounds;
            CameraBounds = info.CameraBounds;
            HasBounds = true;
            return;
        }
    }

    void RefreshTowerInfo()
    {
        towers.Clear();

        foreach (ITowerInfoProvider provider in towerInfoProviders)
        {
            if (provider != null && provider.TryGetTowerInfo(out TowerInfo info) && info.IsAlive)
                towers.Add(info);
        }
    }

    void RefreshEnemyInfo()
    {
        enemies.Clear();

        foreach (IEnemyInfoProvider provider in enemyInfoProviders)
        {
            if (provider != null && provider.TryGetEnemyInfo(out EnemyInfo info) && info.IsAlive)
                enemies.Add(info);
        }
    }
    #endregion
}
