/****************************************************
    文件：ChatWnd.cs
	
    
    
	功能：聊天界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameMessage;
using GameFramework.Event;
using System;

public class ChatWnd : WindowRoot {
    public InputField iptChat;
    public Text txtChat;
    public Image imgWorld;
    public Image imgGuild;
    public Image imgFriend;

    private int chatType;
    private List<string> chatLst = new List<string>();

    protected override void OnOpen(object userData) {
        base.OnOpen(userData);
        GameEntry.Event.Subscribe(RefreshChatUIEventArgs.EventId, AddChatMsg);
        chatType = 0;

        RefreshUI();
    }
    protected override void OnClose(bool isShutdown, object userData) {
        base.OnClose(isShutdown, userData);
        GameEntry.Event.Unsubscribe(RefreshChatUIEventArgs.EventId, AddChatMsg);
    }

    private void AddChatMsg(object sender, GameEventArgs e) {
        RefreshChatUIEventArgs ne = e as RefreshChatUIEventArgs;
        chatLst.Add(Constants.Color(ne.name + "：", TxtColor.Blue) + ne.chat);
        if (chatLst.Count > 12) {
            chatLst.RemoveAt(0);
        }
        if (GetWndState()) {
            RefreshUI();
        }
    }

    private void RefreshUI() {
        if (chatType == 0) {
            string chatMsg = "";
            for (int i = 0; i < chatLst.Count; i++) {
                chatMsg += chatLst[i] + "\n";
            }
            SetText(txtChat, chatMsg);

            SetSprite(imgWorld, "btntype1");
            SetSprite(imgGuild, "btntype2");
            SetSprite(imgFriend, "btntype2");
        }
        else if (chatType == 1) {
            SetText(txtChat, "尚未加入公会");
            SetSprite(imgWorld, "btntype2");
            SetSprite(imgGuild, "btntype1");
            SetSprite(imgFriend, "btntype2");
        }
        else if (chatType == 2) {
            SetText(txtChat, "暂无好友信息");
            SetSprite(imgWorld, "btntype2");
            SetSprite(imgGuild, "btntype2");
            SetSprite(imgFriend, "btntype1");
        }
    }

    private bool canSend = true;
    public void ClickSendBtn() {
        if (!canSend) {
            GameEntry.UI.AddTips("聊天消息每5秒钟才能发送一条");
            return;
        }

        if (iptChat.text != null && iptChat.text != "" && iptChat.text != " ") {
            if (iptChat.text.Length > 12) {
                GameEntry.UI.AddTips("输入信息不能超过12个字");
            }
            else {
                //发送网络消息到服务器
                CSPacketBase msg = new SndChat {
                    Chat = iptChat.text
                };
                iptChat.text = "";
                GameEntry.Net.SendMsg(msg);
                canSend = false;

                GameEntry.Timer.AddTimeTask((int tid) => {
                    canSend = true;
                }, 5, PETimeUnit.Second);
            }
        }
        else {
            GameEntry.UI.AddTips("尚未输入聊天信息");
        }
    }
    public void ClickWorldBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIClickBtn);
        chatType = 0;
        RefreshUI();
    }
    public void ClickGuildBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIClickBtn);
        chatType = 1;
        RefreshUI();
    }
    public void ClickFriendBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIClickBtn);
        chatType = 2;
        RefreshUI();
    }
    public void ClickCloseBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIClickBtn);
        chatType = 0;
        Close(true);
    }
}