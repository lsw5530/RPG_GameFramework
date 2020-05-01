using GameFramework;
using GameFramework.Event;
using StarForce;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

public class ProcedureUpdateResource : ProcedureBase {
    private bool m_UpdateAllComplete = false;
    private int m_UpdateCount = 0;
    private long m_UpdateTotalZipLength = 0L;
    private int m_UpdateSuccessCount = 0;
    private List<UpdateLengthData> m_UpdateLengthData = new List<UpdateLengthData>();
    private UpdateResourceForm m_UpdateResourceForm = null;

    public override bool UseNativeDialog {
        get {
            return true;
        }
    }

    protected override void OnEnter(ProcedureOwner procedureOwner) {
        base.OnEnter(procedureOwner);

        m_UpdateAllComplete = false;
        m_UpdateCount = 0;
        m_UpdateTotalZipLength = 0L;
        m_UpdateSuccessCount = 0;
        m_UpdateLengthData.Clear();
        m_UpdateResourceForm = null;

        GameEntry.Event.Subscribe(ResourceUpdateStartEventArgs.EventId, OnResourceUpdateStart);
        GameEntry.Event.Subscribe(ResourceUpdateChangedEventArgs.EventId, OnResourceUpdateChanged);
        GameEntry.Event.Subscribe(ResourceUpdateSuccessEventArgs.EventId, OnResourceUpdateSuccess);
        GameEntry.Event.Subscribe(ResourceUpdateFailureEventArgs.EventId, OnResourceUpdateFailure);

        GameEntry.Resource.CheckResources(OnCheckResourcesComplete);
    }

    protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown) {
        if (m_UpdateResourceForm != null) {
            Object.Destroy(m_UpdateResourceForm.gameObject);
            m_UpdateResourceForm = null;
        }

        GameEntry.Event.Unsubscribe(ResourceUpdateStartEventArgs.EventId, OnResourceUpdateStart);
        GameEntry.Event.Unsubscribe(ResourceUpdateChangedEventArgs.EventId, OnResourceUpdateChanged);
        GameEntry.Event.Unsubscribe(ResourceUpdateSuccessEventArgs.EventId, OnResourceUpdateSuccess);
        GameEntry.Event.Unsubscribe(ResourceUpdateFailureEventArgs.EventId, OnResourceUpdateFailure);

        base.OnLeave(procedureOwner, isShutdown);
    }

    protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds) {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

        if (!m_UpdateAllComplete) {
            return;
        }

        ChangeState<ProcedurePreload>(procedureOwner);
    }

    private void StartUpdateResources(object userData) {
        if (m_UpdateResourceForm == null) {
            m_UpdateResourceForm = Object.Instantiate(GameEntry.BuiltinData.UpdateResourceFormTemplate);
        }

        Log.Info("Start update resource group 'Base' ...");
        GameEntry.Resource.UpdateResources("Base", OnUpdateResourcesComplete);
    }

    private void ProcessUpdateResourcesComplete() {
        m_UpdateAllComplete = true;
    }

    private void RefreshProgress() {
        long currentTotalUpdateLength = 0L;
        for (int i = 0; i < m_UpdateLengthData.Count; i++) {
            currentTotalUpdateLength += m_UpdateLengthData[i].Length;
        }

        float progressTotal = (float)currentTotalUpdateLength / m_UpdateTotalZipLength;
        string descriptionText = GameEntry.Localization.GetString("UpdateResource.Tips", m_UpdateSuccessCount.ToString(), m_UpdateCount.ToString(), GetLengthString(currentTotalUpdateLength), GetLengthString(m_UpdateTotalZipLength), progressTotal, GetLengthString((int)GameEntry.Download.CurrentSpeed));
        m_UpdateResourceForm.SetProgress(progressTotal, descriptionText);
    }

    private string GetLengthString(long length) {
        if (length < 1024) {
            return string.Format("{0} Bytes", length.ToString());
        }

        if (length < 1024 * 1024) {
            return string.Format("{0} KB", (length / 1024f).ToString("F2"));
        }

        if (length < 1024 * 1024 * 1024) {
            return string.Format("{0} MB", (length / 1024f / 1024f).ToString("F2"));
        }

        return string.Format("{0} GB", (length / 1024f / 1024f / 1024f).ToString("F2"));
    }

    private void OnCheckResourcesComplete(int removedCount, int updateCount, long updateTotalLength, long updateTotalZipLength) {
        Log.Info("Check resources complete, '{0}' resources need to update, zip length is '{1}', unzip length is '{2}'.", updateCount.ToString(), updateTotalZipLength.ToString(), updateTotalLength.ToString());

        m_UpdateCount = updateCount;
        m_UpdateTotalZipLength = updateTotalZipLength;
        if (updateCount <= 0) {
            ProcessUpdateResourcesComplete();
            return;
        }

        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork) {
            GameEntry.UI.OpenDialog(new DialogParams {
                Mode = 2,
                Title = GameEntry.Localization.GetString("UpdateResourceViaCarrierDataNetwork.Title"),
                Message = GameEntry.Localization.GetString("UpdateResourceViaCarrierDataNetwork.Message"),
                ConfirmText = GameEntry.Localization.GetString("UpdateResourceViaCarrierDataNetwork.UpdateButton"),
                OnClickConfirm = StartUpdateResources,
                CancelText = GameEntry.Localization.GetString("UpdateResourceViaCarrierDataNetwork.QuitButton"),
                OnClickCancel = delegate (object userData) { UnityGameFramework.Runtime.GameEntry.Shutdown(ShutdownType.Quit); },
            });

            return;
        }

        StartUpdateResources(null);
    }

    private void OnUpdateResourcesComplete(GameFramework.Resource.IResourceGroup resourceGroup, bool result) {
        if (resourceGroup.Name == "Base") {
            if (result) {
                Log.Info("Update resource group 'Base' complete with no errors.");
                Log.Info("Start update resource group 'Music' ...");
                GameEntry.Resource.UpdateResources("Music", OnUpdateResourcesComplete);
            }
            else {
                Log.Error("Update resource group 'Base' complete with errors.");
            }
        }
        else if (resourceGroup.Name == "Music") {
            if (result) {
                Log.Info("Update resource group 'Music' complete with no errors.");
                Log.Info("Start update other resources ...");
                GameEntry.Resource.UpdateResources(OnUpdateResourcesComplete);
            }
            else {
                Log.Error("Update resource group 'Music' complete with errors.");
            }
        }
        else // resourceGroup.Name == string.Empty
        {
            if (result) {
                Log.Info("Update other resources complete with no errors.");
                ProcessUpdateResourcesComplete();
            }
            else {
                Log.Error("Update other resources complete with errors.");
            }
        }
    }

    private void OnResourceUpdateStart(object sender, GameEventArgs e) {
        ResourceUpdateStartEventArgs ne = (ResourceUpdateStartEventArgs)e;

        for (int i = 0; i < m_UpdateLengthData.Count; i++) {
            if (m_UpdateLengthData[i].Name == ne.Name) {
                Log.Warning("Update resource '{0}' is invalid.", ne.Name);
                m_UpdateLengthData[i].Length = 0;
                RefreshProgress();
                return;
            }
        }

        m_UpdateLengthData.Add(new UpdateLengthData(ne.Name));
    }

    private void OnResourceUpdateChanged(object sender, GameEventArgs e) {
        ResourceUpdateChangedEventArgs ne = (ResourceUpdateChangedEventArgs)e;

        for (int i = 0; i < m_UpdateLengthData.Count; i++) {
            if (m_UpdateLengthData[i].Name == ne.Name) {
                m_UpdateLengthData[i].Length = ne.CurrentLength;
                RefreshProgress();
                return;
            }
        }

        Log.Warning("Update resource '{0}' is invalid.", ne.Name);
    }

    private void OnResourceUpdateSuccess(object sender, GameEventArgs e) {
        ResourceUpdateSuccessEventArgs ne = (ResourceUpdateSuccessEventArgs)e;
        Log.Info("Update resource '{0}' success.", ne.Name);

        for (int i = 0; i < m_UpdateLengthData.Count; i++) {
            if (m_UpdateLengthData[i].Name == ne.Name) {
                m_UpdateLengthData[i].Length = ne.ZipLength;
                m_UpdateSuccessCount++;
                RefreshProgress();
                return;
            }
        }

        Log.Warning("Update resource '{0}' is invalid.", ne.Name);
    }

    private void OnResourceUpdateFailure(object sender, GameEventArgs e) {
        ResourceUpdateFailureEventArgs ne = (ResourceUpdateFailureEventArgs)e;
        if (ne.RetryCount >= ne.TotalRetryCount) {
            Log.Error("Update resource '{0}' failure from '{1}' with error message '{2}', retry count '{3}'.", ne.Name, ne.DownloadUri, ne.ErrorMessage, ne.RetryCount.ToString());
            return;
        }
        else {
            Log.Info("Update resource '{0}' failure from '{1}' with error message '{2}', retry count '{3}'.", ne.Name, ne.DownloadUri, ne.ErrorMessage, ne.RetryCount.ToString());
        }

        for (int i = 0; i < m_UpdateLengthData.Count; i++) {
            if (m_UpdateLengthData[i].Name == ne.Name) {
                m_UpdateLengthData.Remove(m_UpdateLengthData[i]);
                RefreshProgress();
                return;
            }
        }

        Log.Warning("Update resource '{0}' is invalid.", ne.Name);
    }

    private class UpdateLengthData {
        private readonly string m_Name;

        public UpdateLengthData(string name) {
            m_Name = name;
        }

        public string Name {
            get {
                return m_Name;
            }
        }

        public int Length {
            get;
            set;
        }
    }
}
