
$(document).ready(function () {
    var date_input = $('input[name="date_livraison"]'); //our date input has the name "date"
    var container = $('.bootstrap-iso form').length > 0 ? $('.bootstrap-iso form').parent() : "body";
    var options = {
        format: 'dd/mm/yyyy',
        container: container,
        todayHighlight: true,
        autoclose: true,
        weekStart: 1
    };
    date_input.datepicker(options);


    $("#SelectItemTheme").change(function () {

        var dID = $(this).val();

        $.getJSON("../Commandes/LoadRedacteurByTheme",
            { theme: dID },
            function (data) {
                var select = $("#SelectItemRedacteur");
                select.empty();
                select.append($('<option/>',
                    {
                        value: 0,
                        text: "Selectionner redacteur"
                    }));
                $.each(data,
                    function (index, itemData) {
                        select.append($('<option/>',
                            {
                                value: itemData.Value,
                                text: itemData.Text
                            }));
                    });
            });
    });




});
