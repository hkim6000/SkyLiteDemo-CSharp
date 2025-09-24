var XysProPass = 'XysProPass';
function PassSave() {
    var pwd = document.getElementById('pwd');
    var pwd1 = document.getElementById('pwd1');

    var data = [
        { key: 'pwd', vlu: pwd.value },
        { key: 'pwd1', vlu: pwd1.value }
    ];

    XysProPassHideButtons();

    $WaitOn();
    $$ApiRequest(XysProPass, this, JSON.stringify(data));
}

function XysProPassHideButtons() {
    var btns = document.getElementsByClassName('button');
    for (var i = 0; i < btns.length; i++) {
        btns[i].style.display = 'none';
    }
}
function XysProPassShowButtons() {
    var btns = document.getElementsByClassName('button');
    for (var i = 0; i < btns.length; i++) {
        btns[i].style.display = '';
    }
}