﻿var Klinik = {
    init: function () {
        Klinik.addPuschaseOrderDetailItem();
        Klinik.autocompleteProductOne();
        Klinik.savePurchaseOrder();
        Klinik.editPurchaseOrderDetail();
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
            var PurchaseRequestDetailId = $("#PurchaseRequestDetailId");
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
            cell.html(PurchaseRequestDetailId.val());
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
            var _purchaserequest = {};
            _purchaserequest.Id = $('#Id').val();
            _purchaserequest.prnumber = $('#prnumber').val();
            _purchaserequest.prdate = $('#prdate').val();
            _purchaserequest.request_by = $('#request_by').val();
            var purchaserequestDetailModels = new Array();
            $("#tblPurchaseOrder TBODY TR").each(function () {
                var row = $(this);
                var purchaseRequestDetail = {};
                purchaseRequestDetail.Id = row.find("TD").eq(0).html();
                purchaseRequestDetail.ProductId = row.closest('tr').find('td:eq(2) select').val() > 0 ? row.closest('tr').find('td:eq(2) select').val() : row.find("TD").eq(1).html();
                purchaseRequestDetail.tot_pemakaian = row.closest('tr').find('td:eq(3) input').length > 0 ? row.closest('tr').find('td:eq(3) input').val() : row.find("TD").eq(3).html();
                purchaseRequestDetail.sisa_stok = row.closest('tr').find('td:eq(4) input').length > 0 ? row.closest('tr').find('td:eq(4) input').val() : row.find("TD").eq(4).html();
                purchaseRequestDetail.qty = row.closest('tr').find('td:eq(5) input').length > 0 ? row.closest('tr').find('td:eq(5) input').val() : row.find("TD").eq(5).html();
                purchaseRequestDetail.qty_add = row.closest('tr').find('td:eq(6) input').length > 0 ? row.closest('tr').find('td:eq(6) input').val() : row.find("TD").eq(6).html();
                purchaseRequestDetail.reason_add = row.closest('tr').find('td:eq(7) input').length > 0 ? row.closest('tr').find('td:eq(7) input').val() : row.find("TD").eq(7).html();
                purchaseRequestDetail.total = row.closest('tr').find('td:eq(8) input').length > 0 ? row.closest('tr').find('td:eq(8) input').val() : row.find("TD").eq(8).html();
                purchaseRequestDetail.nama_by_ho = row.closest('tr').find('td:eq(9) input').length > 0 ? row.closest('tr').find('td:eq(9) input').val() : row.find("TD").eq(9).html();
                purchaseRequestDetail.qty_by_ho = row.closest('tr').find('td:eq(10) input').length > 0 ? row.closest('tr').find('td:eq(10) input').val() : row.find("TD").eq(10).html();
                purchaseRequestDetail.remark_by_ho = row.closest('tr').find('td:eq(11) input').length > 0 ? row.closest('tr').find('td:eq(11) input').val() : row.find("TD").eq(11).html();
                purchaserequestDetailModels.push(purchaseRequestDetail);
            });
            console.log(purchaserequestDetailModels);
            $.ajax({
                type: "POST",
                url: $(this).data('url'),
                data: JSON.stringify({ _purchaserequest: _purchaserequest, purchaserequestDetailModels: purchaserequestDetailModels }),
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
            getdata.find('td:eq(2) select').select2({
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
            }).prop('disabled', false);

            getdata.find('input').prop('disabled', false);
        });
    }
};
$(document).ready(function () {
    Klinik.init();
});