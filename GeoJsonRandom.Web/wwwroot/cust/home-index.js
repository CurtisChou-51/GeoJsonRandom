/// <reference path="../js/site.js" />

/**
 * 隨機地理位置產生器的主頁面
 */
(function () {
    const { createApp, ref, onMounted } = Vue;
    const DEFAULT_TAKE_COUNT = 10, MAX_TAKE_COUNT = 1000, MIN_TAKE_COUNT = 1;

    createApp({
        setup() {
            let mapRenderHelper;

            const counties = ref([]), towns = ref([]), villages = ref([]);
            const mainFormData = ref({ county: '', town: '', village: '', takeCount: DEFAULT_TAKE_COUNT });
            const tableData = ref([]);
            const dataRenderMode = ref(1), mapRenderCluster = ref(true);
            const mainForm = ref(null), mainMap = ref(null);

            /**
             * 初始化頁面資料
             */
            onMounted(async () => {
                uiHelper.loadingStart();
                await loadCounties();
                uiHelper.loadingEnd();
            });

            /**
             * 載入縣市資料
             */
            async function loadCounties() {
                const resp = await postAjax('/Home/Counties');
                if (!resp.ReqFail)
                    counties.value = resp.data;
            }

            /**
             * 改變縣市事件
             */
            async function countyChange() {
                resetTown();
                resetVillage();
                if (!mainFormData.value.county)
                    return;
                const resp = await postAjax('/Home/Towns');
                if (!resp.ReqFail)
                    towns.value = resp.data;
            }

            /**
             * 改變鄉鎮事件
             */
            async function townChange() {
                resetVillage();
                if (!mainFormData.value.town)
                    return;
                const resp = await postAjax('/Home/Villages');
                if (!resp.ReqFail)
                    villages.value = resp.data;
            }

            /**
             * 重置鄉鎮選項
             */
            function resetTown() {
                mainFormData.value.town = '';
                towns.value = [];
            }

            /**
             * 重置村里選項
             */
            function resetVillage() {
                mainFormData.value.village = '';
                villages.value = [];
            }

            /**
             * 主表單提交事件
             */
            function onSubmit() {
                return false;
            }

            /**
             * 取樣點數輸入事件
             */
            function takeCountInput() {
                let takeCount = parseInt(mainFormData.value.takeCount);
                if (isNaN(takeCount) || !isFinite(takeCount))
                    takeCount = DEFAULT_TAKE_COUNT;
                takeCount = Math.min(Math.max(takeCount, MIN_TAKE_COUNT), MAX_TAKE_COUNT);
                mainFormData.value.takeCount = takeCount;
            }

            /**
             * 切換群聚顯示模式
             */
            function mapRenderClusterChange() {
                if (!mapRenderHelper)
                    return;
                mapRenderHelper.clear(true);
                renderMapClusterOrScatter();
            }

            /**
             * 根據模式渲染地圖
             */
            function renderMapClusterOrScatter() {
                if (mapRenderCluster.value)
                    mapRenderHelper.renderCluster();
                else
                    mapRenderHelper.render();
            }

            /**
             * 顯示地圖資料
             */
            async function renderMap() {
                uiHelper.loadingStart();
                dataRenderMode.value = 2;
                const resp = await postAjax('/Home/RandomPoints');
                if (!resp.ReqFail) {
                    // 如果地圖尚未初始化，則進行初始化
                    mapRenderHelper = mapRenderHelper ?? initMap(mainMap.value);
                    mapRenderHelper.clear().addData(resp.data);
                    renderMapClusterOrScatter();
                }
                uiHelper.loadingEnd();
            }

            /**
             * 頁面顯示資料
             */
            async function renderPage() {
                uiHelper.loadingStart();
                dataRenderMode.value = 1;
                const resp = await postAjax('/Home/RandomPoints');
                if (!resp.ReqFail)
                    tableData.value = resp.data;
                uiHelper.loadingEnd();
            }

            /**
             * Ajax請求表單提交
             */
            function postAjax(url) {
                return ajaxHelper.post(url, mainFormData.value);
            }

            /**
             * 原始表單提交
             */
            function postRaw(url) {
                mainForm.value.action = url;
                mainForm.value.submit();
            }


            /**
             * 下載Json
             */
            function downloadJson() {
                postRaw('/Home/RandomPointsJsonFile');
            }

            /**
             * 下載Csv
             */
            function downloadCsv() {
                postRaw('/Home/RandomPointsCsvFile');
            }

            return {
                // 資料
                mainFormData, tableData, dataRenderMode, mapRenderCluster,
                counties, towns, villages,
                mainForm, mainMap,

                // 事件處理
                countyChange, townChange, onSubmit, takeCountInput, mapRenderClusterChange,
                renderPage, renderMap, downloadJson, downloadCsv
            };
        }
    }).mount('#app');

    /**
     * 初始化地圖
     * @param {HTMLElement} mapDiv - 地圖容器元素
     * @returns {MapRenderHelper} - 地圖渲染輔助器實例
     */
    function initMap(mapDiv) {
        const map = L.map(mapDiv, {
            center: [23.633066, 121.301886],
            zoom: 8,
            maxZoom: 20,
            minZoom: 3,
            worldCopyJump: true,
            attributionControl: true,
            zoomControl: false
        });

        let tileLayer = L.tileLayer("https://wmts.nlsc.gov.tw/wmts/EMAP/default/GoogleMapsCompatible/{z}/{y}/{x}", { maxZoom: 20 });
        let hasErrorShown = false;
        tileLayer.on('tileerror', function () {
            // 如果是第一次載入失敗，顯示錯誤訊息
            if (!hasErrorShown) {
                hasErrorShown = true;
                uiHelper.warning('警告', '地圖圖層載入失敗，可能影響地圖顯示效果');
            }
        });
        tileLayer.addTo(map);

        return new MapRenderHelper(map)
            .setIconUrl("cust/placeholder.png")
            .setClusterColor("#f8ca05")
            .setMarkerClick(showPopup);
    }

    /**
     * 顯示彈出視窗
     * @param {Object} data - 地理位置資料
     * @param {Object} popup - 彈出視窗實例
     */
    function showPopup(data, popup) {
        const content = `
<table class="detail_popup_table">
    <tr>
        <td class="detail_popup_table_title">縣市</td>
        <td class="detail_popup_table_content">${data.County || ""}</td>
    </tr>
    <tr>
        <td class="detail_popup_table_title">鄉鎮</td>
        <td class="detail_popup_table_content">${data.Town || ""}</td>
    </tr>
    <tr>
        <td class="detail_popup_table_title">村里</td>
        <td class="detail_popup_table_content">${data.Village || ""}</td>
    </tr>
    <tr>
        <td class="detail_popup_table_title" style="width: 83px">經緯度</td>
        <td class="detail_popup_table_content">${data.Latitude.toFixed(6)},${data.Longitude.toFixed(6)}</td>
    </tr>
</table>`;
        popup.show(content);
    }
})();
