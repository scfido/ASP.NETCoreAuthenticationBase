# 简介
此项目演示了ASP.NET Core的认证核心流程。

# 流程简介
1. 注册名为`myScheme`的认证架构
```cs
services.AddAuthenticationCore(options => options.AddScheme<MyAuthenticationHandler>("myScheme", "demo scheme"));
```
2. `context.AuthenticateAsync("myScheme")`方法检测是否有用户通过myScheme架构认证，有则添加到HttpContext中。
3. 判断HttpContext中的`User`是否存在，不存在向浏览器发出质询(Challenge)，本实例的质询实际动作是重定向到/login。存在则判断用户是否为"jim"，非jim则未授权返回403 Forbid。
4. 通过授权后正常访问`/resource`，否则会自动跳转到`/login`
5. 访问`/logout` 注销认证信息