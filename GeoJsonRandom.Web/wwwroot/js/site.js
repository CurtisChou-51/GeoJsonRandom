
const uiHelper = (function () {
    Notiflix.Report.init({
        backOverlayClickToClose: true,
        svgSize: '50px',
        borderRadius: '15px',
        titleFontSize: '22px',
        messageFontSize: '16px',
        success: { backOverlayColor: 'rgba(0,0,0,0.3)' },
        failure: { backOverlayColor: 'rgba(0,0,0,0.3)' },
        warning: { backOverlayColor: 'rgba(0,0,0,0.3)' },
        info: { backOverlayColor: 'rgba(0,0,0,0.3)' }
    });
    function loadingStart() { Notiflix.Loading.circle(); }
    function loadingEnd() { Notiflix.Loading.remove(); }
    function success(title, message) { Notiflix.Report.success(title, message, '確定'); }
    function failure(title, message) { Notiflix.Report.failure(title, message, '確定'); }
    function warning(title, message) { Notiflix.Report.warning(title, message, '確定'); }
    function info(title, message) { Notiflix.Report.info(title, message, '確定'); }
    function alert(title, message) { Notiflix.Report.info(title, message, '確定', { svgSize: '0px' }); }
    function report(result) {
        let alertTitle = result.AlertTitle || '系統訊息';
        switch (result.AlertType) {
            case "success":
                return success(alertTitle, result.AlertMessage);
            case "failure":
                return failure(alertTitle, result.AlertMessage);
            case "warning":
                return warning(alertTitle, result.AlertMessage);
            case "info":
            default:
                return info(alertTitle, result.AlertMessage);
        }
    }
    return {
        loadingStart: loadingStart,
        loadingEnd: loadingEnd,
        success: success,
        failure: failure,
        warning: warning,
        info: info,
        alert: alert,
        report: report
    };
})();

const ajaxHelper = (function () {
    let _token = null, _defaultConfig = { url: '', type: 'POST', data: null };
    function processRespPromise(opt) {
        return new Promise((resolve, reject) =>
            $.ajax(opt).done(function (res) {
                if (res.AlertMessage)
                    uiHelper.report(res);

                // 不繼續執行後續動作
                if (typeof (res) === "object" && res.Success === false)
                    return resolve({ ReqFail: true });

                resolve(res);
            }).fail(function (jqXHR, textStatus, errorThrown) {
                uiHelper.failure('非預期錯誤', `${jqXHR.status} ${textStatus || ""}\n${jqXHR.responseText || ""}`);
                resolve({ ReqFail: true });
            })
        );
    }

    function req(opt) {
        opt.headers = opt.headers || {};
        if (_token)
            opt.headers["RequestVerificationToken"] = _token;
        return processRespPromise(opt);
    }
    function post(url, data, options) {
        let opt = $.extend(true, {}, _defaultConfig, options || {});
        opt.url = url;
        opt.data = data;
        opt.type = 'POST';
        return req(opt);
    }
    function postJson(url, data, options) {
        let opt = $.extend(true, {}, _defaultConfig, options || {
            dataType: "json",
            contentType: "application/json;charset=utf-8"
        });
        opt.url = url;
        opt.data = data;
        opt.type = 'POST';
        return req(opt);
    }
    function get(url, data, options) {
        let opt = $.extend(true, {}, _defaultConfig, options || {});
        opt.url = url;
        opt.data = data;
        opt.type = 'GET';
        return req(opt);
    }
    function uploadFile(url, data, options) {
        let opt = $.extend(true, {}, _defaultConfig, options || {
            cache: false,
            contentType: false,
            processData: false
        });
        opt.url = url;
        opt.data = data;
        opt.type = 'POST';
        return req(opt);
    }

    function setAntiForgeryToken(token) { return _token = token, this; }
    function getAntiForgeryToken() { return _token; }

    return {
        get: get,
        post: post,
        postJson: postJson,
        uploadFile: uploadFile,
        setAntiForgeryToken: setAntiForgeryToken,
        getAntiForgeryToken: getAntiForgeryToken
    };
})();
