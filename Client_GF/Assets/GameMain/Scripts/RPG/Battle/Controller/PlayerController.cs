/****************************************************
    文件：PlayerController.cs
	
    
    
	功能：主角表现实体角色控制器类
*****************************************************/

using UnityEngine;

public class PlayerController : Controller {
    public GameObject daggerskill1fx;
    public GameObject daggerskill2fx;
    public GameObject daggerskill3fx;

    public GameObject daggeratk1fx;
    public GameObject daggeratk2fx;
    public GameObject daggeratk3fx;
    public GameObject daggeratk4fx;
    public GameObject daggeratk5fx;


    private Vector3 camOffset;

    private float targetBlend;
    private float currentBlend;
    protected override void OnShow(object userData) {
        base.OnShow(userData);
        daggerskill1fx = FindGo("Bip_master/dagger_skill1");
        daggerskill2fx = FindGo("Bip_master/dagger_skill2");
        daggerskill3fx = FindGo("Bip_master/dagger_skill3");
        daggeratk1fx = FindGo("Bip_master/dagger_atk1");
        daggeratk2fx = FindGo("Bip_master/dagger_atk2");
        daggeratk3fx = FindGo("Bip_master/dagger_atk3");
        daggeratk4fx = FindGo("Bip_master/dagger_atk4");
        daggeratk5fx = FindGo("Bip_master/dagger_atk5");
        if (FindGo("hpRoot")) {
            hpRoot = FindGo("hpRoot").transform;
        }
    }

    public override void Init() {
        base.Init();

        camTrans = Camera.main.transform;
        camOffset = transform.position - camTrans.position;
        fxDic.Clear();
        if (daggerskill1fx != null) {
            fxDic.Add(daggerskill1fx.name, daggerskill1fx);
        }
        if (daggeratk2fx != null) {
            fxDic.Add(daggerskill2fx.name, daggerskill2fx);
        }
        if (daggeratk3fx != null) {
            fxDic.Add(daggerskill3fx.name, daggerskill3fx);
        }

        if (daggeratk1fx != null) {
            fxDic.Add(daggeratk1fx.name, daggeratk1fx);
        }
        if (daggeratk2fx != null) {
            fxDic.Add(daggeratk2fx.name, daggeratk2fx);
        }
        if (daggeratk3fx != null) {
            fxDic.Add(daggeratk3fx.name, daggeratk3fx);
        }
        if (daggeratk4fx != null) {
            fxDic.Add(daggeratk4fx.name, daggeratk4fx);
        }
        if (daggeratk5fx != null) {
            fxDic.Add(daggeratk5fx.name, daggeratk5fx);
        }
    }

    private void Update() {
        #region Input
        /*
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector2 _dir = new Vector2(h, v).normalized;
        if (_dir != Vector2.zero) {
            Dir = _dir;
            SetBlend(Constants.BlendMove);
        }
        else {
            Dir = Vector2.zero;
            SetBlend(Constants.BlendIdle);
        }
        */
        #endregion

        if (currentBlend != targetBlend) {
            UpdateMixBlend();
        }

        if (isMove) {
            //设置方向
            SetDir();
            //产生移动
            SetMove();
            //相机跟随
            SetCam();
        }

        if (skillMove) {
            SetSkillMove();
            //相机跟随
            SetCam();
        }
    }
    /// <summary>
    ///输入角度加相机自身旋转
    /// </summary>
    private void SetDir() {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1)) + camTrans.eulerAngles.y;
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }

    private void SetMove() {
        PlayerCtrl.Move(transform.forward * Time.deltaTime * Constants.PlayerMoveSpeed);
    }
    /// <summary>
    /// 技能伴随移动(通过配置)
    /// </summary>
    private void SetSkillMove() {
        PlayerCtrl.Move(transform.forward * Time.deltaTime * skillMoveSpeed);
    }
    /// <summary>
    /// 当自动寻路时由主城调用
    /// 当有输入时由玩家控制：1.移动时调用2.攻击时调用3.动画加速时不调用
    /// </summary>
    public void SetCam() {
        if (camTrans != null) {
            camTrans.position = transform.position - camOffset;
        }
    }

    private void UpdateMixBlend() {
        if (Mathf.Abs(currentBlend - targetBlend) < Constants.AccelerSpeed * Time.deltaTime) {
            currentBlend = targetBlend;
        }
        else if (currentBlend > targetBlend) {
            currentBlend -= Constants.AccelerSpeed * Time.deltaTime;
        }
        else {
            currentBlend += Constants.AccelerSpeed * Time.deltaTime;
        }
        Animator.SetFloat("Blend", currentBlend);
    }

    //////////////////////////////////////////////////////////////////////////
    public override void SetBlend(float blend) {
        targetBlend = blend;
    }

    public override void SetFX(string name, float destroy) {
        GameObject go;
        if (fxDic.TryGetValue(name, out go)) {
            go.SetActive(true);
            GameEntry.Timer.AddTimeTask((int tid) => {
                go.SetActive(false);
            }, destroy);
        }
    }
}