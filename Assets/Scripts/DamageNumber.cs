using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumber : MonoBehaviour {

    public List<GameObject> exponents;       // (10 ^ (index)) 이미지 그룹의 parent

    public List<GameObject> criticalMarks;  // (10 ^ (index)) 이미지 그룹의 크리티컬 마크

    public List<GameObject> onesNumbers;
    public List<GameObject> tensNumbers;
    public List<GameObject> hundredsNumbers;

    private List<List<GameObject>> numbers;  // (10 ^ i) 이미지 그룹의 (10 ^ j)의 자리 숫자 이미지

    private bool isInitialized;
    private int exp;

	// Use this for initialization
	void Awake () {
        numbers = new List<List<GameObject>>
        {
            onesNumbers,
            tensNumbers,
            hundredsNumbers
        };
        foreach (GameObject g in exponents)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in criticalMarks)
        {
            g.SetActive(false);
        }
        isInitialized = false;
    }
	
	public void Initialize(int damage, bool isCritical)
    {
        if (isInitialized) return;
        isInitialized = true;
        
        string critical = "N";
        if (isCritical) critical = "C";

        if (damage < 0)
        {
            // min damage
            damage = 0;
        }
        if (damage >= Mathf.Pow(10, exponents.Count))
        {
            // max damage
            damage = Mathf.RoundToInt(Mathf.Pow(10, exponents.Count)) - 1;
        }

        if (damage == 0) {
            exp = 0;
            numbers[0][0].GetComponent<Image>().sprite = 
                Resources.Load("DamageNumber/" + critical + 0, typeof(Sprite)) as Sprite;
            numbers[0][0].SetActive(true);
            exponents[0].SetActive(true);
        }
        else {
            exp = (int)(Mathf.Log10(damage));
            for (int i = 0; i <= exp; i++)
            {
                GameObject g = numbers[exp][i];
                int n = (damage % (int)Mathf.Pow(10, i + 1)) / (int)Mathf.Pow(10, i);

                g.GetComponent<Image>().sprite =
                    Resources.Load("DamageNumber/" + critical + n, typeof(Sprite)) as Sprite;
                g.SetActive(true);
            }
            exponents[exp].SetActive(true);
        }

        if (isCritical)
        {
            criticalMarks[exp].SetActive(true);
        }

        StartCoroutine(Disappear());
    }

    IEnumerator Disappear()
    {
        Vector3 originalPosition = GetComponent<RectTransform>().localPosition;
        Color originalColor = new Color(1f, 1f, 1f, 1f);
        float frame = 32f;
        for (int i = 0; i < frame; i++)
        {
            GetComponent<RectTransform>().localPosition = Vector3.Lerp(originalPosition, originalPosition + new Vector3(0f, 15f, 0f), i / frame);
            foreach (GameObject g in numbers[exp])
            {
                g.GetComponent<Image>().color = Color.Lerp(originalColor, ColorManager.ChangeAlpha(originalColor, 0f), i / frame);
            }
            criticalMarks[exp].GetComponent<Image>().color = Color.Lerp(originalColor, ColorManager.ChangeAlpha(originalColor, 0f), i / frame);
            yield return null;
        }
        Destroy(gameObject);
    }
}
