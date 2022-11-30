# XLua 安装及说明

## 下载安装

1. 下载 [xlua](https://github.com/Tencent/xLua) 源码
2. 新建 Unity 工程，`UnityLearningProject`
3. 将 `xLua-Master/Tools` 拷贝到 `UnityLearningProject/Tools` 目录
4. 将 `xLua-Master/Assets/Plugins` 拷贝到 `UnityLearningProject/Assets/Plugins` 目录
5. 将 `xLua-Master/Assets/XLua` 拷贝到 `UnityLearningProject/Assets/XLua` 目录，这里其实不做限制，只要在 `Assets` 目录下就行

## 目录解析

```
| Plugins(不同平台的 xlua 库文件)
    | Android
    | arm64
    | ios
    | WebGL
    | WSA
    | x86
    | x86_64
    | xlua.bundle
| XLua
    | Editor(提供的一个示例配置)
        | ExampleConfig
    | Gen
    | Resources(提供的一些 lua 脚本)
        | perf
            | memory.lua
            | profiler.lua
        | tdr
            | tdr.lua
        | xlua
            | util.lua
    | Src(C# 源代码)
```


## 第三方 lua 库

* https://github.com/xpol/lua-rapidjson.git
* https://github.com/starwing/lua-protobuf


* 拷贝 `xLua-master\build` 目录到 `UnityLearningProject` 目录下, 拷贝之后 `build` 与 `Assets` 同级
* 先执行一下 `UnityLearningProject\build\make_win64_lua53.bat` 文件，看是否正常生成 `xlua.dll`
  * 本地使用 VS2022，改为 `cmake -G "Visual Studio 17 2022"`
  * 如果之前点过一次会报如下错误，删除 `UnityLearningProject\build\build64\CMakeCache.txt` 文件，再次执行
  ```
  CMake Error: Error: generator : Visual Studio 17 2022
  Does not match the generator used previously: Visual Studio 15 2017 Win64
  Either remove the CMakeCache.txt file and CMakeFiles directory or choose a different binary directory.
  ```

正常生成 `xlua.dll` 之后

* 下载 [lua-rapidjson](https://github.com/xpol/lua-rapidjson.git)
* 在 `UnityLearningProject\build` 下新建目录 `lua-rapidjson`
* 拷贝 `lua-rapidjson-master\rapidjson\include` 到 `UnityLearningProject\build\lua-rapidjson\` 目录
* 拷贝 `lua-rapidjson-master\src` 到 `UnityLearningProject\build\lua-rapidjson\` 目录，重命名为 `source`
* 修改 `UnityLearningProject\build\CMakeLists.txt` 增加如下内容
  ```CMakeLists
  #begin lua-rapidjson
  set (RAPIDJSON_SRC 
      lua-rapidjson/source/Document.cpp
      lua-rapidjson/source/rapidjson.cpp
      lua-rapidjson/source/Schema.cpp
      lua-rapidjson/source/values.cpp
    )
  set_property(
    SOURCE ${RAPIDJSON_SRC}
    APPEND
    PROPERTY COMPILE_DEFINITIONS
    LUA_LIB
  )
  list(APPEND THIRDPART_INC  lua-rapidjson/include)
  set (THIRDPART_SRC ${THIRDPART_SRC} ${RAPIDJSON_SRC})
  #end lua-rapidjson
  ```
* 再次执行 `make_win64_lua53.bat`
* 生成 `xlua.dll`，且自动拷贝到了 `build\plugin_lua53\Plugins` 目录下
* 将 `build\plugin_lua53\Plugins` 拷贝到 `Assets\Plugins` 目录下

注意
* CMakeLists.txt 修改位置，放到 `set(LUAJIT_SRC_PATH luajit-2.1.0b3/src)` 之后
* `lua-rapidjson camke` 部分与官方案例提供的有点不一致，因为它只拷贝了 `rapidjson.cpp` 其他没有拷贝


C# 侧集成

* 打开 `UnityLearningProject\Assets\XLua\Src\LuaDLL.cs` 在文件最后添加如下代码
  ```c#
  [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
  public static extern int luaopen_rapidjson(System.IntPtr L);

  [MonoPInvokeCallback(typeof(LuaDLL.lua_CSFunction))]
  public static int LoadRapidJson(System.IntPtr L)
  {
      return luaopen_rapidjson(L);
  }
  ```
* 或者新建一个 `LuaDllExtensions.cs` 文件，放到 `LuaDLL.cs` 同级目录
  ```c#
  namespace XLua.LuaDLL
  {
      using System.Runtime.InteropServices;

      public partial class Lua
      {
          [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
          public static extern int luaopen_rapidjson(System.IntPtr L);

          [MonoPInvokeCallback(typeof(LuaDLL.lua_CSFunction))]
          public static int LoadRapidJson(System.IntPtr L)
          {
              return luaopen_rapidjson(L);
          }
      }
  }
  ```
* 修改 LuaEnv.cs 加入以下代码
  ```c#
  AddBuildin("rapidjson", LuaDLL.Lua.LoadRapidJson);
  ```
* 或者在创建 LuaEnv 实例之后，执行以下代码
  ```c#
  m_luaEnv.AddBuildin("rapidjson", XLua.LuaDLL.Lua.LoadRapidJson);
  ```

测试是否成功，新建 `Resources\main.lua.txt` 文件

```lua
local rapidjson = require('rapidjson')
local t = rapidjson.decode('{"a":123}')
print(t.a)
t.a = 456
local s = rapidjson.encode(t)
print('json', s)
```

## 第三方库2

直接下载 [build_xlua_with_libs](https://github.com/chexiongsheng/build_xlua_with_libs)