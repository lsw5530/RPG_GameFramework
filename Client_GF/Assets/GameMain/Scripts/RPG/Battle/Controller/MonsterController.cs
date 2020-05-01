/****************************************************
	文件：MonsterController.cs
	
	
	
	功能：怪物表现实体角色控制器类
*****************************************************/

using UnityEngine;

public class MonsterController : Controller {
    public MonsterEntityData MonsterEntityData { get; set; }
    protected override void OnShow(object userData) {
        base.OnShow(userData);
        MonsterEntityData = userData as MonsterEntityData;
        hpRoot = FindGo("hpRoot").transform;
    }
    private void Update() {
        //AI逻辑表现
        if (isMove) {
            SetDir();

            SetMove();
        }
    }

    private void SetDir() {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1));
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }

    private void SetMove() {
        PlayerCtrl.Move(transform.forward * Time.deltaTime * Constants.MonsterMoveSpeed);
        //给一个向下的速度，便于在没有apply root时怪物可以落地。Fix Res Error
        PlayerCtrl.Move(Vector3.down * Time.deltaTime * Constants.MonsterMoveSpeed);
    }
}
