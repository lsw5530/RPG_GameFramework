-- xlua热修复测试，主要测试下资源热更以后xlua重启后热修复是否生效
-- 注意：
-- 1、现在的做法热修复模块一定要提供Register、Unregister两个接口，因为现在热修复模块要支持动态加载和卸载
-- 2、注册使用xlua.hotfix或者util.hotfix_ex
-- 3、注销一律使用xlua.hotfix

local util = require "XLua.Common.util"
local LuaComponent = CS.GameMain.LuaComponent

xlua.private_accessible(LuaComponent)

local function TestHotfix(self)
	print("********** Call TestHotfix in lua...<<<")
end


local function Register()
	util.hotfix_ex(LuaComponent, "TestHotfix", TestHotfix)
end

local function Unregister()
	xlua.hotfix(AssetbundleUpdater, "TestHotfix", nil)
end

return {
	Register = Register,
	Unregister = Unregister,
}