function NavXysSignin() {
    $WaitOn();
    $ApiRequest();
}

function Submit() {
    var pin = document.getElementById('pin');

    var data = [
        { key: 'pin', vlu: pin.value }
    ];
    $WaitOn();
    $ApiRequest(this, JSON.stringify(data));
}
