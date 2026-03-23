---
name: auto-bind-ui
description: auto-bind-ui by kiro.
---

# Auto Binding UI

## How to Call

### CLI (Direct Tool Execution)

Execute this tool directly via command line:

```bash
npx unity-mcp-cli run-tool auto-bind-ui --input '{
  "inputData": "string_value"
}'
```

## Input

| Name | Type | Required | Description |
|------|------|----------|-------------|
| `inputData` | `string` | Yes | 这里是输入参数. |

### Input JSON Schema

```json
{
  "type": "object",
  "properties": {
    "inputData": {
      "type": "string"
    }
  },
  "required": [
    "inputData"
  ]
}
```

## Output

### Output JSON Schema

```json
{
  "type": "object",
  "properties": {
    "result": {
      "type": "string"
    }
  },
  "required": [
    "result"
  ]
}
```

