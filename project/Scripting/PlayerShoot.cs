using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public int ammo = 10;
    public float recoilForce = 2f;

    public static event Action<int> AmmoChanged;

    InputAction AttackAction;
    PlayerMovement playerMovement;

    void Awake()
    {
        AttackAction = InputSystem.actions.FindAction("Attack");
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if(AttackAction.WasPressedThisFrame() && ammo > 0)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Vector2 direction = playerMovement.lastDirection;
        Vector3 bulletPosition = transform.position + (Vector3)(direction * 0.7f);

        GameObject newBullet = Instantiate(bulletPrefab, bulletPosition, Quaternion.identity);
        Bullet bullet = newBullet.GetComponent<Bullet>();
        bullet.SetDirection(direction);

        ammo = ammo - 1;

        if(AmmoChanged != null)
        {
            AmmoChanged(ammo);
        }

        playerMovement.PushBack(direction, recoilForce);
    }
}
