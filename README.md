# 一体化配置管理平台

## 📖 简介  
- 目标：替换旧版配置工具，实现数据表配置、配置服务、终端配置的可视化一体化管理  
- 基于 JSON 元数据驱动，通过“主题”描述整个页面的外观与行为  

## 文档地址
https://i1u7uzuohsl.feishu.cn/wiki/EHqOwhF0nivjGSk6k0xcBVTnnCb?from=from_copylink


## 🐾 主题导航  
- 按模块（病理、产科）和功能组织菜单  
- 支持多级结构，配置名称、图标、路径（/crud、/dict、业务自定义）  
- 根据用户角色权限动态过滤展示  

## 🛠 核心组件  

### 1. CRUD 数据表  
- **列配置**：控制显示、查询、编辑、宽度  
- **查询条件**：文本框、下拉、多选等；支持跨页多选  
- **表单编辑**：自动单/双列布局，动态校验规则（必填、正则、长度）  
- **操作按钮**：新增、编辑、删除、批量上移/下移、导入/导出 Excel  
- **数据映射**：固定选项 & 远程字典（SQL）  
- **SQL 模板**：select/insert/update/delete/setState/setSequence/exportData  

### 2. DICT 配置服务  
- **XML 编辑**：可视化表单 ↔ 纯文本切换  
- **目录与锚点**：按 filePath/nodePath 分类，支持注释生成子锚点  
- **节点展示**：叶节点、数组节点、注释节点，多种输入组件可配置  

### 3. 全局状态  
- 注入当前用户（UserID、UserName）、医院/科室（HospitalID、DepartmentID）  
- 在 SQL 模板中以 `{GLOBAL}{…}` 占位替换  

## ⚙️ 通用配置  
- `columnConfig`、`sqlTemplates`、`buttonsShow` 等 JSON 节点统一管理  
- 多表支持 & 附加操作可扩展  
- 按钮显示/隐藏可配置，满足不同业务需求  

## 🧩 个性化扩展  
- 知识库管理  
- 内镜相关配置  
- 质评质控管理  
