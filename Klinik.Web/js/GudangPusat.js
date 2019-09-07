var PurchaseRequest = {
    init: function () {
        PurchaseRequest.Addpurchaserequestitem();
        PurchaseRequest.Total();
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