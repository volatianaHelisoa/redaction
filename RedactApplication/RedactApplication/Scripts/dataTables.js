$(document).ready(function () {
    $('.usr-srch--input').on('search', function (e) {
        if ('' == this.value) {
            jQuery(this).trigger('keyup');
        }
    });

    $('.usr-srch').submit(function (e) {
        e.preventDefault();
    })

    $(".cancelText").click(function () {
        $('input[type=search]').val("");
        $('input[type=search]').trigger('keyup');
    })

    $('.usr-srch--input').keyup(function () {
        var _val = $(this).val();
        $('input[type=search]').val(_val);
        $('.dataTables_filter input[type=search]').keyup();

        if (_val.length > 0)
            $(".cancelText").css('visibility', 'visible')
        else
            $(".cancelText").css('visibility', 'hidden')
    });

    $('.facture-srch').submit(function (e) {
        
        e.preventDefault();
    })



    
    if (jQuery('#myDataTable').length > 0) {
        $('#myDataTable').dataTable({
            "language": {
                "url": "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/French.json"
            },
            "searching": true,
            "paging": true,
            "ordering": true,
            "info": false,
            columnDefs: [{
                orderable: false,
                className: 'select-checkbox',
                targets: 0
            }],
            select: {
                style: 'os',
                selector: 'td:first-child'
            },
            order: [[1, 'asc']],
            initComplete: function () {
                this.api().columns().every(function () {
                    var column = this;

                    if (column.selector.cols == 4) {
                        var select = $('<select><option value=""></option></select>')
                            .appendTo($(column.footer()).empty())
                            .on('change', function () {
                                var val = $.fn.dataTable.util.escapeRegex(
                                    $(this).val()
                                );

                                column
                                    .search(val ? '^' + val + '$' : '', true, false)
                                    .draw();
                            });

                        column.data().unique().sort().each(function (d, j) {
                            select.append('<option value="' + d + '">' + d + '</option>')
                        });


                    }

                });
            }
        });
        //$('#myDataTableListCommande').DataTable({
        //    "dom": '<"top"f>rt<"bottom"ipl><"clear">',
        //    "language": {
        //        "search": '<i class="fa fa-search"></i>',
        //        "searchPlaceholder": "search",
        //        "emptyTable": "Aucun enregistrement trouvé.",
        //        "zeroRecords": "Aucun enregistrement trouvé.",
        //        "url": "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/French.json"
        //    },
            
        //    "searching": false,
        //    "paging": true,
        //    "ordering": false,
        //    "info": false,
        //    columnDefs: [{
        //        orderable: true,
        //        className: 'select-checkbox',
        //        targets: 0
        //    }],
        //    select: {
        //        style: 'os',
        //        selector: 'td:first-child'
        //    },
        //    order: [[1, 'asc']]
        //});
      
    }
    
    //if (jQuery('#myDataTable').length > 0) {
    //    $('#myDataTable').dataTable({
    //        "dom": '<"top"f>rt<"bottom"ipl><"clear">',
    //        "language": {
    //            "search": '<i class="fa fa-search"></i>',
    //            "searchPlaceholder": "search",
    //            "emptyTable": "No records found.",
    //            "zeroRecords": "No records found."
    //        },
    //        "stripeClasses": ['odd-row', 'even-row'],

    //        initComplete: function () {
    //            this.api().columns().every(function () {
    //                var column = this;

    //                if (column.selector.cols == 4) {
    //                    var select = $('<select><option value=""></option></select>')
    //                        .appendTo($(column.footer()).empty())
    //                        .on('change', function () {
    //                            var val = $.fn.dataTable.util.escapeRegex(
    //                                $(this).val()
    //                            );

    //                            column
    //                                .search(val ? '^' + val + '$' : '', true, false)
    //                                .draw();
    //                        });

    //                    column.data().unique().sort().each(function (d, j) {
    //                        select.append('<option value="' + d + '">' + d + '</option>')
    //                    });


    //                }

    //            });
    //        }
    //    });
    //}
})