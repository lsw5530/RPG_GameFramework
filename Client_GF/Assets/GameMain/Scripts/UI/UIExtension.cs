//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using GameFramework.DataTable;
using GameFramework.UI;
using StarForce;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

public static class UIExtension {
    public static IEnumerator FadeToAlpha(this CanvasGroup canvasGroup, float alpha, float duration) {
        float time = 0f;
        float originalAlpha = canvasGroup.alpha;
        while (time < duration) {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(originalAlpha, alpha, time / duration);
            yield return new WaitForEndOfFrame();
        }

        canvasGroup.alpha = alpha;
    }

    public static IEnumerator SmoothValue(this Slider slider, float value, float duration) {
        float time = 0f;
        float originalValue = slider.value;
        while (time < duration) {
            time += Time.deltaTime;
            slider.value = Mathf.Lerp(originalValue, value, time / duration);
            yield return new WaitForEndOfFrame();
        }

        slider.value = value;
    }

    public static bool HasUIForm(this UIComponent uiComponent, UIFormId uiFormId, string uiGroupName = null) {
        return uiComponent.HasUIForm((int)uiFormId, uiGroupName);
    }

    public static bool HasUIForm(this UIComponent uiComponent, int uiFormId, string uiGroupName = null) {
        IDataTable<DRUIForm> dtUIForm = GameEntry.DataTable.GetDataTable<DRUIForm>();
        DRUIForm drUIForm = dtUIForm.GetDataRow(uiFormId);
        if (drUIForm == null) {
            return false;
        }

        string assetName = AssetUtility.GetUIFormAsset(drUIForm.AssetName);
        if (string.IsNullOrEmpty(uiGroupName)) {
            return uiComponent.HasUIForm(assetName);
        }

        IUIGroup uiGroup = uiComponent.GetUIGroup(uiGroupName);
        if (uiGroup == null) {
            return false;
        }

        return uiGroup.HasUIForm(assetName);
    }

    public static UGuiForm GetUIForm(this UIComponent uiComponent, UIFormId uiFormId, string uiGroupName = null) {
        return uiComponent.GetUIForm((int)uiFormId, uiGroupName);
    }

    public static UGuiForm GetUIForm(this UIComponent uiComponent, int uiFormId, string uiGroupName = null) {
        IDataTable<DRUIForm> dtUIForm = GameEntry.DataTable.GetDataTable<DRUIForm>();
        DRUIForm drUIForm = dtUIForm.GetDataRow(uiFormId);
        if (drUIForm == null) {
            return null;
        }

        string assetName = AssetUtility.GetUIFormAsset(drUIForm.AssetName);
        UIForm uiForm = null;
        if (string.IsNullOrEmpty(uiGroupName)) {
            uiForm = uiComponent.GetUIForm(assetName);
            if (uiForm == null) {
                return null;
            }

            return (UGuiForm)uiForm.Logic;
        }

        IUIGroup uiGroup = uiComponent.GetUIGroup(uiGroupName);
        if (uiGroup == null) {
            return null;
        }

        uiForm = (UIForm)uiGroup.GetUIForm(assetName);
        if (uiForm == null) {
            return null;
        }

        return (UGuiForm)uiForm.Logic;
    }

    public static void CloseUIForm(this UIComponent uiComponent, UGuiForm uiForm) {
        if (uiForm != null)
            uiComponent.CloseUIForm(uiForm.UIForm);
    }

    public static int? OpenUIForm(this UIComponent uiComponent, UIFormId uiFormId, object userData = null) {
        return uiComponent.OpenUIForm((int)uiFormId, userData);
    }

    public static int? OpenUIForm(this UIComponent uiComponent, int uiFormId, object userData = null) {
        IDataTable<DRUIForm> dtUIForm = GameEntry.DataTable.GetDataTable<DRUIForm>();
        DRUIForm drUIForm = dtUIForm.GetDataRow(uiFormId);
        if (drUIForm == null) {
            Log.Warning("Can not load UI form '{0}' from data table.", uiFormId.ToString());
            return null;
        }

        string assetName = AssetUtility.GetUIFormAsset(drUIForm.AssetName);
        if (!drUIForm.AllowMultiInstance) {
            if (uiComponent.IsLoadingUIForm(assetName)) {
                return null;
            }

            if (uiComponent.HasUIForm(assetName)) {
                return null;
            }
        }

        return uiComponent.OpenUIForm(assetName, drUIForm.UIGroupName, Constant.AssetPriority.UIFormAsset, drUIForm.PauseCoveredUIForm, userData);
    }

    public static void OpenDialog(this UIComponent uiComponent, DialogParams dialogParams) {
        if (((ProcedureBase)GameEntry.Procedure.CurrentProcedure).UseNativeDialog) {
            OpenNativeDialog(dialogParams);
        }
        else {
            uiComponent.OpenUIForm(UIFormId.DialogForm, dialogParams);
        }
    }

    private static void OpenNativeDialog(DialogParams dialogParams) {
        // TODO：这里应该弹出原生对话框，先简化实现为直接按确认按钮
        if (dialogParams.OnClickConfirm != null) {
            dialogParams.OnClickConfirm(dialogParams.UserData);
        }
    }

    public static void CloseUIForm(this UIComponent uiComponent, UIFormId uiFormId) {
        UGuiForm form = uiComponent.GetUIForm(uiFormId);
        if (form == null) {
            return;
        }
        uiComponent.CloseUIForm(form);
    }
    public static void AddTips(this UIComponent uiComponent, string tip) {
        if (uiComponent.HasUIForm(UIFormId.DynamicForm)) {
            AddTipEventArgs eventArgs = ReferencePool.Acquire<AddTipEventArgs>();
            eventArgs.Fill(tip);
            GameEntry.Event.Fire(uiComponent, eventArgs);
        }
        else {
            uiComponent.OpenUIForm(UIFormId.DynamicForm);
        }
    }
    public static void RmvAllHpItemInfo(this UIComponent uiComponent) {
        if (uiComponent.HasUIForm(UIFormId.DynamicForm)) {
            RmvAllHpItemInfoEventArgs eventArgs = ReferencePool.Acquire<RmvAllHpItemInfoEventArgs>();
            GameEntry.Event.Fire(uiComponent, eventArgs);
        }
        else {
            uiComponent.OpenUIForm(UIFormId.DynamicForm);
        }
    }
    public static void SetDodge(this UIComponent uiComponent, string name) {
        if (uiComponent.HasUIForm(UIFormId.DynamicForm)) {
            SetDodgeEventArgs eventArgs = ReferencePool.Acquire<SetDodgeEventArgs>();
            eventArgs.Fill(name);
            GameEntry.Event.Fire(uiComponent, eventArgs);
        }
        else {
            uiComponent.OpenUIForm(UIFormId.DynamicForm);
        }
    }
    public static void SetCritical(this UIComponent uiComponent, string name, int critical) {
        if (uiComponent.HasUIForm(UIFormId.DynamicForm)) {
            SetCriticalEventArgs eventArgs = ReferencePool.Acquire<SetCriticalEventArgs>();
            eventArgs.Fill(name, critical);
            GameEntry.Event.Fire(uiComponent, eventArgs);
        }
        else {
            uiComponent.OpenUIForm(UIFormId.DynamicForm);
        }
    }
    public static void SetHurt(this UIComponent uiComponent, string name, int hurt) {
        if (uiComponent.HasUIForm(UIFormId.DynamicForm)) {
            SetCriticalEventArgs eventArgs = ReferencePool.Acquire<SetCriticalEventArgs>();
            eventArgs.Fill(name,hurt);
            GameEntry.Event.Fire(uiComponent, eventArgs);
        }
        else {
            uiComponent.OpenUIForm(UIFormId.DynamicForm);
        }
    }
    public static void SetHPVal(this UIComponent uiComponent, string name, int oldval,int newval) {
        if (uiComponent.HasUIForm(UIFormId.DynamicForm)) {
            SetHPValEventArgs eventArgs = ReferencePool.Acquire<SetHPValEventArgs>();
            eventArgs.Fill(name, oldval, newval);
            GameEntry.Event.Fire(uiComponent, eventArgs);
        }
        else {
            uiComponent.OpenUIForm(UIFormId.DynamicForm);
        }
    }
    public static void SetSelfDodge(this UIComponent uiComponent) {
        if (uiComponent.HasUIForm(UIFormId.DynamicForm)) {
            SetSelfDodgeEventArgs eventArgs = ReferencePool.Acquire<SetSelfDodgeEventArgs>();
            GameEntry.Event.Fire(uiComponent, eventArgs);
        }
        else {
            uiComponent.OpenUIForm(UIFormId.DynamicForm);
        }
    }
    public static void AddHpItemInfo(this UIComponent uiComponent,string name,Transform tf,int hp) {
        if (uiComponent.HasUIForm(UIFormId.DynamicForm)) {
            AddHpItemInfoEventArgs eventArgs = ReferencePool.Acquire<AddHpItemInfoEventArgs>();
            eventArgs.Fill(name, tf, hp);
            GameEntry.Event.Fire(uiComponent, eventArgs);
        }
        else {
            uiComponent.OpenUIForm(UIFormId.DynamicForm);
        }
    }
    public static void RmvHpItemInfo(this UIComponent uiComponent, string key) {
        if (uiComponent.HasUIForm(UIFormId.DynamicForm)) {
            RmvHpItemInfoEventArgs eventArgs = ReferencePool.Acquire<RmvHpItemInfoEventArgs>();
            eventArgs.Fill(key);
            GameEntry.Event.Fire(uiComponent, eventArgs);
        }
        else {
            uiComponent.OpenUIForm(UIFormId.DynamicForm);
        }
    }
}
