var general = {
    in: function () {
        general.searchpurchaceorder();
    },
    searchpurchaceorder: function () {
        var el = $("#ponumber");
        if (!el.length) return;

        $('#ponumber').select2({
            width: 'resolve',
            placeholder: 'po..',
            ajax: {
                url: '/General/searchpurchaseorder/',
                data: function (params) {
                    return {
                        prefix: params.term
                    };
                },
                dataType: 'json',
                delay: 250,
                processResults: function (data) {
                    var results = [];

                    $.each(data.data, function (index, item) {
                        results.push({
                            id: item.Id,
                            text: item.ponumber
                        });
                    });
                    return {
                        results: results
                    };
                }
            }
        });
    }
};

$(document).ready(function () {
    general.in();
});