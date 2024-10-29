/// <reference path="../js/site.js" />

(function () {
    const { createApp, ref, onMounted } = Vue;

    createApp({
        setup() {
            let mapRenderHelper;
            onMounted(async () => {
                uiHelper.loadingStart();
                const resp = await postAjax('/Home/Counties');
                if (!resp.ReqFail)
                    counties.value = resp.data;
                uiHelper.loadingEnd();
            })

            const counties = ref([]), towns = ref([]), villages = ref([]);
            const mainFormData = ref({ county: '', town: '', village: '', takeCount: 10 });
            const tableData = ref([]), mode = ref(1), mapRenderCluster = ref(true);
            const mainForm = ref(null), mainMap = ref(null);

            /**
             * 改變縣市事件
             */
            async function countyChange() {
                mainFormData.value.town = '';
                towns.value = [];
                mainFormData.value.village = '';
                villages.value = [];
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
                mainFormData.value.village = '';
                villages.value = [];
                if (!mainFormData.value.town)
                    return;
                const resp = await postAjax('/Home/Villages');
                if (!resp.ReqFail)
                    villages.value = resp.data;
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
                    takeCount = 10;
                takeCount = Math.min(Math.max(takeCount, 1), 1000);
                mainFormData.value.takeCount = takeCount;
            }

            /**
             * 地圖渲染checkbox改變事件
             */
            function mapRenderClusterChange() {
                mapRenderHelper.clear(true);
                renderMapClusterOrScatter();
            }

            /**
             * 地圖渲染為cluster或是scatter (由 checkbox mapRenderCluster 決定)
             */
            function renderMapClusterOrScatter() {
                if (mapRenderCluster.value)
                    mapRenderHelper.renderCluster();
                else
                    mapRenderHelper.render();
            }

            function postAjax(url) {
                return ajaxHelper.post(url, mainFormData.value);
            }

            function postRaw(url) {
                mainForm.value.action = url;
                mainForm.value.submit();
            }

            /**
             * 地圖顯示資料
             */
            async function renderMap() {
                uiHelper.loadingStart();
                mode.value = 2;
                const resp = await postAjax('/Home/RandomPoints');
                if (!resp.ReqFail) {
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
                mode.value = 1;
                const resp = await postAjax('/Home/RandomPoints');
                if (!resp.ReqFail)
                    tableData.value = resp.data;
                uiHelper.loadingEnd();
            }

            function downloadJson() {
                postRaw('/Home/RandomPointsJsonFile');
            }

            function downloadCsv() {
                postRaw('/Home/RandomPointsCsvFile');
            }

            return {
                mainFormData, tableData, mode, mapRenderCluster,
                counties, towns, villages,
                countyChange, townChange, onSubmit, takeCountInput, mapRenderClusterChange,
                renderPage, renderMap, downloadJson, downloadCsv,
                mainForm, mainMap
            }
        }
    }).mount('#app');

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

        L.tileLayer("https://wmts.nlsc.gov.tw/wmts/EMAP/default/GoogleMapsCompatible/{z}/{y}/{x}", { maxZoom: 20 }).addTo(map);
        return new MapRenderHelper(map)
            .setIconUrl("cust/placeholder.png")
            .setClusterColor("#f8ca05")
            .setMarkerClick(function (data, popup) {
                popup.show(`
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
</table>`);
            });
    }
})();
