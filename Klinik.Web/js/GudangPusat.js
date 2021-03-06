﻿var PurchaseRequest = {
    init: function () {
        PurchaseRequest.Addpurchaserequestitem();
        PurchaseRequest.Total();
        PurchaseRequest.SavePurchaseRequestPusat();
        PurchaseRequest.OpenAllButton();
        PurchaseRequest.SavePurchaseOrderDetail();
        PurchaseRequest.EditPurchaseOrderDetail();
        PurchaseRequest.EditNamaBarang();
    },

    Addpurchaserequestitem: function () {
        var el = $("#btnAdd");
        if (!el.length) return;
        $("body").on("click", "#btnAdd", function () {
            var PurchaseRequestPusatDetailId = $("#PurchaseRequestPusatDetailId");
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
            var qty_final = $("#qty_final");
            var remark = $("#remark");
            var total = $("#total");
            var qty_unit = $("#qty_unit");
            var qty_box = $("#qty_box");
            var tBody = $("#tblPurchaseOrderPusat > TBODY")[0];

            //Add Row.
            var row = tBody.insertRow(-1);
            //Add id cell.
            var cell = $(row.insertCell(-1));
            cell.html(PurchaseRequestPusatDetailId.val());
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
            cell.html(qty_final.val());

            cell = $(row.insertCell(-1));
            cell.html(remark.val());

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

    Total: function () {
        var el = $('#qty_final');
        if (!el.length) return;

        var qtyadd = $('#qty_add');
        var reason_add = $('#reason_add');
        var qty_final = $('#qty_final');
        var remark = $('#remark');
        var total = $('#total');
        qtyadd.keyup(function () {
            qty_final.val(parseInt(qtyadd.val()));
            total.val(parseInt(qtyadd.val()) + parseInt(qty_final.val()));
        });
        reason_add.keyup(function () {
            remark.val(reason_add.val());
        });
    },

    SavePurchaseRequestPusat: function () {
        $('.savepurchasepusardetail').on('click', function () {
            var _purchaserequestpusat = {};
            _purchaserequestpusat.Id = $('#Id').val();
            _purchaserequestpusat.prnumber = $('#prnumber').val();
            _purchaserequestpusat.prdate = $('#prdate').val();
            _purchaserequestpusat.request_by = $('#request_by').val();

            var purchaserequestpusatDetailModels = new Array();
            $("#tblPurchaseOrderPusat TBODY TR").each(function () {
                var row = $(this);
                var purchaserequestPusatDetail = {};
                purchaserequestPusatDetail.Id = row.find("TD").eq(0).html();
                purchaserequestPusatDetail.ProductId = row.closest('tr').find('td:eq(3) select').val() > 0 ? row.closest('tr').find('td:eq(3) select').val() : row.find("TD").eq(1).html();
                purchaserequestPusatDetail.VendorId = row.closest('tr').find('td:eq(4) select').val() > 0 ? row.closest('tr').find('td:eq(4) select').val() : row.find("TD").eq(2).html();
                //purchaseOrderPusatDetail.namabarang = row.closest('tr').find('td:eq(5) input').length > 0 ? row.closest('tr').find('td:eq(4) input').val() : row.find("TD").eq(5).html();
                //purchaseOrderPusatDetail.namavendor = row.closest('tr').find('td:eq(6) input').length > 0 ? row.closest('tr').find('td:eq(6) input').val() : row.find("TD").eq(6).html();
                purchaserequestPusatDetail.satuan = row.closest('tr').find('td:eq(5) input').length > 0 ? row.closest('tr').find('td:eq(5) input').val() : row.find("TD").eq(5).html();
                purchaserequestPusatDetail.harga = row.closest('tr').find('td:eq(6) input').length > 0 ? row.closest('tr').find('td:eq(6) input').val() : row.find("TD").eq(6).html();
                purchaserequestPusatDetail.stok_prev = row.closest('tr').find('td:eq(7) input').length > 0 ? row.closest('tr').find('td:eq(7) input').val() : row.find("TD").eq(7).html();
                purchaserequestPusatDetail.total_req = row.closest('tr').find('td:eq(8) input').length > 0 ? row.closest('tr').find('td:eq(8) input').val() : row.find("TD").eq(8).html();
                purchaserequestPusatDetail.total_dist = row.closest('tr').find('td:eq(9) input').length > 0 ? row.closest('tr').find('td:eq(9) input').val() : row.find("TD").eq(9).html();
                purchaserequestPusatDetail.sisa_stok = row.closest('tr').find('td:eq(10) input').length > 0 ? row.closest('tr').find('td:eq(10) input').val() : row.find("TD").eq(10).html();
                purchaserequestPusatDetail.qty = row.closest('tr').find('td:eq(11) input').length > 0 ? row.closest('tr').find('td:eq(11) input').val() : row.find("TD").eq(11).html();
                purchaserequestPusatDetail.qty_add = row.closest('tr').find('td:eq(12) input').length > 0 ? row.closest('tr').find('td:eq(12) input').val() : row.find("TD").eq(12).html();
                purchaserequestPusatDetail.reason_add = row.closest('tr').find('td:eq(13) input').length > 0 ? row.closest('tr').find('td:eq(13) input').val() : row.find("TD").eq(13).html();
                purchaserequestPusatDetail.qty_final = row.closest('tr').find('td:eq(14) input').length > 0 ? row.closest('tr').find('td:eq(14) input').val() : row.find("TD").eq(14).html();
                purchaserequestPusatDetail.remark = row.closest('tr').find('td:eq(15) input').length > 0 ? row.closest('tr').find('td:eq(15) input').val() : row.find("TD").eq(15).html();
                purchaserequestPusatDetail.total = row.closest('tr').find('td:eq(16) input').length > 0 ? row.closest('tr').find('td:eq(16) input').val() : row.find("TD").eq(16).html();
                purchaserequestPusatDetail.qty_unit = row.closest('tr').find('td:eq(17) input').length > 0 ? row.closest('tr').find('td:eq(17) input').val() : row.find("TD").eq(17).html();
                purchaserequestPusatDetail.qty_box = row.closest('tr').find('td:eq(18) input').length > 0 ? row.closest('tr').find('td:eq(18) input').val() : row.find("TD").eq(18).html();
                purchaserequestpusatDetailModels.push(purchaserequestPusatDetail);
            });
            console.log(purchaserequestpusatDetailModels);
            $.ajax({
                type: "POST",
                url: $(this).data('url'),
                data: JSON.stringify({ _purchaserequestpusat: _purchaserequestpusat, purchaserequestpusatDetailModels: purchaserequestpusatDetailModels }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    window.location.reload();
                }
            });

        });
    },

    OpenAllButton: function () {
        var el = $('.openallbutton');
        if (!el.length) return;

        el.click(function () {
            $('.savepurchasepusardetail').show();
            $(this).hide();
            $('.edit-purchaserequestpusatdetail').attr('disabled', false);
            $('#sendby').attr('disabled', false);
        });
    },

    SavePurchaseOrderDetail: function () {
        var el = $('.save-purchaserequestpusatdetail');
        if (!el.length) return;

        $(el).on('click', function () {
            $(this).hide();
            var row = $(this).closest('tr');
            row.find('.delete-purchaserequestpusatdetail').hide();
            row.find('.image-loading').show();
            var purchaserequestPusatDetail = {};
            purchaserequestPusatDetail.PurchaseRequestPusatId = $('#Id').val();
            purchaserequestPusatDetail.Id = row.find("TD").eq(0).html();
            purchaserequestPusatDetail.ProductId = row.closest('tr').find('td:eq(3) select').val() > 0 ? row.closest('tr').find('td:eq(3) select').val() : row.find("TD").eq(1).html();
            purchaserequestPusatDetail.VendorId = row.closest('tr').find('td:eq(4) select').val() > 0 ? row.closest('tr').find('td:eq(4) select').val() : row.find("TD").eq(2).html();
            purchaserequestPusatDetail.satuan = row.closest('tr').find('td:eq(5) input').length > 0 ? row.closest('tr').find('td:eq(5) input').val() : row.find("TD").eq(5).html();
            purchaserequestPusatDetail.harga = row.closest('tr').find('td:eq(6) input').length > 0 ? row.closest('tr').find('td:eq(6) input').val() : row.find("TD").eq(6).html();
            purchaserequestPusatDetail.stok_prev = row.closest('tr').find('td:eq(7) input').length > 0 ? row.closest('tr').find('td:eq(7) input').val() : row.find("TD").eq(7).html();
            purchaserequestPusatDetail.total_req = row.closest('tr').find('td:eq(8) input').length > 0 ? row.closest('tr').find('td:eq(8) input').val() : row.find("TD").eq(8).html();
            purchaserequestPusatDetail.total_dist = row.closest('tr').find('td:eq(9) input').length > 0 ? row.closest('tr').find('td:eq(9) input').val() : row.find("TD").eq(9).html();
            purchaserequestPusatDetail.sisa_stok = row.closest('tr').find('td:eq(10) input').length > 0 ? row.closest('tr').find('td:eq(10) input').val() : row.find("TD").eq(10).html();
            purchaserequestPusatDetail.qty = row.closest('tr').find('td:eq(11) input').length > 0 ? row.closest('tr').find('td:eq(11) input').val() : row.find("TD").eq(11).html();
            purchaserequestPusatDetail.qty_add = row.closest('tr').find('td:eq(12) input').length > 0 ? row.closest('tr').find('td:eq(12) input').val() : row.find("TD").eq(12).html();
            purchaserequestPusatDetail.reason_add = row.closest('tr').find('td:eq(13) input').length > 0 ? row.closest('tr').find('td:eq(13) input').val() : row.find("TD").eq(13).html();
            purchaserequestPusatDetail.qty_final = row.closest('tr').find('td:eq(14) input').length > 0 ? row.closest('tr').find('td:eq(14) input').val() : row.find("TD").eq(14).html();
            purchaserequestPusatDetail.remark = row.closest('tr').find('td:eq(15) input').length > 0 ? row.closest('tr').find('td:eq(15) input').val() : row.find("TD").eq(15).html();
            purchaserequestPusatDetail.total = row.closest('tr').find('td:eq(16) input').length > 0 ? row.closest('tr').find('td:eq(16) input').val() : row.find("TD").eq(16).html();
            purchaserequestPusatDetail.qty_unit = row.closest('tr').find('td:eq(17) input').length > 0 ? row.closest('tr').find('td:eq(17) input').val() : row.find("TD").eq(17).html();
            purchaserequestPusatDetail.qty_box = row.closest('tr').find('td:eq(18) input').length > 0 ? row.closest('tr').find('td:eq(18) input').val() : row.find("TD").eq(18).html();

            $.ajax({
                type: "POST",
                url: $(this).data('url'),
                data: JSON.stringify({ purchaseRequestPusatDetail: purchaserequestPusatDetail }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    window.location.reload();
                }
            });
        });
    },

    EditPurchaseOrderDetail: function () {
        var el = $('.edit-purchaserequestpusatdetail');
        if (!el.length) return;

        $(el).on('click', function () {
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
                    url: '/GudangPusat/searchvendor/',
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

            getdata.find('td:eq(5) input').prop('disabled', false);
            getdata.find('td:eq(6) input').prop('disabled', false);
            getdata.find('td:eq(12) input').prop('disabled', false);
            getdata.find('td:eq(13) input').prop('disabled', false);
            getdata.find('td:eq(17) input').prop('disabled', false);
            getdata.find('td:eq(18) input').prop('disabled', false);
            $(this).hide();
            getdata.find('.save-purchaserequestpusatdetail').show();
            PurchaseRequest.EditNamaBarang();
        });
    },

    EditNamaBarang: function () {
        var el = $('.editnamabarang');
        if (!el.length) return;

        $(el).change(function () {
            var row = $(this).closest('tr');

            $.ajax({
                type: "GET",
                url: "/GudangPusat/GetStokdatabyProductId?productid=" + $(this).val(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    console.log(r.data);
                    var data = r.data;
                    row.find('td:eq(7) input').val(data.stock);
                    row.find('td:eq(8) input').val(data.datapo);
                    row.find('td:eq(9) input').val(data.datado);
                    row.find('td:eq(10) input').val(data.sisastock);
                }
            })

        });
    }
}

var General = {
    init: function () {
        General.Searchproduct();
        General.Searchvendor();
    },

    Searchproduct: function () {
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
            $.ajax({
                type: "GET",
                url: "/GudangPusat/GetStokdatabyProductId?productid=" + $(el).val(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    console.log(r.data);
                    var data = r.data;
                    $('#stok_prev').val(data.stock);
                    $('#total_req').val(data.datapo);
                    $('#total_dist').val(data.datado);
                    $('#sisa_stok').val(data.sisastock);
                }
            })
        });
    },

    Searchvendor: function () {
        var el = $("#namavendor");
        if (!el.length) return;

        $('#namavendor').select2({
            width: 'resolve',
            placeholder: 'vendor..',
            ajax: {
                url: '/GudangPusat/searchvendor/',
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
    }
}

$(document).ready(function () {
    PurchaseRequest.init();
    General.init();
});