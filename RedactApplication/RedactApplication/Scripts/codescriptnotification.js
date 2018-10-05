$(document).ready(function () {


    $(function () {
        // Declare a proxy to reference the hub.
        var notifications = $.connection.messagesHub;
       
        //debugger;
        // Create a function that the hub can call to broadcast messages.
        notifications.client.sendMessages = function () {
            console.log("connecter updateMessages");
            getAllMessages();

        };
        // Start the connection.
        $.connection.hub.start().done(function () {
            console.log("connecter");
            getAllMessages();
        }).fail(function (e) {
            alert(e);
        });
    });

    function getAllMessages()
    {
        var tbl = $('#messagesTable');
        $('#notiContent').empty();
        $('#notiContent').append('<li>...</li>');

        $.ajax({
            type: 'GET',
            url: '/Commandes/GetNotifications',
            contentType: "application/json; charset=utf-8",
            dataType: "json",


            success: function(response) {
                console.log(response.d);
                $('#notiContent').empty();
                if (response.length === 0) {
                    
                    $('#notiContent').append($('<li>Aucune nouvelle notification.</li>'));
                }
                else {
                    $('#notification-bell').append($('<span class="count-notif">' + response.length +'</span>'));               

                    var tr_str = '';
                    $.each(response, function (index, value) {
                        var divrole = $("#divrole").data('value');
                        console.log(divrole);
                        var link = $(location).attr('host') + "/Commandes/DetailsCommande/" + value.commandeId + "?not=" + value.notificationId;
                        if (divrole == "2")
                         link = $(location).attr('host') + "/Commandes/DetailsCommandeAValider/" + value.commandeId + "?not=" + value.notificationId;

                        var dateStr = parseDate(value.datenotif);
                        
                        //tr_str += '<li class="recent"><a href="' + link + '" id="submit-link"> La commande #' + value.commanderef + ' a été mise à jour par ' + value.fromUserName + ' le ' + dateStr + ' .</a></li>';

                        tr_str += '<li class="recent"><a href="http://'+ link +'"> ' + value.message + '</a></li>';
                    });
                
                    $('#notiContent').html(tr_str);
                }
            },
            error: function(error) {
                console.log(error);
            }
        });
    }

    function parseDate(value) {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        var dt = new Date(parseFloat(results[1]));
        return dt.getDate() +"/"+ (dt.getMonth() + 1) + "/" +  dt.getFullYear();

    }

    function RedirectCommande(value) {
        alert(value);
        window.location.href = value;

    }


});
    