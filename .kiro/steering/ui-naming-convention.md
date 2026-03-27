---
inclusion: always
---

# Unity UI 节点命名规范

生成 Unity UI Prefab 时，所有 GameObject 节点名称必须遵循以下前缀规则：

## 前缀对照表
| 前缀 | 组件类型 | 示例 |
|------|---------|------|
| `#lbl_` | Text（旧版） | `#lbl_Title`, `#lbl_Time`, `#lbl_Gold` |
| `#txt_` | TextMeshProUGUI | `#txt_Title`, `#txt_Time`, `#txt_Gold` |
| `#img_` | Image | `#img_Reward`, `#img_Avatar`, `#img_Bg` |
| `#btn_` | Button | `#btn_Close`, `#btn_Sign`, `#btn_Confirm` |
| `#imgBtn_` | Image + Click（可点击的背景图） | `#imgBtn_Tab`, `#imgBtn_Item`, `#imgBtn_Card` |
| `#sld_` | Slider（滑动条） | `#sld_Volume`, `#sld_Progress` |
| `#scro_` | Scroll View（滚动视图） | `#scro_List`, `#scro_Content` |
| `#tgl_` | Toggle | `#tgl_Sound`, `#tgl_Notify` |
| `#ipt_` | InputField | `#ipt_Name`, `#ipt_Search` |
| `#drp_` | Dropdown | `#drp_Language`, `#drp_Quality` |
| `#view_` | View Behaviour（视图行为组件） | `#view_SignIn`, `#view_Reward`, `#view_Task` |

## 规则
- 前缀使用 `#` 开头 + 小写缩写 + `_` 分隔符
- 分隔符后跟大驼峰功能描述，如 `#btn_Confirm`
- 纯容器/面板节点不加前缀，直接用描述性名称，如 `MainPanel`、`SignInArea`、`Day1Panel`
- 循环生成的节点用编号后缀，如 `#img_Reward1`、`Day3Panel`

## 注意事项
- 禁止重名