using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatUnitView : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public void SetFromUnit(Unit unit, bool isLeft)
    {
        spriteRenderer.sprite = unit.combatSprite;
        spriteRenderer.color = Color.white;
        if(isLeft)
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

    public IEnumerator FlashHit()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.4f);
        spriteRenderer.color = Color.white;
    }

    public IEnumerator Dodge()
    {
        Vector3 original = transform.localPosition;
        Vector3 offset = original + new Vector3(0, 1f, 0);
        transform.localPosition = offset;
        yield return new WaitForSeconds(0.2f);
        transform.localPosition = original;
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
            spriteRenderer.color = new Color(1,1,1,i);
            yield return null;
        }
    }
}