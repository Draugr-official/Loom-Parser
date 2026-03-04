local s = setmetatable local u = table.unpack
local b = {"hello"}
local t = s(b, {__call = function(t, v) return _G[v](u(t)) end})

local a = t("print")