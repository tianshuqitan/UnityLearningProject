local rapidjson = require('rapidjson')
local t = rapidjson.decode('{"a":123}')
print(t.a)
t.a = 456
local s = rapidjson.encode(t)
print('json', s)