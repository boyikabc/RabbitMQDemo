var jkModule = function (vip) {
    var yvip = document.getElementById(vip);
    return {
        add: function (t) {
            if (t > 12) {
                var yv = t;
                yvip.innerHTML = "年费" + yv;
            } else {
                var mv = t;
                yvip.innerHTML = "月份"+mv;
            }
        }
    }
}
var jm = new jkModule("vip");

jm.add(12);