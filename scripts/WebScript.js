$ClickListener.push($ClsPop);
function txtblur(event, obj) {
    if (obj.value == '') {
        obj.style.border = '2px solid #E4702F';
    } else {
        obj.style.border = '';
    }
}
var XysHome = 'XysHome';
function Profile() {
    $WaitOn();
    $$ApiRequest(XysHome, this, null);
}
function SignOut() {
    $WaitOn();
    $$ApiRequest(XysHome, this, null);
}
function UpdatePhoto(f, s,  p) {
    var data = [
        { key: 'f', vlu: f },
        { key: 's', vlu: s },
        { key: 'p', vlu: p }
    ];
    $WaitOn();
    $$ApiRequest('WebBase', this, JSON.stringify(data));
}