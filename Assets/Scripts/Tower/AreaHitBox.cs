using System.Collections.Generic;
using UnityEngine;

public class AreaHitBox : PoolableObject
{
    private int damage;
    private LayerMask monsterLayer;
    private HitBoxData hitBoxData;

    // 공격 속도, 간격
    private float attackSpeed;
    private float tickInterval;

    private Dictionary<Monster, float> damageTimers = new Dictionary<Monster, float>();
    private IHitBoxShapeInitializer shapeInitializer;

    private Collider Collider;

    private Transform target;

    private void Awake()
    {
        Collider = GetComponent<Collider>();
    }

    public void Initialize(Transform target, int damage, LayerMask monsterLayer, HitBoxData data, float attackSpeed)
    {
        this.target = target;
        this.damage = damage;
        this.monsterLayer = monsterLayer;
        this.hitBoxData = data;
        this.attackSpeed = attackSpeed;

        damageTimers.Clear();

        if (Collider == null)
            Collider = GetComponent<Collider>();

        if (shapeInitializer == null)
            shapeInitializer = GetComponent<IHitBoxShapeInitializer>();

        if (shapeInitializer == null)
        {
            Debug.LogError($"{name}에 IHitBoxShapeInitializer가 없습니다.");
            return;
        }

        shapeInitializer.Initialize(data);
    } 

    private void OnTriggerStay(Collider other)
    {
        if (((1 << other.gameObject.layer) & monsterLayer) == 0)
            return;

        Monster monster = other.GetComponentInParent<Monster>();

        if (monster == null) 
            return;

        float tickInterval = hitBoxData.damageInterval / Mathf.Max(0.01f, attackSpeed);

        if (!damageTimers.ContainsKey(monster))
        {
            monster.TakeDamage(damage);
            damageTimers[monster] = tickInterval;
            return;
        }

        damageTimers[monster] -= Time.deltaTime;

        if (damageTimers[monster] <= 0f)
        {
            monster.TakeDamage(damage);
            damageTimers[monster] = tickInterval;
        }

    }

    public override void OnDespawned()
    {
        target = null;
        damageTimers.Clear();
        //hitBoxData = null;

        base.OnDespawned();
    }

}
