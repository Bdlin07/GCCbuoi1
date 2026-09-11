using System.Collections;
using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    public GameObject ammoPanel;
    public TMP_Text ammoText;
    Coroutine hideCoroutine;

    void OnEnable()
    {
        PlayerShoot.AmmoChanged += ShowAmmo;
    }

    void OnDisable()
    {
        PlayerShoot.AmmoChanged -= ShowAmmo;
    }

    void Start()
    {
        ammoPanel.SetActive(false);
    }

    void ShowAmmo(int ammo)
    {
        ammoPanel.SetActive(true);
        ammoText.text = "Dan con lai: " + ammo;

        if(hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideUI());
    }

    IEnumerator HideUI()
    {
        yield return new WaitForSeconds(2f);
        ammoPanel.SetActive(false);
    }
}
