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

