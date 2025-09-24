function NavXysPassReset() {
    $WaitOn();
    $ApiRequest();
}
function NavXysSignIn() {
    $WaitOn();
    $ApiRequest();
}

function NavXysHome() {
    var pass = document.getElementById('pass');

    var data = [
        { key: 'pass', vlu: pass.value }
    ];
    $WaitOn();
    $ApiRequest(this, JSON.stringify(data));
}
