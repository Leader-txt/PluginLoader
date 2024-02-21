### 插件热重载

让您在调试插件时更加快捷！

### 权限

- pluginloader.admin

### 指令

- `/load`: 显示指令列表
- `/load all`: 加载所有插件（不包括PluginLoader.json中的插件）
- `/load list`: 显示可加载的插件列表
- `/load this <插件名>`: 加载指定插件（注意这里使用插件名而非文件名，可通过`/load list`获取）
- `/load unload <插件名>`: 卸载已加载的插件
- `/load loaded`: 列出所有已加载的插件

### 注意事项

- 本插件仅适用于调试插件时使用。
- 如果出现指令重复等问题是因为目标插件未编写释放方法，本插件已完整编写了释放方法。
- 使用时请尽量避免全部重载，因为目前没有多少插件编写了完整的释放(dispose)方法。
- 需要调试的插件请编写完整的dispose方法。

### C#示例

```csharp
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        // 在这里完成Dispose方法
    }
    base.Dispose(disposing);
}
```

---

### Plugin Hot Reload

Allows for quicker debugging of plugins!

### Permissions

- pluginloader.admin

### Commands

- `/load`: Show command list
- `/load all`: Load all plugins (excluding those in PluginLoader.json)
- `/load list`: Show list of plugins that can be loaded
- `/load this <plugin name>`: Load specified plugin (note that this uses the plugin name, not the file name; use `/load list` to get the plugin names)
- `/load unload <plugin name>`: Unload a loaded plugin
- `/load loaded`: List all loaded plugins

### Notes

- This plugin is intended for use during plugin debugging.
- If you encounter issues like duplicate commands, it's because the target plugin lacks a proper release method. This plugin includes a complete release method.
- Try to avoid full reloads as many plugins currently do not have complete release (dispose) methods.
- For plugins that need debugging, please write a complete dispose method.

### C# Example

```csharp
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        // Complete Dispose method here
    }
    base.Dispose(disposing);
}
```
