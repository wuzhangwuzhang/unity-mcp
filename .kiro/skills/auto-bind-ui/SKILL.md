---
name: auto-bind-ui
description: auto-bind-ui by kiro.
---

# Auto Binding UI

## How to Call

```bash
unity-mcp-cli run-tool auto-bind-ui --input '{
  "inputData": "string_value"
}'
```

> For complex input (multi-line strings, code), save the JSON to a file and use:
> ```bash
> unity-mcp-cli run-tool auto-bind-ui --input-file args.json
> ```
>
> Or pipe via stdin (recommended):
> ```bash
> unity-mcp-cli run-tool auto-bind-ui --input-file - <<'EOF'
> {"param": "value"}
> EOF
> ```


### Troubleshooting

If `unity-mcp-cli` is not found, either install it globally (`npm install -g unity-mcp-cli`) or use `npx unity-mcp-cli` instead.
Read the /unity-initial-setup skill for detailed installation instructions.

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

