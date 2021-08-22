# 项目介绍

- 基于Asp .net core 3.1的仿携程网的Restful web api 项目
- 技术栈
  - Asp .net core 3.1
  - Entity Framework core
  - MySQL
  - AutoMapper
  - Jwt
  - Asp .net core Identity 
  - Nlog
  - NewtonsolftJson
- 功能
  - 旅游路线增删改查
  - Jwt用户权限授权认证
  - 购物车系统
  - 订单系统
  - 数据分页
  - 数据排序
  - 数据塑形
- [项目一级路由查看地址](http://47.113.218.122:8080/api)
- 项目详细内容请关注[博客](https://www.cnblogs.com/bleso/)，或者+2495644988@qq.com/bleso624@gmail.com



# 项目演示

1. 使用postman打开项目介绍中的地址![image-20210822132854622](https://gitee.com/bleso624/bleso_picbed/raw/master/img/image-20210822132854622.png)

2. 可尝试用返回的link产看其余api, [产看路由路线](http://47.113.218.122:8080/api/TouristRoutes)为例, 由于项目实现了Restful 第三等级成熟度的HATEOAS, 在请求头中修改Accept如图,如果不想获取link信息,直接使用**application/json**![image-20210822134719115](https://gitee.com/bleso624/bleso_picbed/raw/master/img/image-20210822134719115.png)

3. 在返回的数据中我们可以看到每条数据相关的自发现链接![image-20210822135053010](https://gitee.com/bleso624/bleso_picbed/raw/master/img/image-20210822135053010.png)

4. 数据分页(pagesize, pagenumber)![image-20210822140340082](https://gitee.com/bleso624/bleso_picbed/raw/master/img/image-20210822140340082.png)

5. 数据排序(orderby)![image-20210822141017206](https://gitee.com/bleso624/bleso_picbed/raw/master/img/image-20210822141017206.png)

6. 数据塑形(fields)![image-20210822141502432](https://gitee.com/bleso624/bleso_picbed/raw/master/img/image-20210822141502432.png)

   

   

   

   

