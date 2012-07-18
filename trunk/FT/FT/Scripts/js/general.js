jQuery(document).ready(function () {
    jQuery("#teamlogourl").blur(function () {
        jQuery("#teamlogoimg").attr("src", jQuery("#teamlogourl").val());
    });
});