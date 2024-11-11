# GeoJsonRandom - 台灣地理位置隨機產生器

## 專案簡介
- GeoJsonRandom 是一個基於 GeoJSON 資料的隨機地理位置產生工具。此工具可以根據實際地理資料產生合理的隨機座標，快速取得台灣本島與離島的測試用地理位置資料
- 目前已部署於 Azure：[台灣地理位置隨機產生器](https://geojsonrandom20241021100422.azurewebsites.net/)

## 主要功能
- 支援三個層級行政區域（縣市、鄉鎮、村里）檢索，生成符檢索範圍的隨機位置資訊
- 位置資訊可由 Web 應用模組輸出：
  - 透過 Web API 接口
  - 透過網頁列表展示
  - 透過網頁地圖元件展示
  - 透過 CSV、JSON 檔案匯出


## 專案結構
專案分為兩個主要部分：

### GeoJsonRandom.Core
核心邏輯模組，負責：
- GeoJSON 資料的解析和處理
- 由地理區域的面積分配隨機數權重
- Prefix Tree 資料結構實現，用於行政區名稱檢索

### GeoJsonRandom.Web
Web 應用模組，提供：
- 網頁使用者介面
- 地理位置視覺化展示
- Web API 接口

## 技術架構

### 後端
- .NET Core
- NetTopologySuite：GeoJSON 地理資料處理

### 前端
- Leaflet：開源地圖庫
- Leaflet.markercluster：標記點群聚功能
- MapRenderHelper：自製的地圖渲染輔助工具
  - 支援標記點的建立和管理
  - 支援群聚功能的配置
  - 提供彈出視窗的控制
  - 可自訂標記點圖示和群聚樣式


## 使用方式
1. GeoJSON 地理資料與路徑依照專案預設即可
2. 啟動服務
3. 通過 Web API 或網頁界面使用

## API 使用範例

### 端點
- GET /api/GeoData/RandomPoints/{takeCount}/{county}/{town}/{village}

### 參數
- takeCount：要產生的隨機點數量
- county：縣市名稱
- town：鄉鎮名稱
- village：村里名稱

### 範例
- /api/GeoData/RandomPoints/10
- /api/GeoData/RandomPoints/10/新北市
- /api/GeoData/RandomPoints/10/新北市/三重區
- /api/GeoData/RandomPoints/10/新北市/三重區/培德里

### 回應格式
```json
[
  {
    "Longitude": 120.200429,
    "Latitude": 22.947739,
    "County": "臺南市",
    "Town": "南區",
    "Village": "喜東里"
  },
  {
    "Longitude": 121.005155,
    "Latitude": 22.852315,
    "County": "臺東縣",
    "Town": "延平鄉",
    "Village": "紅葉村"
  }
]
```

## 資料來源
- 本專案使用資料方式為讀取實體 JSON 檔案 [VILLAGE_NLSC_1130807_clear.json](GeoJsonRandom.Web/VILLAGE_NLSC_1130807_clear.json)
- 於 [內政部國土測繪中心開放資料](https://data.gov.tw/dataset/7441) 下載 SHP 檔案
- 再將 SHP 檔轉換為 GeoJSON 格式，實測後使用 [Mapshaper](https://mapshaper.org/) 沒有亂碼問題

## 目前實作與限制
- Prefix Tree 基於行政區名稱檢索而建立，不支援複雜的地理空間查詢（如範圍查詢、最近點查詢等）
- 所有資料都載入記憶體，對大量資料可能造成記憶體壓力，重啟服務需重新載入所有資料，適合小型應用場景和資料量較小的使用情境
- 目前 Web API 或網頁界面限制單次輸出最多 1000 筆
