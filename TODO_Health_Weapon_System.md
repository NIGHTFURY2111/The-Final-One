# TODO: Health, Damage & Weapon System Implementation

## Phase 1: Core Infrastructure (The "Glue")
- [ ] 1. Create `IDamageable.cs` interface and `DamageInfo` struct.
- [ ] 2. Create `CombatEnums.cs` to hold `FireMode`, `WeaponType`, and `BulletType`.

## Phase 2: Health System Upgrade (`EC_Health`)
- [ ] 3. Refactor `EC_Health.cs` to use cached totals for `Current` and `Max` (Performance).
- [ ] 4. Implement `HOTData` struct and `HealOverTime` logic.
- [ ] 5. Implement `TakeDamage` from `IDamageable` and add HOT interruption logic.
- [ ] 6. Add `UnityEvents` for `OnDamage`, `OnHeal`, and `OnDeath`.

## Phase 3: Data Containers (ScriptableObjects)
- [ ] 7. Implement `SO_BulletData.cs`.
- [ ] 8. Implement `SO_WeaponData.cs` (referencing `SO_BulletData`).

## Phase 4: Object Pooling (Projectiles)
- [ ] 9. Create `ProjectilePool.cs` (Simple Static/Singleton pool).
- [ ] 10. Create a base `PooledProjectile.cs` script to handle returning to pool on hit.

## Phase 5: Weapon Logic (`EC_WeaponManager`)
- [ ] 11. Create `EC_WeaponManager.cs` component.
- [ ] 12. Implement `Fire()` logic (timers, ammo, fire-rate).
- [ ] 13. Integrate `Hitscan` vs `Projectile` logic using the `ProjectilePool`.
- [ ] 14. Link to `SO_InputAccess` for shooting and reloading.

## Phase 6: Integration & MVP Test
- [ ] 15. Update `Player_Entity` to include `EC_WeaponManager`.
- [ ] 16. Create a test "Dummy" enemy using `EC_Health`.
- [ ] 17. Verify: Shooting dummy reduces health, interrupts HOT, and uses pooled projectiles.
