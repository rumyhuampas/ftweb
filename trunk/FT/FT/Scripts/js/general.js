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

    //CHAMP TEAMS
    jQuery("#champtype").ready(function () {
        jQuery.get(location.protocol + '//' + location.host + '/Championship/GetChampTeams', { champType: jQuery("#champtype").val() }, function (data) {
            jQuery('#champteams').empty();
            jQuery.each(data, function (key, value) {
                jQuery('#champteams').append('<option value="' + value.Value + '">' + value.Text + '</option>');
            });
            jQuery.get(location.protocol + '//' + location.host + '/Championship/GetSelectedType', null, function (data) {
                jQuery("#champtype").val(data);
                //TODO: add change code here
            });
        });
    });


    jQuery("#champtype").change(function () {
        jQuery('#champteams').empty();
        jQuery('#dyntable tbody').empty();
        jQuery.get(location.protocol + '//' + location.host + '/Championship/GetChampTeams', { champType: jQuery("#champtype").val() }, function (data) {
            jQuery.each(data, function (key, value) {
                jQuery('#champteams').append('<option value="' + value.Value + '">' + value.Text + '</option>');
            });
            jQuery.get(location.protocol + '//' + location.host + '/Championship/SetSelectedType', { champType: jQuery("#champtype").val() }, function (data) { });
        });
        /*jQuery.get(location.protocol + '//' + location.host + '/Championship/CleanSelectedTeams', null, function (data) {
            
        });*/
    });
});