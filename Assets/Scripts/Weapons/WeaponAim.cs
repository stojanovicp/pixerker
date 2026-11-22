using UnityEngine;

// Runs after PlayerController (default order 0) so it reads this frame's FacingLeft.
[DefaultExecutionOrder(1)]
public class WeaponAim : MonoBehaviour
{
    public const float MIRRORED_Y_ROTATION = -180f;

    private void Update()
    {
        Aim();
    }

    private void Aim()
    {
        WeaponInfo weaponInfo = ActiveWeapon.Instance.CurrentWeaponInfo;
        if (weaponInfo == null) { return; }

        if (weaponInfo.aimMode == WeaponAimMode.Mirrored)
        {
            AimMirrored();
        }
        else
        {
            AimFree();
        }
    }

    private void AimFree()
    {
        transform.right = MouseInput.WorldPosition - (Vector2)transform.position;
    }

    // Course behavior: the tilt uses the mouse's absolute screen position, not its position relative to the player.
    private void AimMirrored()
    {
        Vector2 mouseScreenPosition = MouseInput.ScreenPosition;
        float tiltAngle = Mathf.Atan2(mouseScreenPosition.y, mouseScreenPosition.x) * Mathf.Rad2Deg;
        float yRotation = PlayerController.Instance.FacingLeft ? MIRRORED_Y_ROTATION : 0f;

        transform.rotation = Quaternion.Euler(0f, yRotation, tiltAngle);
    }
}
