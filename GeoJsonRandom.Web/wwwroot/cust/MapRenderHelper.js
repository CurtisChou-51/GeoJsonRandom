
class MapRenderHelper {

    constructor(map) {
        this._map = map;
        this._settingDic = {
            'Longitude': x => x.Longitude,
            'Latitude': x => x.Latitude,
            'IconWidth': 32,
            'IconHeight': 32,
            'ClusterColor': '#000000',
            'ClusterSize': childCount => {
                let minSize = 35, maxSize = 80, rate = 0.2;
                return Math.min(minSize + childCount * rate, maxSize).toFixed(2);
            },
            'ClusterFontSize': childCount => {
                let minSize = 15, maxSize = 28, rate = 0.03;
                return `${Math.min(minSize + childCount * rate, maxSize).toFixed(2)}px`;
            }
        }
        this._markers = [];
        let self = this;
        self._custPopup = new CustMapPopup();
    }

    /**
     * 取得設定值
     * @param {string} key - 設定的 key 名稱
     * @param {...*} args - 當 key 對應的值是函式時，將 args 作為參數列表傳入該函式
     * @returns {*} - 回傳設定值，或是函式的回傳值
     */
    _getSetting(key, ...args) {
        let setting = this._settingDic[key];
        if (typeof setting === 'function')
            return setting(...args);
        return setting;
    }

    /**
     * 設置Icon網址
     */
    setIconUrl(f) { return this._settingDic['IconUrl'] = f, this; }

    /**
     * 設置Icon大小
     */
    setIconWidth(f) { return this._settingDic['IconWidth'] = f, this; }

    /**
     * 設置Icon大小
     */
    setIconHeight(f) { return this._settingDic['IconHeight'] = f, this; }

    /**
     * 設置經度
     */
    setLongitude(f) { return this._settingDic['Longitude'] = f, this; }

    /**
     * 設置緯度
     */
    setLatitude(f) { return this._settingDic['Latitude'] = f, this; }

    /**
     * 設置群聚顏色
     */
    setClusterColor(f) { return this._settingDic['ClusterColor'] = f, this; }

    /**
     * 設置群聚大小
     * @param {function(number) | *} f - 可以是接收 childCount(number) 的函式，可由childCount動態決定群聚大小
     */
    setClusterSize(f) { return this._settingDic['ClusterSize'] = f, this; }

    /**
     * 設置群聚文字大小
     * @param {function(number) | *} f - 可以是接收 childCount(number) 的函式，可由childCount動態決定群聚文字大小
     */
    setClusterFontSize(f) { return this._settingDic['ClusterFontSize'] = f, this; }

    /**
     * 設置marker click事件
     * @param {function} callbackFunc - callback (接收 marker.data, CustMapPopup instance 為參數)
     */
    setMarkerClick(callbackFunc) {
        let self = this;
        this._markerClickWrapper = function (evt) {
            let marker = evt.sourceTarget;
            let options = { offset: new L.Point(0, -10), minWidth: 300, maxWidth: 390, maxHeight: 320 };
            self._custPopup.setOpen(marker, options);
            callbackFunc(marker.data, self._custPopup);
        };
        return this;
    }

    _createMarkers(pointsJson) {
        let self = this;
        return pointsJson.map(function (item) {
            const icon = L.icon({
                iconUrl: self._getSetting('IconUrl', item),
                iconSize: [self._getSetting('IconWidth', item), self._getSetting('IconHeight', item)],
                iconAnchor: [self._getSetting('IconWidth', item) / 2, self._getSetting('IconHeight', item) / 2],
            });
            let marker = L.marker([self._getSetting('Latitude', item), self._getSetting('Longitude', item)], { icon: icon });
            marker.data = item;
            if (self._markerClickWrapper)
                marker.on('click', self._markerClickWrapper);
            return marker;
        });
    }

    /**
     * 建立群聚
     */
    _createCluster() {
        let self = this;
        return L.markerClusterGroup({
            showCoverageOnHover: false,
            iconCreateFunction: function (cluster) {
                let childCount = cluster.getChildCount();
                let size = self._getSetting('ClusterSize', childCount);
                let color = self._getSetting('ClusterColor', childCount);
                let fontSize = self._getSetting('ClusterFontSize', childCount);
                return L.divIcon({
                    html: `<div class='marker-circle' style='background-color:${color}; font-size:${fontSize};'>${childCount}</div>`,
                    className: 'marker-cluster',
                    iconSize: L.point(size, size)
                });
            }
        });
    }

    /**
     * 加入資料
     */
    addData(pointsJson) {
        let markers = this._createMarkers(pointsJson);
        this._markers = this._markers.concat(markers);
        return this;
    }

    /**
     * 地圖渲染(一般)
     */
    render() {
        this.clearCluster();
        for (let marker of this._markers) {
            this._map.removeLayer(marker);
            marker.addTo(this._map);
        }
    }

    /**
     * 地圖渲染(群聚)
     */
    renderCluster() {
        this.clearCluster();
        this._cluster = this._createCluster();
        for (let marker of this._markers) {
            this._map.removeLayer(marker);
            this._cluster.addLayer(marker);
        }
        this._map.addLayer(this._cluster);
    }

    /**
     * 清除(marker)
     */
    clear(keepData) {
        for (let marker of this._markers)
            this._map.removeLayer(marker);
        if (!keepData)
            this._markers = [];
        this.clearCluster();
        this._custPopup.close();
        return this;
    }

    /**
     * 清除(群聚)
     */
    clearCluster() {
        this._cluster && this._map.removeLayer(this._cluster);
        delete this._cluster;
        return this;
    }
}

class CustMapPopup {
    constructor() {

    }

    /**
     * 顯示內容
     */
    setOpen(marker, options) {
        this._marker = marker;
        this._popup = L.popup(options);
        this._popup.setLatLng(this._marker._latlng);
    }

    /**
     * 顯示內容
     */
    show(content) {
        this._popup.setContent(content);
        this._popup.openOn(this._marker._map);
        this._popup.getElement()?.querySelector('.leaflet-popup-content')?.classList.add('map_popup_div');
    }

    /**
     * 取得DOM
     */
    getElement() {
        return this._popup?.getElement();
    }

    /**
     * 關閉
     */
    close() {
        return this._popup?.close();
    }
}
