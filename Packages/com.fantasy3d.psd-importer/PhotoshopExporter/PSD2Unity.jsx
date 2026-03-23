/**
 * PSD to Unity UGUI 导出脚本
 * 
 * 使用方法：
 *   1. 在 Photoshop 中打开 PSD 文件
 *   2. 文件 > 脚本 > 浏览... 选择此脚本
 *   3. 选择导出目录
 *   4. 脚本会在导出目录下生成：
 *      - [PSD文件名].json  （布局数据）
 *      - images/           （所有切图 PNG）
 *
 * 图层命名规范：
 *   - 普通图层 → 导出为 PNG 图片（Type: "png"）
 *   - 名称以 "txt_" 开头的图层 → 识别为文本（Type: "text"）
 *   - 文本图层（TextItem）→ 自动识别为文本
 *   - 图层组 → 识别为节点容器（Type: "node"）
 *   - 名称包含 "[9]" 的图层 → 标记为九宫格图片
 *   - 名称以 "_" 开头的图层 → 跳过不导出
 *
 * 九宫格标记：
 *   在图层名称中添加 [9] 标记，如 "btn_bg[9]"
 *   脚本会自动根据图片尺寸计算均分的九宫格边距
 *   也可以手动指定：[9:left,top,right,bottom]，如 "btn_bg[9:10,10,10,10]"
 */

// ============================================================
// 全局配置
// ============================================================
var CONFIG = {
    imageFormat: "png",
    imageQuality: 100,
    skipHiddenLayers: true,
    skipPrefix: "_",
    nineSliceTag: "[9",
    textPrefix: "txt_",
    defaultNineSliceRatio: 0.33  // 默认九宫格边距占比
};

// ============================================================
// 主入口
// ============================================================
function main() {
    if (!app.documents.length) {
        alert("请先打开一个PSD文件！");
        return;
    }

    var doc = app.activeDocument;
    var psdName = doc.name.replace(/\.psd$/i, "");

    // 选择导出目录
    var outputFolder = Folder.selectDialog("选择导出目录");
    if (!outputFolder) return;

    var exportPath = outputFolder.fsName;
    var imagesFolder = new Folder(exportPath + "/images");
    if (!imagesFolder.exists) imagesFolder.create();

    // 保存当前文档状态
    var originalRulerUnits = app.preferences.rulerUnits;
    app.preferences.rulerUnits = Units.PIXELS;

    var canvasWidth = doc.width.as("px");
    var canvasHeight = doc.height.as("px");

    // 收集数据
    var infoList = [];
    var duplicateMap = {};
    var exportedImages = {};

    // 遍历图层
    traverseLayers(doc, "", infoList, duplicateMap, exportedImages, imagesFolder, doc);

    // 构建JSON
    var jsonData = {
        "CanvasSize": {
            "Width": canvasWidth,
            "Height": canvasHeight
        },
        "InfoList": infoList,
        "DuplicateMap": duplicateMap
    };

    // 写入JSON文件
    var jsonFile = new File(exportPath + "/" + psdName + ".json");
    jsonFile.encoding = "UTF-8";
    jsonFile.open("w");
    jsonFile.write(jsonStringify(jsonData, 2));
    jsonFile.close();

    // 恢复设置
    app.preferences.rulerUnits = originalRulerUnits;

    alert("导出完成！\n\n" +
        "画布尺寸: " + canvasWidth + "x" + canvasHeight + "\n" +
        "导出图层: " + infoList.length + "\n" +
        "导出路径: " + exportPath);
}

// ============================================================
// 图层遍历
// ============================================================
function traverseLayers(layerSet, parentTree, infoList, duplicateMap, exportedImages, imagesFolder, doc) {
    var layers = layerSet.layers;

    for (var i = layers.length - 1; i >= 0; i--) {
        var layer = layers[i];

        // 跳过隐藏图层
        if (CONFIG.skipHiddenLayers && !layer.visible) continue;

        // 跳过以 _ 开头的图层
        var layerName = sanitizeName(layer.name);
        if (layerName.indexOf(CONFIG.skipPrefix) === 0) continue;

        // 清理九宫格标记后的名称
        var cleanName = cleanNineSliceTag(layerName);
        var treePath = parentTree ? (parentTree + "/" + cleanName) : cleanName;

        if (layer.typename === "LayerSet") {
            // 图层组 → node
            var groupBounds = getLayerBounds(layer);
            var groupInfo = {
                "Name": cleanName,
                "Type": "node",
                "Tree": treePath,
                "FilePath": "",
                "Pos": { "X": groupBounds.x, "Y": groupBounds.y },
                "Size": { "Width": groupBounds.width, "Height": groupBounds.height },
                "IsNineSlice": false,
                "Left": 0, "Top": 0, "Right": 0, "Bottom": 0
            };
            infoList.push(groupInfo);

            // 递归子图层
            traverseLayers(layer, treePath, infoList, duplicateMap, exportedImages, imagesFolder, doc);
        } else {
            // 判断是否是文本图层
            var isText = false;
            var textContent = "";
            var fontSize = 24;
            var textColor = { "Red": 255, "Green": 255, "Blue": 255 };
            var alignment = "MiddleLeft";

            try {
                if (layer.kind === LayerKind.TEXT) {
                    isText = true;
                    var textItem = layer.textItem;
                    textContent = textItem.contents;
                    fontSize = parseFloat(textItem.size.as("px"));
                    var c = textItem.color;
                    textColor = {
                        "Red": Math.round(c.rgb.red),
                        "Green": Math.round(c.rgb.green),
                        "Blue": Math.round(c.rgb.blue)
                    };
                    alignment = getTextAlignment(textItem);
                }
            } catch (e) {
                // 非文本图层，忽略
            }

            // txt_ 前缀也视为文本（但可能不是PS文本图层）
            if (!isText && layerName.indexOf(CONFIG.textPrefix) === 0) {
                isText = true;
            }

            var bounds = getLayerBounds(layer);

            if (isText) {
                var textInfo = {
                    "Name": cleanName,
                    "Type": "text",
                    "Tree": treePath,
                    "FilePath": "",
                    "Pos": { "X": bounds.x, "Y": bounds.y },
                    "Size": { "Width": bounds.width, "Height": bounds.height },
                    "IsNineSlice": false,
                    "Left": 0, "Top": 0, "Right": 0, "Bottom": 0,
                    "Content": textContent,
                    "FontSize": fontSize,
                    "TextColor": textColor,
                    "Alignment": alignment
                };
                infoList.push(textInfo);
            } else {
                // 图片图层 → 导出PNG
                var fileName = cleanName + ".png";

                // 检查重复图片（同名图层复用同一张图）
                var imageKey = getLayerHash(layer, doc);
                if (exportedImages[imageKey]) {
                    duplicateMap[fileName] = exportedImages[imageKey];
                } else {
                    exportLayerAsImage(layer, doc, imagesFolder, fileName);
                    exportedImages[imageKey] = fileName;
                }

                // 九宫格检测
                var nineSlice = parseNineSlice(layerName, bounds.width, bounds.height);

                var imgInfo = {
                    "Name": cleanName,
                    "Type": "png",
                    "Tree": treePath,
                    "FilePath": fileName,
                    "Pos": { "X": bounds.x, "Y": bounds.y },
                    "Size": { "Width": bounds.width, "Height": bounds.height },
                    "IsNineSlice": nineSlice.isNineSlice,
                    "Left": nineSlice.left,
                    "Top": nineSlice.top,
                    "Right": nineSlice.right,
                    "Bottom": nineSlice.bottom
                };
                infoList.push(imgInfo);
            }
        }
    }
}

// ============================================================
// 图层导出为PNG
// ============================================================
function exportLayerAsImage(layer, doc, imagesFolder, fileName) {
    // 保存当前状态
    var savedState = doc.activeHistoryState;

    try {
        // 隐藏所有图层
        hideAllLayers(doc);

        // 只显示目标图层及其父级
        showLayerAndParents(layer);

        // 裁剪到图层边界并导出
        var bounds = layer.bounds;
        var left = bounds[0].as("px");
        var top = bounds[1].as("px");
        var right = bounds[2].as("px");
        var bottom = bounds[3].as("px");

        // 复制到新文档
        doc.selection.selectAll();
        doc.selection.copy(true);

        var tempDoc = app.documents.add(
            right - left,
            bottom - top,
            doc.resolution,
            "temp_export",
            NewDocumentMode.RGB,
            DocumentFill.TRANSPARENT
        );

        app.activeDocument = tempDoc;
        tempDoc.paste();

        // 调整画布偏移
        if (tempDoc.layers.length > 1) {
            var pastedLayer = tempDoc.activeLayer;
            pastedLayer.translate(
                new UnitValue(-left, "px"),
                new UnitValue(-top, "px")
            );
        }

        // 合并可见图层
        tempDoc.mergeVisibleLayers();
        tempDoc.trim(TrimType.TRANSPARENT);

        // 保存PNG
        var pngFile = new File(imagesFolder.fsName + "/" + fileName);
        var pngOptions = new PNGSaveOptions();
        pngOptions.compression = 6;
        pngOptions.interlaced = false;
        tempDoc.saveAs(pngFile, pngOptions, true, Extension.LOWERCASE);

        // 关闭临时文档
        tempDoc.close(SaveOptions.DONOTSAVECHANGES);
    } catch (e) {
        // 如果上面的方法失败，尝试备用方案
        try {
            exportLayerFallback(layer, doc, imagesFolder, fileName);
        } catch (e2) {
            // 忽略导出失败
        }
    }

    // 恢复文档状态
    app.activeDocument = doc;
    doc.activeHistoryState = savedState;
}

// 备用导出方案：使用 Export As
function exportLayerFallback(layer, doc, imagesFolder, fileName) {
    var savedState = doc.activeHistoryState;

    // 复制图层到新文档
    var tempDoc = app.documents.add(
        doc.width, doc.height, doc.resolution,
        "temp", NewDocumentMode.RGB, DocumentFill.TRANSPARENT
    );

    app.activeDocument = doc;
    layer.duplicate(tempDoc);

    app.activeDocument = tempDoc;
    tempDoc.mergeVisibleLayers();
    tempDoc.trim(TrimType.TRANSPARENT);

    var pngFile = new File(imagesFolder.fsName + "/" + fileName);
    var pngOptions = new PNGSaveOptions();
    pngOptions.compression = 6;
    tempDoc.saveAs(pngFile, pngOptions, true, Extension.LOWERCASE);
    tempDoc.close(SaveOptions.DONOTSAVECHANGES);

    app.activeDocument = doc;
    doc.activeHistoryState = savedState;
}

// ============================================================
// 工具函数
// ============================================================

function getLayerBounds(layer) {
    var bounds = layer.bounds;
    var left = bounds[0].as("px");
    var top = bounds[1].as("px");
    var right = bounds[2].as("px");
    var bottom = bounds[3].as("px");
    return {
        x: left,
        y: top,
        width: right - left,
        height: bottom - top
    };
}

function sanitizeName(name) {
    // 去掉PS自动添加的 "副本" / "copy" 后缀，保留有意义的名称
    return name.replace(/\s+/g, "_").replace(/[\/\\:*?"<>|]/g, "_");
}

function cleanNineSliceTag(name) {
    // 移除 [9] 或 [9:x,x,x,x] 标记
    return name.replace(/\[9[^\]]*\]/g, "").replace(/_+$/, "");
}

function parseNineSlice(layerName, width, height) {
    var result = { isNineSlice: false, left: 0, top: 0, right: 0, bottom: 0 };

    var idx = layerName.indexOf(CONFIG.nineSliceTag);
    if (idx === -1) return result;

    result.isNineSlice = true;

    // 检查是否有自定义参数 [9:left,top,right,bottom]
    var match = layerName.match(/\[9:(\d+),(\d+),(\d+),(\d+)\]/);
    if (match) {
        result.left = parseInt(match[1]);
        result.top = parseInt(match[2]);
        result.right = parseInt(match[3]);
        result.bottom = parseInt(match[4]);
    } else {
        // 默认均分
        var ratio = CONFIG.defaultNineSliceRatio;
        result.left = Math.round(width * ratio);
        result.top = Math.round(height * ratio);
        result.right = Math.round(width * ratio);
        result.bottom = Math.round(height * ratio);
    }

    return result;
}

function getTextAlignment(textItem) {
    try {
        var justification = textItem.justification;
        switch (justification) {
            case Justification.LEFT: return "MiddleLeft";
            case Justification.CENTER: return "MiddleCenter";
            case Justification.RIGHT: return "MiddleRight";
            case Justification.FULLYJUSTIFIED: return "MiddleJustify";
            default: return "MiddleLeft";
        }
    } catch (e) {
        return "MiddleLeft";
    }
}

function getLayerHash(layer, doc) {
    // 简单的图层标识：名称 + 尺寸
    var bounds = getLayerBounds(layer);
    return layer.name + "_" + bounds.width + "x" + bounds.height;
}

function hideAllLayers(doc) {
    for (var i = 0; i < doc.layers.length; i++) {
        doc.layers[i].visible = false;
    }
}

function showLayerAndParents(layer) {
    layer.visible = true;
    var parent = layer.parent;
    while (parent && parent.typename !== "Document") {
        parent.visible = true;
        parent = parent.parent;
    }
}

// ============================================================
// JSON序列化（ExtendScript没有原生JSON）
// ============================================================
function jsonStringify(obj, indent) {
    indent = indent || 0;
    return _stringify(obj, 0, indent);
}

function _stringify(value, currentIndent, indentSize) {
    if (value === null || value === undefined) return "null";
    if (typeof value === "boolean") return value ? "true" : "false";
    if (typeof value === "number") return String(value);
    if (typeof value === "string") return '"' + escapeString(value) + '"';

    var spaces = "";
    var innerSpaces = "";
    if (indentSize > 0) {
        for (var s = 0; s < currentIndent + indentSize; s++) innerSpaces += " ";
        for (var s2 = 0; s2 < currentIndent; s2++) spaces += " ";
    }

    if (value instanceof Array) {
        if (value.length === 0) return "[]";
        var items = [];
        for (var i = 0; i < value.length; i++) {
            items.push(innerSpaces + _stringify(value[i], currentIndent + indentSize, indentSize));
        }
        if (indentSize > 0) {
            return "[\n" + items.join(",\n") + "\n" + spaces + "]";
        }
        return "[" + items.join(",") + "]";
    }

    // Object
    var keys = [];
    for (var key in value) {
        if (value.hasOwnProperty(key)) {
            keys.push(key);
        }
    }
    if (keys.length === 0) return "{}";

    var pairs = [];
    for (var k = 0; k < keys.length; k++) {
        var keyStr = '"' + escapeString(keys[k]) + '"';
        var valStr = _stringify(value[keys[k]], currentIndent + indentSize, indentSize);
        pairs.push(innerSpaces + keyStr + ": " + valStr);
    }

    if (indentSize > 0) {
        return "{\n" + pairs.join(",\n") + "\n" + spaces + "}";
    }
    return "{" + pairs.join(",") + "}";
}

function escapeString(str) {
    return str
        .replace(/\\/g, "\\\\")
        .replace(/"/g, '\\"')
        .replace(/\n/g, "\\n")
        .replace(/\r/g, "\\r")
        .replace(/\t/g, "\\t");
}

// ============================================================
// 执行
// ============================================================
main();
