function CallReport(t) {
    var data = [
        { key: 'rpt', vlu: t.value }
    ];
    $WaitOn();
    $ApiRequest(this, JSON.stringify(data));
}
