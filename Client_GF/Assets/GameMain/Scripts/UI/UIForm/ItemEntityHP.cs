/****************************************************
    文件：ItemEntityHP.cs
	
    
    	功能：血条物体
*****************************************************/

using UnityEngine;
using UnityEngine.UI;


public class ItemEntityHP : MonoBehaviour {
    #region UI Define
    public Image imgHPGray;
    public Image imgHPRed;

    public Animation criticalAni;
    public Text txtCritical;

    public Animation dodgeAni;
    public Text txtDodge;

    public Animation hpAni;
    public Text txtHp;
    #endregion

    private RectTransform rect;
    private Transform rootTrans;
    private int hpVal;
    private float scaleRate = 1.0F * Constants.ScreenStandardHeight / Screen.height;

    private void Update() {
        /*
        if (Input.GetKeyUp(KeyCode.Space)) {
            SetCritical(696);
            SetHurt(336);
        }

        if (Input.GetKeyUp(KeyCode.A)) {
            SetDodge();
        }
        */
        Vector3 screenPos = Camera.main.WorldToScreenPoint(rootTrans.position);
        rect.anchoredPosition = screenPos * scaleRate;

        UpdateMixBlend();
        imgHPGray.fillAmount = currentPrg;
    }

    private void UpdateMixBlend() {
        if (Mathf.Abs(currentPrg - targetPrg) < Constants.AccelerHPSpeed * Time.deltaTime) {
            currentPrg = targetPrg;
        }
        else if (currentPrg > targetPrg) {
            currentPrg -= Constants.AccelerHPSpeed * Time.deltaTime;
        }
        else {
            currentPrg += Constants.AccelerHPSpeed * Time.deltaTime;
        }
    }

    public void InitItemInfo(Transform trans, int hp) {
        rect = transform.GetComponent<RectTransform>();
        rootTrans = trans;
        hpVal = hp;
        imgHPGray.fillAmount = 1;
        imgHPRed.fillAmount = 1;
    }

    public void SetCritical(int critical) {
        criticalAni.Stop();
        txtCritical.text = "暴击 " + critical;
        criticalAni.Play();
    }

    public void SetDodge() {
        dodgeAni.Stop();
        txtDodge.text = "闪避";
        dodgeAni.Play();
    }

    public void SetHurt(int hurt) {
        hpAni.Stop();
        txtHp.text = "-" + hurt;
        hpAni.Play();
    }

    private float currentPrg;
    private float targetPrg;

    public void SetHPVal(int oldVal, int newVal) {
        currentPrg = oldVal * 1.0f / hpVal;
        targetPrg = newVal * 1.0f / hpVal;
        imgHPRed.fillAmount = targetPrg;
    }
}