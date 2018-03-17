var renderNotifyTable = (function () {

    var eventBinding = function () {

        $('.filter-btn').change(function () {

            $('#table-filter-header').data('filter', $(this).attr('data-filter'));
            $('.dataTables_wrapper').removeClass('table-show');
            var currentFilter = $(this).attr('data-filter');

            $('#' + currentFilter + '-notify-data-table_wrapper').addClass('table-show');
        });

        $(".table-count-area").on("click", ".all-notify-row", function () {

            var notifyInfo = $(this).data('notifyInfo');

            var createDom = '';

            console.log(notifyInfo);

            $.each(notifyInfo, function (key, value) {
                createDom = createDom +'<div class = "notify-detail-row">'+
                                          '<div class = "col-lg-4 col-md-4 col-sm-4 col-xs-4">' + key + '</div>' +
                                          '<div class = "col-lg-8 col-md-8 col-sm-8 col-xs-8">' + value + '</div>' +
                                        '</div>'
            });
            $('.modal-body').html(createDom);
        });

    }

    var init = function () {
        eventBinding();
    };

    renderTable = function (notifyInfo, status) {

        if (status === 'email') {

            var emailCounting = $('.email-notify-row').length;

            emailNotifyTable.row.add([
                '<span>' + (emailCounting + 1) + '</span>',
                '<span>' + notifyInfo.Sender + '</span>',
                '<span>' + notifyInfo.Receiver + '</span>',
                '<span>' + notifyInfo.Subject + '</span>',
                '<span>' + notifyInfo.DateTime + '</span>',
            ]);

            emailNotifyTable.rows(emailCounting).nodes().to$().addClass('email-notify-row notify-row');
            emailNotifyTable.columns.adjust().draw();

        } else if (status === 'sms') {

            var smsCounting = $('.sms-notify-row').length;

            smsNotifyTable.row.add([
                '<span>' + (smsCounting + 1) + '</span>',
                '<span>' + notifyInfo.Sender + '</span>',
                '<span>' + notifyInfo.Receiver + '</span>',
                '<span>' + notifyInfo.Content + '</span>',
                '<span>' + notifyInfo.DateTime + '</span>',
            ]);

            smsNotifyTable.rows(smsCounting).nodes().to$().addClass('sms-notify-row notify-row');
            smsNotifyTable.columns.adjust().draw();

        } else if (status === 'erp') {

            var erpCounting = $('.erp-notify-row').length;

            erpNotifyTable.row.add([
                '<span>' + (erpCounting + 1) + '</span>',
                '<span>' + notifyInfo.equipmentId + '</span>',
                '<span>' + notifyInfo.equipmentName + '</span>',
                '<span>' + notifyInfo.eventCode + '</span>',
                '<span>' + notifyInfo.eventMessage + '</span>',
            ]);

            erpNotifyTable.rows(erpCounting).nodes().to$().addClass('erp-notify-row notify-row');
            erpNotifyTable.columns.adjust().draw();
        } else if (status === 'mes') {

            var mesCounting = $('.mes-notify-row').length;

            mesNotifyTable.row.add([
                '<span>' + (mesCounting + 1) + '</span>',
                '<span>' + notifyInfo.Sender + '</span>',
                '<span>' + notifyInfo.Receiver + '</span>',
                '<span>' + notifyInfo.Content + '</span>',
                '<span>' + notifyInfo.DateTime + '</span>',
            ]);

            mesNotifyTable.rows(mesCounting).nodes().to$().addClass('mes-notify-row notify-row');
            mesNotifyTable.columns.adjust().draw();

        }

        //update all notify table

        var allCounting = $('.all-notify-row').length;

        allNotifyTable.row.add([
            '<span>' + (allCounting + 1) + '</span>',
            '<span>' + status + '</span>',
            '<span>' + notifyInfo.DateTime + '</span>',
            '<span class="prevent-long">' + JSON.stringify(notifyInfo) + '</span>',
        ]);
        allNotifyTable.rows(allCounting).nodes().to$().addClass('all-notify-row notify-row').attr({
            'data-toggle': 'modal',
            'data-target': '#notify-info'
        }).data('notifyInfo' , notifyInfo);
        allNotifyTable.columns.adjust().draw();
    };



    // Public API
    return {
        init: init,
        renderTable: renderTable,
    };
})();