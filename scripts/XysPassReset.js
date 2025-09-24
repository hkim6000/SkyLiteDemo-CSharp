function NavXysSignin() {
    $WaitOn();
    $ApiRequest();
}

function NavXysSent() {
    var email = document.getElementById('email');

    var data = [
        { key: 'email', vlu: email.value }
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
    var btns1 = document.getElementsByClassName('button1');
    for (var i = 0; i < btns1.length; i++) {
        btns1[i].style.display = 'none';
    }
}
function ShowButtons() {
    var btns = document.getElementsByClassName('button');
    for (var i = 0; i < btns.length; i++) {
        btns[i].style.display = '';
    }
    var btns1 = document.getElementsByClassName('button1');
    for (var i = 0; i < btns1.length; i++) {
        btns1[i].style.display = '';
    }
}

