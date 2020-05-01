local m_VersionText
local m_NoticeButton
local m_AccountButton
local m_NoticePanel
local m_LoginPanel
local m_RegisterPanel
local m_LoginAccountInput
local m_LoginPasswordInput
local m_RegisterAccountInput
local m_RegisterPasswordInput
local m_AccountRegisterButton
local m_RegisterButton
local m_LoginButton

local m_Params

function OnInit(userData)
	m_VersionText = self.UI:GetChild("tf_Version").asTextField;
	m_NoticeButton = self.UI:GetChild("btn_Notice").asButton;
	m_AccountButton = self.UI:GetChild("btn_Account").asButton;
	m_NoticePanel = self.UI:GetChild("notice").asCom;
	m_LoginPanel = self.UI:GetChild("login").asCom;
	m_RegisterPanel = self.UI:GetChild("register").asCom;
	m_LoginAccountInput = m_LoginPanel:GetChild("ipt_account").asTextInput;
	m_LoginPasswordInput = m_LoginPanel:GetChild("ipt_password").asTextInput;
	m_RegisterAccountInput = m_RegisterPanel:GetChild("ipt_account").asTextInput;
	m_RegisterPasswordInput = m_RegisterPanel:GetChild("ipt_password").asTextInput;
	m_AccountRegisterButton = m_RegisterPanel:GetChild("btn_register").asButton;
	m_RegisterButton = m_LoginPanel:GetChild("btn_rigister").asButton;
	m_LoginButton = m_LoginPanel:GetChild("btn_login").asButton;

	m_NoticePanel.visible = false;
    m_LoginPanel.visible = false;
	m_RegisterPanel.visible = false;
end 

function OnOpen(userData)
    if userData == nil then
		Log.Error("userData is invalid.")
    	return
    end

	m_Params = userData
	
	m_VersionText.text = userData.Version;
	m_NoticeButton.onClick:Add(
	    function()
	    	m_NoticePanel.visible = not m_NoticePanel.visible;
	    end)

    m_AccountButton.onClick:Add(
	    function()
	    	m_LoginPanel.visible = not m_LoginPanel.visible;
		end)

	m_RegisterButton.onClick:Add(
		function()
			m_RegisterPanel.visible = true; 
		end)

    m_LoginButton.onClick:Add(OnLoginClick)
    m_AccountRegisterButton.onClick:Add(OnRegisterClick)
end

function OnClose(userData)
	m_VersionText.text = string.Empty;
	
	m_NoticeButton.onClick:Clear();
	m_AccountButton.onClick:Clear();
	m_RegisterButton.onClick:Clear();
	m_LoginButton.onClick:Clear();
	m_AccountRegisterButton.onClick:Clear();
    m_LoginButton.onClick:Clear();
    m_RegisterButton.onClick:Clear();
end

function OnLoginClick()
	account = m_LoginAccountInput.text;
	password = m_LoginPasswordInput.text;
	if CheckInput(account) and CheckInput(password) then
		m_Params.OnClickLogin(account,password)
	end
end

function OnRegisterClick()
	account = m_RegisterAccountInput.text;
	password = m_RegisterPasswordInput.text;
	if CheckInput(account) and CheckInput(password) then
		m_Params.OnClickRegister(account,password,
		function()
			m_RegisterPanel.visible = false
		end
		)		
	end
end

function CheckInput(input)
	if input == nil or string.len(input) == 0 then
		Log.Error("账号密码不能为空！")
		return false
	elseif string.len(input) > 6 then
		Log.Error("账号密码不能超出6位！")
		return false
	else
		local num = tonumber(input)
		if num == nil then
			Log.Error("账号密码只能是6位数字！")
			return false
		else
			return true
		end
	end
end

--function OnUpdate(elapseSeconds,realElapseSeconds)
	--print("LuaForm OnUpdate:"..elapseSeconds..":"..realElapseSeconds)
--end

-- function OnDestroy()
-- 	OnClose(nil)
-- end