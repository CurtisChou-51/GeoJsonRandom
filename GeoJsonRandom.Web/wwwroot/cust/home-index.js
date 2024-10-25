/// <reference path="../js/site.js" />

(function () {
    const { createApp, ref, onMounted } = Vue;

    createApp({
        setup() {
            let helper;
            onMounted(async () => {
                uiHelper.loadingStart();
                const resp = await postAjax('/Home/Counties');
                if (!resp.ReqFail)
                    counties.value = resp.data;
                uiHelper.loadingEnd();
            })

            const counties = ref([]), towns = ref([]), villages = ref([]);
            const mainFormData = ref({ county: '', town: '', village: '', takeCount: 10 });
            const tableData = ref([]), mode = ref(1);
            const mainForm = ref(null), mainMap = ref(null);

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

            async function townChange() {
                mainFormData.value.village = '';
                villages.value = [];
                if (!mainFormData.value.town)
                    return;
                const resp = await postAjax('/Home/Villages');
                if (!resp.ReqFail)
                    villages.value = resp.data;
            }


            function onSubmit() {
                return false;
            }

            function takeCountInput() {
                let takeCount = parseInt(mainFormData.value.takeCount);
                if (isNaN(takeCount) || !isFinite(takeCount))
                    takeCount = 10;
                takeCount = Math.min(Math.max(takeCount, 1), 1000);
                mainFormData.value.takeCount = takeCount;
            }

            function postAjax(url) {
                return ajaxHelper.post(url, mainFormData.value);
            }

            function postRaw(url) {
                mainForm.value.action = url;
                mainForm.value.submit();
            }

            async function renderMap() {
                uiHelper.loadingStart();
                mode.value = 2;
                const resp = await postAjax('/Home/RandomPoints');
                if (!resp.ReqFail) {
                    helper = helper ?? initMap(mainMap.value);
                    helper.clear().addData(resp.data).renderCluster();
                }
                uiHelper.loadingEnd();
            }

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
                mainFormData, tableData, mode,
                counties, towns, villages,
                countyChange, townChange, onSubmit, takeCountInput,
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
