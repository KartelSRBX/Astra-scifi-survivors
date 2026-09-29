using System;
using System.Collections.Generic;
using UnityEngine;
using GamingApp.Core;
using GamingApp.Enemies;

namespace GamingApp.Weapons
{
    /// <summary>
    /// The player's single run weapon. Stats = definition base x loadout/generic multipliers x weapon steps.
    /// Handles projectile volleys, laser beams and drone squads.
    /// </summary>
    public class PlayerWeapon : MonoBehaviour
    {
        public WeaponDef Def { get; private set; }
        public int Level { get; private set; }
        public WeaponStats Stats { get; private set; } = new();

        // Ship, items and generic level-up cards.
        [NonSerialized] public float damageMult = 1f, fireRateMult = 1f, rangeMult = 1f, projectileSpeedMult = 1f;
        [NonSerialized] public int extraProjectiles;

        float stepDamage = 1f, stepRate = 1f, stepRange = 1f, stepAoe = 1f, stepSetAoe;
        int stepProjectiles, stepPierce, stepDrones;

        float cooldown;
        readonly List<Drone> drones = new();
        readonly List<LineRenderer> beams = new();
        readonly List<Enemy> beamTargets = new();
        int beamTicks;

        public event Action Changed;

        public bool CanLevel => Def != null && Level < Def.steps.Length;
        public WeaponStep NextStep => CanLevel ? Def.steps[Level] : null;

        public void Equip(WeaponDef def)
        {
            Def = def;
            Level = 0;
            stepDamage = stepRate = stepRange = stepAoe = 1f;
            stepSetAoe = 0f;
            stepProjectiles = stepPierce = stepDrones = 0;
            Recompute();
        }

        public void ApplyNextStep()
        {
            var s = NextStep;
            if (s == null) return;
            stepDamage *= s.damageMult;
            stepRate *= s.fireRateMult;
            stepRange *= s.rangeMult;
            stepAoe *= s.aoeMult;
            stepSetAoe = Mathf.Max(stepSetAoe, s.setAoe);
            stepProjectiles += s.addProjectiles;
            stepPierce += s.addPierce;
            stepDrones += s.addDrones;
            Level++;
            Recompute();
        }

        public void Recompute()
        {
            if (Def == null) return;
            var b = Def.stats;
            var s = b.Clone();
            s.damage = b.damage * damageMult * stepDamage;
            s.fireInterval = b.fireInterval / (fireRateMult * stepRate);
            s.range = b.range * rangeMult * stepRange;
            s.speed = b.speed * projectileSpeedMult;
            s.projectiles = b.projectiles + stepProjectiles + extraProjectiles;
            s.aoe = Mathf.Max(b.aoe, stepSetAoe) * stepAoe;
            s.pierce = b.pierce + stepPierce;
            s.drones = b.drones + stepDrones;
            Stats = s;
            SyncDrones();
            Changed?.Invoke();
        }

        void SyncDrones()
        {
            if (Def == null || Def.mode != WeaponMode.Drones || Def.dronePrefab == null) return;
            while (drones.Count < Stats.drones)
            {
                var d = Instantiate(Def.dronePrefab, transform.position, Quaternion.identity);
                d.Init(this, drones.Count);
                drones.Add(d);
            }
            for (int i = 0; i < drones.Count; i++) drones[i].SetSlot(i);
        }

        void OnDestroy()
        {
            foreach (var d in drones) if (d != null) Destroy(d.gameObject);
        }

        void Update()
        {
            var gm = GameManager.Instance;
            bool playing = gm != null && gm.State == RunState.Playing;
            if (Def == null) return;

            if (Def.mode == WeaponMode.Beam) { UpdateBeams(playing); return; }
            if (Def.mode != WeaponMode.Projectile || !playing) return;

            cooldown -= Time.deltaTime;
            if (cooldown > 0f) return;
            var target = Enemy.Nearest(transform.position, Stats.range);
            if (target == null) return;
            cooldown = Stats.fireInterval;
            FireVolley(transform.position, target);
        }

        /// <summary>Fires Stats.projectiles shots in a fan towards the target. Also used by drones.</summary>
        public void FireVolley(Vector3 origin, Enemy target)
        {
            Vector3 dir = target.transform.position - origin;
            dir.y = 0f;
            dir.Normalize();
            int n = Stats.projectiles;
            origin.y = 1.5f;
            for (int i = 0; i < n; i++)
            {
                float offset = n == 1 ? 0f : (i - (n - 1) * 0.5f) * Stats.spread;
                offset += UnityEngine.Random.Range(-2f, 2f);
                Vector3 d = Quaternion.Euler(0f, offset, 0f) * dir;
                var p = Instantiate(Def.projectilePrefab, origin, Quaternion.LookRotation(d, Vector3.up));
                p.Launch(d, Stats, Def, target);
            }
        }

        // ---------- laser ----------
        void UpdateBeams(bool playing)
        {
            if (playing)
            {
                cooldown -= Time.deltaTime;
                if (cooldown <= 0f)
                {
                    cooldown = Stats.fireInterval;
                    Enemy.NearestN(transform.position, Stats.range, Stats.projectiles, beamTargets);
                    beamTicks++;
                    foreach (var e in beamTargets)
                    {
                        e.Health.TakeDamage(Stats.damage);
                        if (beamTicks % 3 == 0) FxBurst.Spawn(Def.hitPrefab, e.transform.position, 0.9f, Def.color);
                    }
                }
            }
            else beamTargets.Clear();

            while (beams.Count < beamTargets.Count) beams.Add(CreateBeam());
            Vector3 from = transform.position; from.y = 0.3f;
            for (int i = 0; i < beams.Count; i++)
            {
                bool on = i < beamTargets.Count && beamTargets[i] != null && beamTargets[i].Alive;
                beams[i].enabled = on;
                if (!on) continue;
                Vector3 to = beamTargets[i].transform.position; to.y = 0.3f;
                beams[i].SetPosition(0, from);
                beams[i].SetPosition(1, to);
                float w = 0.32f + Mathf.Sin(Time.time * 40f + i) * 0.05f;
                beams[i].startWidth = w;
                beams[i].endWidth = w * 0.7f;
            }
        }

        LineRenderer CreateBeam()
        {
            var go = new GameObject("Beam");
            go.transform.SetParent(transform, false);
            var lr = go.AddComponent<LineRenderer>();
            lr.useWorldSpace = true;
            lr.positionCount = 2;
            lr.material = Def.beamMaterial;
            lr.textureMode = LineTextureMode.Stretch;
            lr.numCapVertices = 4;
            lr.startColor = lr.endColor = Def.color;
            lr.sortingOrder = 5;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;
            return lr;
        }
    }
}
