local oldWrap
oldWrap = hookfunction(coroutine.wrap, newcclosure(function(...)
    print("wow")
    return oldWrap(...)
end))