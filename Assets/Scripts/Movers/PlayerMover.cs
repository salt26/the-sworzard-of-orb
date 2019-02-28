using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMover : Mover {

    GameManager gm;
    Transform t;
    
    private const float bonusDamage = 1.5f;     // 돌진 시 곱해지는 추가 대미지
    
    public List<Sprite> weaponMarkSprite;
    public List<Sprite> characterSprite;

    public GameObject distanceSprite;

    // Use this for initialization
    void Start () {
        t = GetComponent<Transform>();
        c = GetComponent<Character>();
        gm = GameManager.gm;
        isMoving = false;
	}
	
	void Update () {
		if (gm.Turn == 0 && gm.IsSceneLoaded && !isMoving)
        {
            if (c.hasStuned)
            {
                c.hasStuned = false;
                gm.NextTurn();
                return;
            }
            // TODO 맵을 보고 갈 수 있는 지형인지 확인할 것
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                int x = Mathf.RoundToInt(t.position.x - 1f), y = Mathf.RoundToInt(t.position.y);

                GetComponent<SpriteRenderer>().flipX = false;
                if (gm.map.CanMoveToTile(x, y, true))
                {
                    StartCoroutine(MoveAnimation(new Vector3(-1f, 0f, 0f)));
                }
                else if (gm.map.GetTypeOfTile(x, y) == 0 && gm.map.GetEntityOnTile(x, y) != null)
                {
                    Interaction(new Vector3(-1f, 0f, 0f), false);
                }
                else if (gm.map.GetTypeOfTile(x, y) == 1 && c.EquippedWeapon.Range == 2 &&
                    gm.map.GetEntityOnTile(x - 1, y) != null && typeof(Character).IsAssignableFrom(gm.map.GetEntityOnTile(x - 1, y).GetType()))
                {
                    Interaction(new Vector3(-1f, 0f, 0f), false);
                }
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                int x = Mathf.RoundToInt(t.position.x + 1f), y = Mathf.RoundToInt(t.position.y);

                GetComponent<SpriteRenderer>().flipX = true;
                if (gm.map.CanMoveToTile(x, y, true))
                {
                    StartCoroutine(MoveAnimation(new Vector3(1f, 0f, 0f)));
                }
                else if (gm.map.GetTypeOfTile(x, y) == 0 && gm.map.GetEntityOnTile(x, y) != null)
                {
                    Interaction(new Vector3(1f, 0f, 0f), false);
                }
                else if (gm.map.GetTypeOfTile(x, y) == 1 && c.EquippedWeapon.Range == 2 && 
                    gm.map.GetEntityOnTile(x + 1, y) != null && typeof(Character).IsAssignableFrom(gm.map.GetEntityOnTile(x + 1, y).GetType()))
                {
                    Interaction(new Vector3(1f, 0f, 0f), false);
                }
            }
            else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                int x = Mathf.RoundToInt(t.position.x), y = Mathf.RoundToInt(t.position.y + 1f);

                if (gm.map.CanMoveToTile(x, y, true))
                {
                    StartCoroutine(MoveAnimation(new Vector3(0f, 1f, 0f)));
                }
                else if (gm.map.GetTypeOfTile(x, y) == 0 && gm.map.GetEntityOnTile(x, y) != null)
                {
                    Interaction(new Vector3(0f, 1f, 0f), false);
                }
                else if (gm.map.GetTypeOfTile(x, y) == 1 && c.EquippedWeapon.Range == 2 &&
                    gm.map.GetEntityOnTile(x, y + 1) != null && typeof(Character).IsAssignableFrom(gm.map.GetEntityOnTile(x, y + 1).GetType()))
                {
                    Interaction(new Vector3(0f, 1f, 0f), false);
                }
            }
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                int x = Mathf.RoundToInt(t.position.x), y = Mathf.RoundToInt(t.position.y - 1f);

                if (gm.map.CanMoveToTile(x, y, true))
                {
                    StartCoroutine(MoveAnimation(new Vector3(0f, -1f, 0f)));
                }
                else if (gm.map.GetTypeOfTile(x, y) == 0 && gm.map.GetEntityOnTile(x, y) != null)
                {
                    Interaction(new Vector3(0f, -1f, 0f), false);
                }
                else if (gm.map.GetTypeOfTile(x, y) == 1 && c.EquippedWeapon.Range == 2 &&
                    gm.map.GetEntityOnTile(x, y - 1) != null && typeof(Character).IsAssignableFrom(gm.map.GetEntityOnTile(x, y - 1).GetType()))
                {
                    Interaction(new Vector3(0f, -1f, 0f), false);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                gm.NextTurn();
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                c.ToggleWeapon();
                gm.NextTurn();
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                // TODO 디버그용 오브 생성 코드
                gm.map.AddItemOnTile(Random.Range(122, 125), t.position);
                gm.NextTurn();
            }
        }
        
    }

    IEnumerator MoveAnimation(Vector3 direction)
    {
        isMoving = true;
        int frame = 15;
        Vector3 origin = t.position;
        Vector3 destination = t.position + direction;

        // 애니메이션이 시작하기 전에 이동 판정 완료
        gm.map.SetEntityOnTile(null, origin);
        gm.map.SetEntityOnTile(c, destination);
        for (int i = 0; i < frame; i++)
        {
            t.position = Vector3.Lerp(origin, destination, Mathf.Sqrt(Mathf.Sqrt((float)i / frame)));
            /*
            if (i < frame / 2)
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0.4f, 0.7f, 0.6f, 1f), (float)i / frame * 2);
            else
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0.4f, 0.7f, 0.6f, 1f), (float)(frame - i) / frame * 2);
                */
            yield return null;
        }
        //GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        t.position = destination;
        isMoving = false;
        gm.map.PickUpItemsOnTile(GetComponent<Inventory>(), t.position);
        Interaction(direction, true);
    }

    /// <summary>
    /// 진행 방향(direction)에 있는 대상과 상호작용합니다. 만약 대상이 적이면 공격합니다. isCharge가 true이면 돌진 공격이 적용됩니다.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="isCharge"></param>
    private void Interaction(Vector3 direction, bool isCharge)
    {
        // 적 또는 상호작용 가능한 대상이 진행 방향에 있는지 체크
        Entity e = gm.map.GetEntityOnTile(Mathf.RoundToInt(t.position.x + direction.x), Mathf.RoundToInt(t.position.y + direction.y));
        if (e != null && typeof(Interactable).IsAssignableFrom(e.GetType()))
        {
            // 진행 방향에 상호작용 가능한 대상이 있을 경우
            ((Interactable)e).Interact();
            gm.NextTurn();
        }
        else
        {
            // 사정거리 내의 모든 적 확인
            List<Character> enemies = new List<Character>();
            for (int i = 1; i <= c.EquippedWeapon.Range; i++)
            {
                e = gm.map.GetEntityOnTile(Mathf.RoundToInt(t.position.x + direction.x * i), Mathf.RoundToInt(t.position.y + direction.y * i));
                if (e != null && e.GetType().Equals(typeof(Character)) && ((Character)e).type == Character.Type.Enemy)
                {
                    enemies.Add((Character)e);
                }
            }
            if (enemies.Count > 0)
            {
                // 진행 방향으로 사정거리 내에 적이 있을 경우
                float bonus = bonusDamage;         // 돌진 시 추가 대미지 적용
                if (!isCharge) bonus = 1f;

                for (int i = 1; i <= c.EquippedWeapon.Range; i++)
                {
                    GameObject g = Instantiate(distanceSprite,
                        new Vector3(Mathf.RoundToInt(t.position.x + direction.x * i), Mathf.RoundToInt(t.position.y + direction.y * i), -0.25f), Quaternion.identity);
                    g.GetComponent<SpriteRenderer>().color = new Color(0f, 0.8f, 0f, 1f);
                    g.GetComponent<DistanceSprite>().Disappear(30);
                }
                StartCoroutine(AttackAnimation(direction, enemies, bonus));
            }
            else
            {
                // 진행 방향에 아무것도 없을 경우
                gm.NextTurn();
            }
        }
    }

    // TODO direction은 현재 사용하지 않음.
    IEnumerator AttackAnimation(Vector3 direction, List<Character> enemies, float bonusDamage)
    {
        isMoving = true;
        int frame = 20;     // int이어야 동작함. float이면 동작하지 않음
        Vector3 origin = t.position;
        
        for (int i = 0; i < frame; i++)
        {
            if (i < frame / 3)
            {
                //GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0.9f, 1f, 0.2f, 1f), (float)i / frame * 2);
                if (!direction.Equals(new Vector3()))
                    t.position = Vector3.Lerp(origin, origin + 0.15f * direction, (float)i / frame * 3);
            }
            else
            {
                //GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0.9f, 1f, 0.2f, 1f), (float)(frame - i) / frame * 2);
                if (!direction.Equals(new Vector3()))
                    t.position = Vector3.Lerp(origin, origin + 0.15f * direction, (float)(frame - i) / frame * 3 / 2);
            }

            if (i == frame / 3)
            {
                foreach (Character enemy in enemies)
                {
                    enemy.Damaged(enemy.armor.ComputeDamage(c.EquippedWeapon, bonusDamage), direction, (bonusDamage > 1f));
                    //enemy.Damaged(Mathf.Max(0, (int)(bonusDamage * c.EquippedWeapon.Attack()) - enemy.armor.Defense()));
                    enemy.DamagedWithEffects(c.EquippedWeapon.afterAttackEffect);
                }
                gm.NextTurn();
            }

            yield return null;
        }
        //GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        if (!direction.Equals(new Vector3()))
            t.position = origin;
        isMoving = false;
    }

    public override IEnumerator DamagedAnimation(int oldHealth, Slider healthBar = null, StatusUI statusUI = null, Vector3 direction = new Vector3(), bool isCritical = false)
    {
        isMoving = true;
        float frame = 16, frame2 = 0, frame3 = 10;
        Vector3 originalPosition = t.position;
        Color originalColor = GetComponent<SpriteRenderer>().color;
        DamagedScreen damagedScreen = GameManager.gm.Canvas.GetComponent<UIInfo>().DamagedPanel;
        DamageNumber.DamageType dt = DamageNumber.DamageType.Normal;
        if (isCritical) dt = DamageNumber.DamageType.Critical;

        if (!direction.Equals(new Vector3()))
        {
            GameObject g = Instantiate(damageNumber, c.canvas.GetComponent<Transform>());
            g.GetComponent<DamageNumber>().Initialize(oldHealth - c.currentHealth, dt);
        }

        for (int i = 0; i < frame + frame2; i++)
        {
            if (i < frame / 2)
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(originalColor, new Color(0.7f, 0f, 0f, 0.6f), i / frame * 2);
                if (!direction.Equals(new Vector3()))
                    t.position = Vector3.Lerp(originalPosition, originalPosition + 0.2f * direction, i / frame * 2);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(originalColor, new Color(0.7f, 0f, 0f, 0.6f), (frame + frame2 - i) / (frame / 2 + frame2));
                if (!direction.Equals(new Vector3()))
                    t.position = Vector3.Lerp(originalPosition, originalPosition + 0.2f * direction, (frame + frame2 - i) / (frame / 2 + frame2));
            }

            if (c.currentHealth <= c.MaxHealth / 6)
            {
                damagedScreen.StartEffect(true);
            }
            else if (c.currentHealth <= c.MaxHealth / 3)
            {
                damagedScreen.StartEffect(false);
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
            t.position = originalPosition;

        for (int i = 0; i < frame3; i++)
        {
            yield return null;
        }


        isMoving = false;
    }

    public override void Death()
    {
        // TODO 크기가 2 이상인 개체에 대해, 개체가 차지하고 있던 모든 타일 고려
        gm.map.SetEntityOnTile(null, t.position);
        this.enabled = false;
        gm.restartText.SetActive(true);
    }
}
