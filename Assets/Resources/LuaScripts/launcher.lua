require("framework.System")

local WindowContainer = CS.Loxodon.Framework.Views.WindowContainer
local Context = CS.Loxodon.Framework.Contexts.Context
local AccountService = require("Services.AccountService")
---
--模块
--@module Launcher
local M=class("Launcher",target)

function M:start()
	-- 获得应用上下文，一个游戏建议创建应用上下文和玩家上下文。
	-- 全局的服务都放入应用上下文中，如账号服务，网络组件，配置服务等基础组件和服务
	-- 只与某个玩家相关的如背包服务、装备服务、角色服务都放入玩家上下文，当登出游戏可以统一释放
	local context = Context.GetApplicationContext()
	
	--注册一个账号服务
	context:GetContainer():Register("accountService",AccountService())
	
	-- 从应用上下文获得一个视图定位器
	local locator = context:GetService("IUIViewLocator")

	-- 创建一个名为MAIN的窗口容器
	local winContainer = WindowContainer.Create("MAIN")
	
	-- 通过视图定位器，加载一个启动窗口视图
	local window = locator:LoadWindow(winContainer, "LuaUI/Startup/Startup")
	window:Create() --创建窗口
	local transition = window:Show() --显示窗口，返回一个transition对象，窗口显示一般会有窗口动画，所以是一个持续过程的操作
	transition:OnStateChanged(function(w,state) print("Window:"..w.Name.." State:"..state:ToString()) end) --监听显示窗口过程的窗口状态
	transition:OnFinish(function() print("OnFinished")  end) --监听窗口显示完成事件

	print("lua start...")
end

return M