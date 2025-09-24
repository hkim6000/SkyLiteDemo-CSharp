function NavXysSignin() {
    var pwd = document.getElementById('pwd');
    var pwd1 = document.getElementById('pwd1');

    var data = [
        { key: 'pwd', vlu: pwd.value },
        { key: 'pwd1', vlu: pwd1.value }
    ];

    HideButtons();

    $WaitOn();
    $ApiRequest(this, JSON.stringify(data));
}

function HideButtons() {
    var btns = document.getElementsByClassName('button');
    for (var i = 0; i < btns.length; i++) {
        btns[i].style.display = 'none';
    }
}
function ShowButtons() {
    var btns = document.getElementsByClassName('button');
    for (var i = 0; i < btns.length; i++) {
        btns[i].style.display = '';
    }
}

