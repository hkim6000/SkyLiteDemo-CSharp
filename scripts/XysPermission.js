function SetRoleRange(f, d, c, t, s) {
    var data = [
        { key: 'd', vlu: d },
        { key: 'c', vlu: c },
        { key: 't', vlu: t },
        { key: 's', vlu: (s == true) ? '1' : '0' }
    ];
    $WaitOn();
    $ApiRequest(f, JSON.stringify(data));
} 