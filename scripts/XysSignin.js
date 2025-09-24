function NavXysSignup() {
    $WaitOn();
    $ApiRequest();
}

function NavXysPass() {
    var email = document.getElementById('email');

    var data = [
        { key: 'email', vlu: email.value }
    ];
    $WaitOn();
    $ApiRequest(this, JSON.stringify(data));
}
 