jQuery(document).ready(function () {
    //TEAM LOGO
    jQuery("#teamlogourl").ready(function () {
        jQuery("#teamlogoimg").attr("src", jQuery("#teamlogourl").val());
    });

    jQuery("#teamlogourl").blur(function () {
        jQuery("#teamlogoimg").attr("src", jQuery("#teamlogourl").val());
    });

    //PLAYER LOGO
    jQuery("#playerimageurl").ready(function () {
        jQuery("#playerimageimg").attr("src", jQuery("#playerimageurl").val());
    });

    jQuery("#playerimageurl").blur(function () {
        jQuery("#playerimageimg").attr("src", jQuery("#playerimageurl").val());
    });

    //NOTIFICATION CLOSE BUTTON
    jQuery('.notification .close').click(function () {
        jQuery(this).parent().fadeOut();
    });
});