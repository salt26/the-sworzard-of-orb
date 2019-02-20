using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Character))]
public class Mover : MonoBehaviour {

    protected bool isMoving;   // 이동 중에 true

    protected Character c;

    [SerializeField]
    protected GameObject damageNumber;

    public bool IsMoving
    {
        get
        {
            return isMoving;
        }
    }

    public virtual IEnumerator DamagedAnimation(int oldHealth, Slider healthBar = null, StatusUI statusUI = null, Vector3 direction = new Vector3(), bool isCritical = false)
    {
        yield return null;
    }

    public IEnumerator HealedAnimation(int oldHealth, Slider healthBar = null, StatusUI statusUI = null)
    {
        yield return null;
        while (IsMoving)
        {
            yield return null;
        }
        int frame = 30;

        if (c.currentHealth == oldHealth)
        {
            yield break;
        }

        GameObject g = Instantiate(damageNumber, c.canvas.GetComponent<Transform>());
        g.GetComponent<DamageNumber>().Initialize(c.currentHealth - oldHealth, DamageNumber.DamageType.Heal);

        for (int i = 0; i < frame; i++)
        {
            if (i < frame / 2)
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0f, 0.7f, 0.2f, 0.6f), (float)i / frame * 2);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0f, 0.7f, 0.2f, 0.6f), (float)(frame - i) / frame * 2);
            }

            float f = Mathf.Lerp(c.currentHealth, oldHealth, Mathf.Pow(1 - ((float)i / frame), 2f));
            if (healthBar != null)
                healthBar.value = f;
            if (statusUI != null)
            {
                statusUI.UpdateAll(GetComponent<Character>(), (int)f);
            }

            yield return null;
        }
        if (healthBar != null)
            healthBar.value = c.currentHealth;
        if (statusUI != null)
        {
            statusUI.UpdateAll(GetComponent<Character>(), c.currentHealth);
        }
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }

    public IEnumerator PoisonedAnimation(int oldHealth, Slider healthBar = null, StatusUI statusUI = null)
    {
        yield return null;
        while (IsMoving)
        {
            yield return null;
        }
        int frame = 30;

        GameObject g = Instantiate(damageNumber, c.canvas.GetComponent<Transform>());
        g.GetComponent<DamageNumber>().Initialize(oldHealth - c.currentHealth, DamageNumber.DamageType.Poison);

        for (int i = 0; i < frame; i++)
        {
            if (i < frame / 2)
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0.7f, 0f, 0.7f, 0.6f), (float)i / frame * 2);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0.7f, 0f, 0.7f, 0.6f), (float)(frame - i) / frame * 2);
            }

            float f = Mathf.Lerp(c.currentHealth, oldHealth, Mathf.Pow(1 - ((float)i / frame), 2f));
            if (healthBar != null)
                healthBar.value = f;
            if (statusUI != null)
            {
                statusUI.UpdateAll(GetComponent<Character>(), (int)f);
            }

            yield return null;
        }
        if (healthBar != null)
            healthBar.value = c.currentHealth;
        if (statusUI != null)
        {
            statusUI.UpdateAll(GetComponent<Character>(), c.currentHealth);
        }
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }

    public virtual void Death()
    {

    }
}
