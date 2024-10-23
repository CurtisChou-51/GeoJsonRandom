
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
                let minSize = 35, maxSize = 75, rate = 0.3;
                return Math.min(minSize + childCount * rate, maxSize);
            },
            'ClusterFontSize': childCount => {
                let minSize = 15, maxSize = 30, rate = 0.03;
                return Math.min(minSize + childCount * rate, maxSize);
            }
        }
        this._markers = [];
        let self = this;
        self._custPopup = new CustMapPopup();
    }

    _getSetting(attr, item) {
        let setting = this._settingDic[attr];
        if (typeof setting === 'function')
            return setting(item);
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
     */
    setClusterSize(f) { return this._settingDic['ClusterSize'] = f, this; }

    /**
     * 設置群聚文字大小
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
                let size = self._getSetting('ClusterSize', childCount).toFixed(2);
                let color = self._getSetting('ClusterColor', childCount);
                let fontSize = self._getSetting('ClusterFontSize', childCount).toFixed(2);
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
