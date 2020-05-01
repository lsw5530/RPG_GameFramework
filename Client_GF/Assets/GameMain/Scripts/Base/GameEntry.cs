//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using UnityEngine;
using UnityGameFramework.Runtime;

/// <summary>
/// 游戏入口。
/// </summary>
public partial class GameEntry : MonoBehaviour {
    public static BuiltinDataComponent BuiltinData {
        get;
        private set;
    }
    private void Start() {
        InitBuiltinComponents();
        InitCustomComponents();
    }
    public void OnDestroy() {
        CloseCustomComponents();
    }
}
