local Parser = require('std.syntax.parser') 

function test_syntax(code_string, test_name)
    local success = pcall(function()
        return Parser.parse(code_string)
    end)
    
    if success then
        print(string.format("[PASS] %s: Syntax is valid.", test_name))
    else
        print(string.format("[FAIL] %s: Syntax error found: %s", test_name, result))
    end
end