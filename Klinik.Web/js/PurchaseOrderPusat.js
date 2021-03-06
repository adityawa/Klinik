﻿var Klinik = {
    init: function () {
        Klinik.addPuschaseOrderPusatDetailItem();
        Klinik.autocompleteProductOne();
        Klinik.autocompleteVendor();
        Klinik.savePurchaseOrderPusat();
        Klinik.editPurchaseOrderPusatDetail();
        Klinik.searchpurchacerequest();
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

    autocompleteVendor: function () {
        var el = $("#namavendor");
        if (!el.length) return;

        $('#namavendor').select2({
            width: 'resolve',
            placeholder: 'vendor..',
            ajax: {
                url: '/PurchaseOrderPusat/searchvendor/',
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
                            text: item.namavendor
                        });
                    });
                    return {
                        results: results
                    };
                }
            }
        });
        $(el).change(function () {
            $("#VendorId").val($(el).val());
        });
    },

    addPuschaseOrderPusatDetailItem: function () {
        var el = $("#btnAdd");
        if (!el.length) return;
        $("body").on("click", "#btnAdd", function () {
            var PurchaseOrderPusatDetailId = $("#PurchaseOrderPusatDetailId");
            var namabarang = $("#namabarang");
            var ProductId = $("#ProductId");
            var VendorId = $("#VendorId");
            var namavendor = $("#namavendor");
            var satuan = $("#satuan");
            var harga = $("#harga");
            var stok_prev = $("#stok_prev");
            var total_req = $("#total_req");
            var total_dist = $("#total_dist");
            var sisa_stok = $("#sisa_stok");
            var qty = $("#qty");
            var qty_add = $("#qty_add");
            var reason_add = $("#reason_add");
            var total = $("#total");
            var qty_unit = $("#qty_unit");
            var qty_box = $("#qty_box");
            var tBody = $("#tblPurchaseOrderPusat > TBODY")[0];

            //Add Row.
            var row = tBody.insertRow(-1);
            //Add id cell.
            var cell = $(row.insertCell(-1));
            cell.html(PurchaseOrderPusatDetailId.val());
            cell.hide();

            cell = $(row.insertCell(-1));
            cell.html(ProductId.val());
            cell.hide();

            cell = $(row.insertCell(-1));
            cell.html(VendorId.val());
            cell.hide();

            cell = $(row.insertCell(-1));
            cell.html(namabarang.text());

            cell = $(row.insertCell(-1));
            cell.html(namavendor.text());

            cell = $(row.insertCell(-1));
            cell.html(satuan.val());

            cell = $(row.insertCell(-1));
            cell.html(harga.val());

            cell = $(row.insertCell(-1));
            cell.html(stok_prev.val());

            cell = $(row.insertCell(-1));
            cell.html(total_req.val());

            cell = $(row.insertCell(-1));
            cell.html(total_dist.val());

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
            cell.html(qty_unit.val());

            cell = $(row.insertCell(-1));
            cell.html(qty_box.val());

            cell = $(row.insertCell(-1));
            var btnRemove = $("<input />");
            btnRemove.attr("type", "button");
            btnRemove.attr("onclick", "Remove(this);");
            btnRemove.val("Remove");
            cell.append(btnRemove);

            $('#purchaseorderpusatdetail input[type="text"], textarea').val('');
            namabarang.text('');
            namavendor.text('');

        });
    },

    savePurchaseOrderPusat: function () {
        $('.savepurchasepusardetail').on('click', function () {
            var _purchaseorderpusat = {};
            _purchaseorderpusat.Id = $('#Id').val();
            _purchaseorderpusat.PurchaseRequestId = $('#PurchaseRequestId').val();
            _purchaseorderpusat.ponumber = $('#ponumber').val();
            _purchaseorderpusat.podate = $('#podate').val();
            _purchaseorderpusat.request_by = $('#request_by').val();

            var purchaseOrderPusatDetailModels = new Array();
            $("#tblPurchaseOrderPusat TBODY TR").each(function () {
                var row = $(this);
                var purchaseOrderPusatDetail = {};
                purchaseOrderPusatDetail.Id = row.find("TD").eq(0).html();
                purchaseOrderPusatDetail.ProductId = row.closest('tr').find('td:eq(3) select').val() > 0 ? row.closest('tr').find('td:eq(3) select').val() : row.find("TD").eq(1).html();
                purchaseOrderPusatDetail.VendorId = row.closest('tr').find('td:eq(4) select').val() > 0 ? row.closest('tr').find('td:eq(4) select').val() : row.find("TD").eq(2).html();
                //purchaseOrderPusatDetail.namabarang = row.closest('tr').find('td:eq(5) input').length > 0 ? row.closest('tr').find('td:eq(4) input').val() : row.find("TD").eq(5).html();
                //purchaseOrderPusatDetail.namavendor = row.closest('tr').find('td:eq(6) input').length > 0 ? row.closest('tr').find('td:eq(6) input').val() : row.find("TD").eq(6).html();
                purchaseOrderPusatDetail.satuan = row.closest('tr').find('td:eq(5) input').length > 0 ? row.closest('tr').find('td:eq(5) input').val() : row.find("TD").eq(5).html();
                purchaseOrderPusatDetail.harga = row.closest('tr').find('td:eq(6) input').length > 0 ? row.closest('tr').find('td:eq(6) input').val() : row.find("TD").eq(6).html();
                purchaseOrderPusatDetail.stok_prev = row.closest('tr').find('td:eq(7) input').length > 0 ? row.closest('tr').find('td:eq(7) input').val() : row.find("TD").eq(7).html();
                purchaseOrderPusatDetail.total_req = row.closest('tr').find('td:eq(8) input').length > 0 ? row.closest('tr').find('td:eq(8) input').val() : row.find("TD").eq(8).html();
                purchaseOrderPusatDetail.total_dist = row.closest('tr').find('td:eq(9) input').length > 0 ? row.closest('tr').find('td:eq(9) input').val() : row.find("TD").eq(9).html();
                purchaseOrderPusatDetail.sisa_stok = row.closest('tr').find('td:eq(10) input').length > 0 ? row.closest('tr').find('td:eq(10) input').val() : row.find("TD").eq(10).html();
                purchaseOrderPusatDetail.qty = row.closest('tr').find('td:eq(11) input').length > 0 ? row.closest('tr').find('td:eq(11) input').val() : row.find("TD").eq(11).html();
                purchaseOrderPusatDetail.qty_add = row.closest('tr').find('td:eq(12) input').length > 0 ? row.closest('tr').find('td:eq(12) input').val() : row.find("TD").eq(12).html();
                purchaseOrderPusatDetail.reason_add = row.closest('tr').find('td:eq(13) input').length > 0 ? row.closest('tr').find('td:eq(13) input').val() : row.find("TD").eq(13).html();
                purchaseOrderPusatDetail.total = row.closest('tr').find('td:eq(14) input').length > 0 ? row.closest('tr').find('td:eq(14) input').val() : row.find("TD").eq(14).html();
                purchaseOrderPusatDetail.qty_unit = row.closest('tr').find('td:eq(15) input').length > 0 ? row.closest('tr').find('td:eq(15) input').val() : row.find("TD").eq(15).html();
                purchaseOrderPusatDetail.qty_box = row.closest('tr').find('td:eq(16) input').length > 0 ? row.closest('tr').find('td:eq(16) input').val() : row.find("TD").eq(16).html();
                purchaseOrderPusatDetailModels.push(purchaseOrderPusatDetail);
            });
            console.log(purchaseOrderPusatDetailModels);
            $.ajax({
                type: "POST",
                url: $(this).data('url'),
                data: JSON.stringify({ _purchaseorderpusat: _purchaseorderpusat, purchaseOrderPusatDetailModels: purchaseOrderPusatDetailModels }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    window.location.reload();
                }
            });

        });
    },

    editPurchaseOrderPusatDetail: function () {
        $('.edit-purchaseorderpusatdetail').on('click', function () {
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
            }).prop('disabled', false);

            getdata.find('td:eq(4) select').select2({
                width: 'resolve',
                placeholder: 'vendor..',
                ajax: {
                    url: '/PurchaseOrderPusat/searchvendor/',
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
                                text: item.namavendor
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
    },

    searchpurchacerequest: function () {
        var el = $("#PurchaseRequestId");
        if (!el.length) return;

        $('#PurchaseRequestId').select2({
            width: 'resolve',
            placeholder: 'purchacerequest..',
            ajax: {
                url: '/General/searchpurchaserequestpusat/',
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
    }
};
$(document).ready(function () {
    Klinik.init();
});