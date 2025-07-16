using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatUnitView : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private GameObject animPrefab; // from unit a copy of the prefab
    private Animator animator;
    public void SetFromUnit(Unit unit, bool isLeft)
    {
        if (unit.animPrefab) // this if is just because there are some units without animations yet and it would crash otherwise
        {
            spriteRenderer.sprite = null; // set sprite to null (remove sprite renderer from combat scene later, then this line not needed)
            animPrefab = Instantiate(unit.animPrefab, transform); // add the animation in
            animator = animPrefab?.GetComponent<Animator>(); // ease of access
            if (isLeft) // do this better later
            {
                Vector3 scale = transform.localScale;
                scale.x = -5f;
                transform.localScale = scale;
            }
        }
        else
        {
            spriteRenderer.sprite = unit.combatSprite;
            spriteRenderer.color = Color.white;
        }
        if (isLeft)
        {
            transform.localPosition = new Vector3(-5, 0, 0);
        }
        else
        {
            transform.localPosition = new Vector3(5, 0, 0);
        }
    }

    public void FlipSprite(bool flip)
    {
        spriteRenderer.flipX = flip;
    }

    public IEnumerator Lunge(float distance)
    {
        if (animPrefab) // same as above for why theres an if statement
        {
            animator.SetBool("isAttacking", true); // set state to attacking

            yield return null; // delay one frame so it can switch

            float clipLength = animator.GetCurrentAnimatorStateInfo(0).length; // get clip length
            yield return new WaitForSeconds(clipLength); // wait for animation to play

            animator.SetBool("isAttacking", false); // go back to idle
        }

        else
        {
            Vector3 start = transform.localPosition;
            Vector3 target = start + new Vector3(distance, 0, 0);

            float t = 0;
            while (t < 1f)
            {
                transform.localPosition = Vector3.Lerp(start, target, t);
                t += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = start;
        }
    }

    public IEnumerator FlashHit()
    {
        // same but for hurt
        if (animator) 
        {
            animator.GetComponent<Animator>().SetBool("isHurt", true);

            yield return null; 

            float clipLength = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(clipLength);

            animator.GetComponent<Animator>().SetBool("isHurt", false);
        }
        else
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.4f);
            spriteRenderer.color = Color.white;
        }
    }

    public IEnumerator Dodge()
    {
        // same but for dodge
        if (animator)
        {
            animator.GetComponent<Animator>().SetBool("isDodging", true);

            yield return null;

            float clipLength = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(clipLength);

            animator.GetComponent<Animator>().SetBool("isDodging", false);
        }
        else
        {
            Vector3 original = transform.localPosition;
            Vector3 offset = original + new Vector3(0, 1f, 0);
            transform.localPosition = offset;
            yield return new WaitForSeconds(0.2f);
            transform.localPosition = original;
        }
    }

    public IEnumerator CritEffect()
    {
        // flash gold or smth or like a weapon sheen or for mages like an eye sparkle
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    public IEnumerator PlayDeath()
    {
        for (float i = 1f; i >= 0; i -= Time.deltaTime * 2f)
        {
            spriteRenderer.color = new Color(1, 1, 1, i);
            yield return null;
        }
    }
    public void ExitCombat()
    {
        // remove anim prefab from root so it isn't there next time combat happens
        if (animPrefab)
        {
            Destroy(animPrefab);
        }
    }
}