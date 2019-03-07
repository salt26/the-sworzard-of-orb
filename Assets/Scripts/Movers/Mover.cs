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
    [SerializeField]
    protected GameObject stunEffect;
    protected GameObject myStunMark;

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

    public IEnumerator HealedAnimation(int oldHealth, Slider healthBar = null, StatusUI statusUI = null, bool isDrain = false)
    {
        isMoving = true;
        int frame = 25;
        if (isDrain) frame = 6;
        Color original = GetComponent<SpriteRenderer>().color;
        Color afterColor = new Color(1f, 1f, 1f, 1f);
        if (isDrain) afterColor = original;

        if (c.currentHealth == oldHealth)
        {
            GetComponent<SpriteRenderer>().color = afterColor;
            isMoving = false;

            // 여기서도 코루틴이 종료될 수 있음에 주의!
            yield break;
        }

        GameObject g = Instantiate(damageNumber, c.canvas.GetComponent<Transform>());
        g.GetComponent<DamageNumber>().Initialize(c.currentHealth - oldHealth, DamageNumber.DamageType.Heal);

        for (int i = 0; i < frame; i++)
        {
            if (!isDrain)
            {
                if (i < frame / 2)
                {
                    GetComponent<SpriteRenderer>().color = Color.Lerp(original, new Color(0f, 0.7f, 0.2f, 0.6f), (float)i / frame * 2);
                }
                else
                {
                    GetComponent<SpriteRenderer>().color = Color.Lerp(afterColor, new Color(0f, 0.7f, 0.2f, 0.6f), (float)(frame - i) / frame * 2);
                }
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
        GetComponent<SpriteRenderer>().color = afterColor;
        isMoving = false;
    }

    public IEnumerator PoisonedAnimation(int oldHealth, Slider healthBar = null, StatusUI statusUI = null)
    {
        isMoving = true;
        int frame = 10;
        Color original = GetComponent<SpriteRenderer>().color;

        GameObject g = Instantiate(damageNumber, c.canvas.GetComponent<Transform>());
        g.GetComponent<DamageNumber>().Initialize(oldHealth - c.currentHealth, DamageNumber.DamageType.Poison);

        for (int i = 0; i < frame; i++)
        {
            if (i < frame / 2)
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(original, new Color(0.7f, 0f, 0.7f, 0.6f), (float)i / frame * 2);
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
                statusUI.UpdateAll(c, (int)f);
            }

            yield return null;
        }
        if (healthBar != null)
            healthBar.value = c.currentHealth;
        if (statusUI != null)
        {
            statusUI.UpdateAll(c, c.currentHealth);
        }
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        isMoving = false;
    }

    public IEnumerator GustedAnimation(int oldHealth, Slider healthBar = null, StatusUI statusUI = null, Vector3 direction = new Vector3())
    {
        isMoving = true;
        float frame = 16, frame2 = 0, frame3 = 10;
        Vector3 originalPosition = GetComponent<Transform>().position;
        Color originalColor = GetComponent<SpriteRenderer>().color;
        
        GameObject g = Instantiate(damageNumber, c.canvas.GetComponent<Transform>());
        g.GetComponent<DamageNumber>().Initialize(oldHealth - c.currentHealth, DamageNumber.DamageType.Flurry);

        for (int i = 0; i < frame + frame2; i++)
        {
            if (i < frame / 2)
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(originalColor, new Color(0f, 0.7f, 0.7f, 0.6f), i / frame * 2);
                if (!direction.Equals(new Vector3()))
                    GetComponent<Transform>().position = Vector3.Lerp(originalPosition, originalPosition + 0.2f * direction, i / frame * 2);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(originalColor, new Color(0f, 0.7f, 0.7f, 0.6f), (frame + frame2 - i) / (frame / 2 + frame2));
                if (!direction.Equals(new Vector3()))
                    GetComponent<Transform>().position = Vector3.Lerp(originalPosition, originalPosition + 0.2f * direction, (frame + frame2 - i) / (frame / 2 + frame2));
            }

            if (i < frame)
            {
                float f = Mathf.Lerp(c.currentHealth, oldHealth, Mathf.Pow(1 - (i / frame), 2f));
                if (healthBar != null)
                    healthBar.value = f;
                if (statusUI != null)
                {
                    statusUI.UpdateAll(c, (int)f);
                }
            }

            yield return null;
        }
        if (healthBar != null)
            healthBar.value = c.currentHealth;
        if (statusUI != null)
        {
            statusUI.UpdateAll(c, c.currentHealth);
        }
        GetComponent<SpriteRenderer>().color = originalColor;
        if (!direction.Equals(new Vector3()))
            GetComponent<Transform>().position = originalPosition;

        for (int i = 0; i < frame3; i++)
        {
            yield return null;
        }
        
        isMoving = false;
    }

    public IEnumerator ReflectedAnimation(int oldHealth, Slider healthBar = null, StatusUI statusUI = null, Vector3 direction = new Vector3())
    {
        isMoving = true;
        int frame = 25;
        Vector3 originalPosition = GetComponent<Transform>().position;
        Color originalColor = GetComponent<SpriteRenderer>().color;

        GameObject g = Instantiate(damageNumber, c.canvas.GetComponent<Transform>());
        g.GetComponent<DamageNumber>().Initialize(oldHealth - c.currentHealth, DamageNumber.DamageType.Reflect);

        for (int i = 0; i < frame; i++)
        {
            if (i < frame / 2)
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(originalColor, new Color(0.7f, 0.7f, 0f, 0.8f), (float)i / frame * 2);
                if (!direction.Equals(new Vector3()))
                    GetComponent<Transform>().position = Vector3.Lerp(originalPosition, originalPosition + 0.2f * direction, (float)i / frame * 2);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(originalColor, new Color(0.7f, 0.7f, 0f, 0.8f), (float)(frame - i) / frame * 2);
                if (!direction.Equals(new Vector3()))
                    GetComponent<Transform>().position = Vector3.Lerp(originalPosition, originalPosition + 0.2f * direction, (float)(frame - i) / frame * 2);
            }

            float f = Mathf.Lerp(c.currentHealth, oldHealth, Mathf.Pow(1 - ((float)i / frame), 2f));
            if (healthBar != null)
                healthBar.value = f;
            if (statusUI != null)
            {
                statusUI.UpdateAll(c, (int)f);
            }

            yield return null;
        }
        if (healthBar != null)
            healthBar.value = c.currentHealth;
        if (statusUI != null)
        {
            statusUI.UpdateAll(c, c.currentHealth);
        }
        GetComponent<SpriteRenderer>().color = originalColor;
        if (!direction.Equals(new Vector3()))
            GetComponent<Transform>().position = originalPosition;

        isMoving = false;
    }

    public IEnumerator RegeneratedAnimation(int oldHealth, Slider healthBar = null, StatusUI statusUI = null)
    {
        isMoving = true;
        int frame = 6;

        if (c.currentHealth == oldHealth)
        {
            isMoving = false;

            // 여기서도 코루틴이 종료될 수 있음에 주의!
            yield break;
        }

        GameObject g = Instantiate(damageNumber, c.canvas.GetComponent<Transform>());
        g.GetComponent<DamageNumber>().Initialize(c.currentHealth - oldHealth, DamageNumber.DamageType.Heal);

        for (int i = 0; i < frame; i++)
        {
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
        isMoving = false;
    }

    public virtual void Death()
    {

    }
}
