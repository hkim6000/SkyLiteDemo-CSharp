function FileDownLoad(t) {
    var data = [{ key: 'FileId', vlu: t }];
    $WaitOn();
    $ApiRequest(this, JSON.stringify(data));
}
