using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum TentacleOrientation
{
    Auto,
    DownRight,
    DownLeft,
    UpRight,
    UpLeft
}

public class BossTentacleAttackController : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    [SerializeField]
    private SpriteRenderer sr;

    [SerializeField]
    private Hitbox hitboxDown;

    [SerializeField]
    private Hitbox hitboxUp;

    [SerializeField]
    private int damage = 100;

    [SerializeField]
    private TentacleOrientation orientation = TentacleOrientation.DownRight;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(anim);
        Assert.IsNotNull(sr);
        Assert.IsNotNull(hitboxDown);
        Assert.IsNotNull(hitboxUp);

        hitboxDown.Damage = damage;
        hitboxUp.Damage = damage;

        if (orientation == TentacleOrientation.Auto)
        {
            GameObject player = PlayerSystem.Inst.GetPlayer();
            if (player == null)
            {
                Destroy(gameObject);
                return;
            }

            Vector2 directionToPlayer = player.transform.position - transform.position;
            if (directionToPlayer.y > 0)
            {
                if (directionToPlayer.x > 0)
                {
                    orientation = TentacleOrientation.UpRight;
                }
                else
                {
                    orientation = TentacleOrientation.UpLeft;
                }
            }
            else
            {
                if (directionToPlayer.x > 0)
                {
                    orientation = TentacleOrientation.DownRight;
                }
                else
                {
                    orientation = TentacleOrientation.DownLeft;
                }
            }
        }

        if (orientation.ToString().Contains("Left"))
        {
            transform.localScale = new(-1f, 1f, 1f);
        }

        if (orientation.ToString().Contains("Down"))
        {
            anim.Play("attack_down");
            hitboxUp.gameObject.SetActive(false);
        }
        else
        {
            anim.Play("attack_up");
            hitboxDown.gameObject.SetActive(false);
        }
    }

    public void FlashHitbox()
    {
        StartCoroutine(FlashHitboxCoroutine());
    }

    private IEnumerator FlashHitboxCoroutine()
    {
        if (orientation.ToString().Contains("Down"))
        {
            hitboxDown.Enable();
        }
        else
        {
            hitboxUp.Enable();
        }

        yield return new WaitForSeconds(0.1f);

        hitboxDown.Disable();
        hitboxUp.Disable();
        yield break;
    }

    public void EndAttack()
    {
        StartCoroutine(EndAttackCoroutine());
    }

    private IEnumerator EndAttackCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        float startTime = Time.time;
        float duration = 1f;

        while (Time.time - startTime < duration)
        {
            Color spriteColor = sr.color;
            spriteColor.a = 1 - ((Time.time - startTime) / duration);
            sr.color = spriteColor;
            yield return null;
        }

        Destroy(gameObject);
    }
}
