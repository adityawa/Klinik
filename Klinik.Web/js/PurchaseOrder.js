var Klinik = {
    init: function () {
        Klinik.addPuschaseOrderDetailItem();
        Klinik.autocompleteProductOne();
        Klinik.savePurchaseOrder();
        Klinik.editPurchaseOrderDetail();
        Klinik.searchpurchacerequest();
        Klinik.duplicaterowtable();
        Klinik.checkall();
        Klinik.checkbox();
        Klinik.savePoPerRow();
    },

    autocompleteProductOne: function () {
        var el = $("#namabarang");
        if (!el.length) return;

        $('#namabarang').select2({
            width: 'resolve',
            placeholder: 'product..',
            ajax: {
                url: '/DeliveryOrder/searchproduct/',
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
                            text: item.Name
                        });
                    });
                    return {
                        results: results
                    };
                }
            }
        });
        $(el).change(function () {
            $("#ProductId").val($(el).val());
        });
    },

    addPuschaseOrderDetailItem: function () {
        var el = $("#btnAdd");
        if (!el.length) return;
        $("body").on("click", "#btnAdd", function () {
            var PurchaseOrderDetailId = $("#PurchaseOrderDetailId");
            var tot_pemakaian = $("#tot_pemakaian");
            var ProductId = $("#ProductId");
            var namabarang = $("#namabarang");
            var sisa_stok = $("#sisa_stok");
            var qty = $("#qty");
            var qty_add = $("#qty_add");
            var reason_add = $("#reason_add");
            var total = $("#total");
            var nama_by_ho = $("#nama_by_ho");
            var qty_by_ho = $("#qty_by_ho");
            var remark_by_ho = $("#remark_by_ho");
            var tBody = $("#tblPurchaseOrder > TBODY")[0];

            //Add Row.
            var row = tBody.insertRow(-1);
            //Add id cell.
            var cell = $(row.insertCell(-1));
            cell.html(PurchaseOrderDetailId.val());
            cell.hide();

            cell = $(row.insertCell(-1));
            cell.html(ProductId.val());
            cell.hide();

            cell = $(row.insertCell(-1));
            cell.html(namabarang.text());

            cell = $(row.insertCell(-1));
            cell.html(tot_pemakaian.val());

            cell = $(row.insertCell(-1));
            cell.html(sisa_stok.val());

            cell = $(row.insertCell(-1));
            cell.html(qty.val());

            cell = $(row.insertCell(-1));
            cell.html(qty_add.val());

            cell = $(row.insertCell(-1));
            cell.html(reason_add.val());

            cell = $(row.insertCell(-1));
            cell.html(total.val());

            cell = $(row.insertCell(-1));
            cell.html(nama_by_ho.val());

            cell = $(row.insertCell(-1));
            cell.html(qty_by_ho.val());

            cell = $(row.insertCell(-1));
            cell.html(remark_by_ho.val());

            cell = $(row.insertCell(-1));
            var btnRemove = $("<input />");
            btnRemove.attr("type", "button");
            btnRemove.attr("onclick", "Remove(this);");
            btnRemove.val("Remove");
            cell.append(btnRemove);

            $('#purchaseorderdetail input[type="text"], textarea').val('');
        });
    },

    savePurchaseOrder: function () {
        $('.savepurchasedetail').on('click', function () {
            var _purchaseorder = {};
            _purchaseorder.Id = $('#Id').val();
            _purchaseorder.ponumber = $('#ponumber').val();
            _purchaseorder.PurchaseRequestId = $('#PurchaseRequestId').val();
            _purchaseorder.podate = $('#podate').val();
            _purchaseorder.request_by = $('#request_by').val();
            var purchaseOrderDetailModels = new Array();
            $("#tblPurchaseOrder TBODY TR").each(function (item, key) {
                var row = $(this);
                var purchaseOrderDetail = {};
                purchaseOrderDetail.Id = row.find("TD").eq(1).html();
                purchaseOrderDetail.ProductId = row.closest('tr').find('td:eq(3) select').val() > 0 ? row.closest('tr').find('td:eq(3) select').val() : row.find("TD").eq(2).html();
                purchaseOrderDetail.tot_pemakaian = row.closest('tr').find('td:eq(4) input').length > 0 ? row.closest('tr').find('td:eq(4) input').val() : row.find("TD").eq(4).html();
                purchaseOrderDetail.sisa_stok = row.closest('tr').find('td:eq(5) input').length > 0 ? row.closest('tr').find('td:eq(5) input').val() : row.find("TD").eq(5).html();
                purchaseOrderDetail.qty = row.closest('tr').find('td:eq(6) input').length > 0 ? row.closest('tr').find('td:eq(6) input').val() : row.find("TD").eq(6).html();
                purchaseOrderDetail.qty_add = row.closest('tr').find('td:eq(7) input').length > 0 ? row.closest('tr').find('td:eq(7) input').val() : row.find("TD").eq(7).html();
                purchaseOrderDetail.reason_add = row.closest('tr').find('td:eq(8) input').length > 0 ? row.closest('tr').find('td:eq(8) input').val() : row.find("TD").eq(8).html();
                purchaseOrderDetail.total = row.closest('tr').find('td:eq(9) input').length > 0 ? row.closest('tr').find('td:eq(9) input').val() : row.find("TD").eq(9).html();
                purchaseOrderDetail.nama_by_ho = row.closest('tr').find('td:eq(10) input').length > 0 ? row.closest('tr').find('td:eq(10) input').val() : row.find("TD").eq(10).html();
                purchaseOrderDetail.qty_by_ho = row.closest('tr').find('td:eq(11) input').length > 0 ? row.closest('tr').find('td:eq(11) input').val() : row.find("TD").eq(11).html();
                purchaseOrderDetail.remark_by_ho = row.closest('tr').find('td:eq(12) input').length > 0 ? row.closest('tr').find('td:eq(12) input').val() : row.find("TD").eq(12).html();
                purchaseOrderDetail.Verified = row.closest('tr').find('td:eq(13) input[type="checkbox"]').val();
                purchaseOrderDetailModels.push(purchaseOrderDetail);
            });
            console.log(purchaseOrderDetailModels);
            purchaseOrderDetailModels.sort();
            $.ajax({
                type: "POST",
                url: $(this).data('url'),
                data: JSON.stringify({ _purchaseorder: _purchaseorder, purchaseOrderDetailModels: purchaseOrderDetailModels }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    window.location.reload();
                }
            });
        });
    },

    editPurchaseOrderDetail: function () {
        $('.edit-purchaseorderdetail').on('click', function () {
            var getdata = $(this).closest('tr');
            getdata.find('td:eq(3) select').select2({
                width: 'resolve',
                placeholder: 'product..',
                ajax: {
                    url: '/DeliveryOrder/searchproduct/',
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
                                text: item.Name
                            });
                        });
                        return {
                            results: results
                        };
                    }
                }
            });

            $('input[type="checkbox"]').prop('disabled', false);
            $(this).hide();
            getdata.find('.save-purchaseorderdetail').show();
        });
    },

    searchpurchacerequest: function () {
        var el = $("#PurchaseRequestId");
        if (!el.length) return;

        $('#PurchaseRequestId').select2({
            width: 'resolve',
            placeholder: 'purchacerequest..',
            ajax: {
                url: '/General/searchpurchaserequest/',
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
                        if (item.approve >= 1) {
                            results.push({
                                id: item.Id,
                                text: item.prnumber
                            });
                        } else {
                            return true;
                        }
                    });
                    return {
                        results: results
                    };
                }
            }
        });
    },

    duplicaterowtable: function () {
        var el = $(".subtitusi");
        if (!el.length) return;

        $(el).on('click', function () {
            var $tr = $(this).closest('tr');
            var $clone = $tr.clone();
            $clone.find(".podetail").text('');
            $tr.after($clone);
            Klinik.editPurchaseOrderDetail();
            Klinik.autocompleteProductOne();
        });
    },

    checkall: function () {
        var el = $('.checkall');
        if (!el.length) return;

        $(el).click(function () {
            if ($(this).prop("checked") == true) {
                $('input:checkbox').not(this).prop('checked', this.checked).val('true');
            } else {
                $('input:checkbox').not(this).prop('checked', this.checked).val('false');
            }
        });
    },

    checkbox: function () {
        var el = $('input[type="checkbox"]');
        $(el).click(function () {
            if ($(this).prop("checked") == true) {
                $(this).prop('checked', this.checked).val('true');
            } else {
                $(this).prop('checked', this.checked).val('false');
            }
        });
    },

    savePoPerRow: function () {
        var el = $(".save-purchaseorderdetail");
        if (!el.length) return;

        $(el).on('click', function () {
            $(this).hide();
            var row = $(this).closest('tr');
            row.find('.delete-purchaseorderdetail').hide();
            row.find('.image-loading').show();
            var purchaseOrderDetail = {};
            purchaseOrderDetail.Id = row.find("TD").eq(1).html();
            purchaseOrderDetail.ProductId = row.closest('tr').find('td:eq(3) select').val() > 0 ? row.closest('tr').find('td:eq(3) select').val() : row.find("TD").eq(2).html();
            purchaseOrderDetail.tot_pemakaian = row.closest('tr').find('td:eq(4) input').length > 0 ? row.closest('tr').find('td:eq(4) input').val() : row.find("TD").eq(4).html();
            purchaseOrderDetail.sisa_stok = row.closest('tr').find('td:eq(5) input').length > 0 ? row.closest('tr').find('td:eq(5) input').val() : row.find("TD").eq(5).html();
            purchaseOrderDetail.qty = row.closest('tr').find('td:eq(6) input').length > 0 ? row.closest('tr').find('td:eq(6) input').val() : row.find("TD").eq(6).html();
            purchaseOrderDetail.qty_add = row.closest('tr').find('td:eq(7) input').length > 0 ? row.closest('tr').find('td:eq(7) input').val() : row.find("TD").eq(7).html();
            purchaseOrderDetail.reason_add = row.closest('tr').find('td:eq(8) input').length > 0 ? row.closest('tr').find('td:eq(8) input').val() : row.find("TD").eq(8).html();
            purchaseOrderDetail.total = row.closest('tr').find('td:eq(9) input').length > 0 ? row.closest('tr').find('td:eq(9) input').val() : row.find("TD").eq(9).html();
            purchaseOrderDetail.nama_by_ho = row.closest('tr').find('td:eq(10) input').length > 0 ? row.closest('tr').find('td:eq(10) input').val() : row.find("TD").eq(10).html();
            purchaseOrderDetail.qty_by_ho = row.closest('tr').find('td:eq(11) input').length > 0 ? row.closest('tr').find('td:eq(11) input').val() : row.find("TD").eq(11).html();
            purchaseOrderDetail.remark_by_ho = row.closest('tr').find('td:eq(12) input').length > 0 ? row.closest('tr').find('td:eq(12) input').val() : row.find("TD").eq(12).html();
            purchaseOrderDetail.Verified = row.closest('tr').find('td:eq(13) input[type="checkbox"]').val();
            $.ajax({
                type: "POST",
                url: $(this).data('url'),
                data: JSON.stringify({ purchaseOrderDetail: purchaseOrderDetail }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    $('.image-loading').hide();
                    row.find('.edit-purchaseorderdetail').show();
                    row.find('.delete-purchaseorderdetail').show();
                }
            });
        });
    }


};
$(document).ready(function () {
    Klinik.init();
});