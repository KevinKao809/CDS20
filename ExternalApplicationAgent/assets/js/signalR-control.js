var signalRController = (function () {


    var updateCard = function (status) {

        var target;

        switch (status) {
            case 'email':
                target = $('#mail-count-number');
                break;
            case 'sms':
                target = $('#sms-count-number');
                break;
            case 'erp':
                target = $('#erp-count-number');
                break;
        }

        if (!target.data('count')) {
            target.data('count', 1);
        } else {
            var value = parseInt(target.data('count'));
            target.data('count', (value + 1));
        }

        target.text(target.data('count'));

    };

    var updateTable = function (status, info) {
        var infoObject = $.parseJSON(info);
        renderNotifyTable.renderTable(infoObject, status);
    };

    var init = function (status) {

        var hub = $.connection.RTMessageHub;
        hub.client.onReceivedMessage = function (message) {
            console.log('onReceivedMessage:', message);
            //alert(message);
        };
        hub.client.onSendEmailResult = function (message) {
            console.log('onSendEmailResult:', message);


            updateCard('email');
            updateTable('email', message);
            //alert(message);
        };

        hub.client.onSendSMSResult = function (message) {

            console.log('onSendSMSResult:', message);
            updateCard('sms');
            updateTable('sms', message);
            //alert(message);
        };
        hub.client.onSendERPResult = function (message) {
            console.log('onSendERPResult:', message);
            updateCard('erp');
            updateTable('erp', message);
            //alert(message);
        };
        /**
        hub.client.onSendEmailResult = function (message) {
            console.log('onSendEmailResult:', message);
            this.updateCard('sms');
            // alert(message);
        };
        */

        $.connection.hub.start().done(function () {
            hub.server.register();
        });


    };

    // Public API
    return {
        init: init
    };
})();