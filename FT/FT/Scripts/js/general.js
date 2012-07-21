jQuery(document).ready(function () {
    jQuery("#teamlogourl").blur(function () {
        jQuery("#teamlogoimg").attr("src", jQuery("#teamlogourl").val());
    });

    jQuery("#playerimageurl").blur(function () {
        jQuery("#playerimageimg").attr("src", jQuery("#playerimageurl").val());
    });

    //NOTIFICATION CLOSE BUTTON
    jQuery('.notification .close').click(function () {
        jQuery(this).parent().fadeOut();
    });
});