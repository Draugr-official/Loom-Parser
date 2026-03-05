--lua

function onDone(res)
	print("response_code: " .. tostring(res.response_code))
	print("response_message: " .. res.response_message)
	print("mime_type: " .. res.mime_type)
	print("body_data: " .. string.sub(buffer.tostring(res.body_data), 1, 100))

	local data = parseJSON(buffer.tostring(res.body_data))
	local price_num = tonumber(data.result)
	this_object.content = "Ethereum gas price: " .. string.format("%.2f", price_num * 1.0e-9) ..
		" Gwei"
end

function onError(res)
	print("error_code: " .. tostring(res.error_code))
	print("error_description: " .. res.error_description)
end

infura_project_id = getSecret('infura_project_id')
print("infura_project_id " .. tostring(infura_project_id))

function onTimerEvent()
	print("Fetching current gas price...")
	-- See https://docs.infura.io/api/networks/ethereum/json-rpc-methods/eth_gasprice

	doHTTPPostRequestAsync("https://mainnet.infura.io/v3/" .. infura_project_id,
		'{"jsonrpc":"2.0","method":"eth_gasPrice","params": [],"id":1}', -- post content
		"application/json", -- content type
 		{}, -- additional header lines
		onDone, onError)
end

if(IS_SERVER) then
	createTimer(onTimerEvent, 60.0, true) -- Call onTimerEvent every 60 seconds, repeatedly.
else
	print("running on client, not doing anything.")
end