jQuery(document).ready(function () {
    jQuery("#d").click(function () {
        alert(window.location.pathname + "/Team/ChooseLogo");
        jQuery.get(location.protocol + '//' + location.host +"/Team/ChooseLogo", function (data) {
            alert(data);
        });
    });
});