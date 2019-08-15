var Klinik = {
    init: function () {
        Klinik.addDeliveryOrderDetailItem();
        Klinik.autocompleteProductOne();
        Klinik.autocompleteGudangOne();
        Klinik.autocompleteKlinikOne();
        Klinik.autocompleteProductTwo();
        Klinik.saveDeliveryOrder();
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
            $("#ProductId").attr("value", el.val());
        });
    },

    autocompleteGudangOne: function () {
        var el = $("#namagudang");
        if (!el.length) return;

        $('#namagudang').select2({
            width: 'resolve',
            placeholder: 'gudang..',
            ajax: {
                url: '/DeliveryOrder/searchgudang/',
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
                            text: item.name
                        });
                    });
                    return {
                        results: results
                    };
                }
            }
        });
        $(el).change(function () {
            $("#GudangId").attr("value", el.val());
        });
    },

    autocompleteKlinikOne: function () {
        var el = $("#namaklinik");
        if (!el.length) return;

        $('#namaklinik').select2({
            width: 'resolve',
            placeholder: 'klinik..',
            ajax: {
                url: '/DeliveryOrder/searchklinik/',
                data: function (params) {
                    return {
                        prefix: params.term
                    };
                },
                dataType: 'json',
                delay: 250,
                processResults: function (data) {
                    var results = [];
                    console.log(data.data);
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
            $("#ClinicId").attr("value", el.val());
        });
    },

    autocompleteProductTwo: function () {
        var el = $("#namabarang_po");
        if (!el.length) return;

        $('#namabarang_po').select2({
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
            $("#ProductId_Po").attr("value", el.val());
        });
    },

    addDeliveryOrderDetailItem: function () {
        var el = $("#btnAdd");
        if (!el.length) return;
        $("body").on("click", "#btnAdd", function () {
            var DeliveryOrderDetailId = $("#DeliveryOrderDetailId");
            var ProductId = $("#ProductId");
            var namabarang = $("#namabarang");
            var GudangId = $("#GudangId");
            var namagudang = $("#namagudang");
            var ClinicId = $("#ClinicId");
            var namaklinik = $("#namaklinik");
            var ProductId_Po = $("#ProductId_Po");
            var namabarang_po = $("#namabarang_po");
            var qty_po = $("#qty_po");
            var qty_po_final = $("#qty_po_final");
            var qty_do = $("#qty_do");
            var qty_adj = $("#qty_adj");
            var remark_do = $("#remark_do");
            var remark_adj = $("#remark_adj");
            alert(ProductId.val());
            var tBody = $("#tblDeliveryOrder > TBODY")[0];
            //Add Row.
            var row = tBody.insertRow(-1);

            //Add id cell.
            var cell = $(row.insertCell(-1));
            cell.html(DeliveryOrderDetailId.val());
            cell.hide();

            cell = $(row.insertCell(-1));
            cell.html(ProductId.val());
            cell.hide();

            cell = $(row.insertCell(-1));
            cell.html(GudangId.val());
            cell.hide();

            cell = $(row.insertCell(-1));
            cell.html(ClinicId.val());
            cell.hide();

            cell = $(row.insertCell(-1));
            cell.html(ProductId_Po.val());
            cell.hide();
            //add nama obat
            cell = $(row.insertCell(-1));
            cell.html(namabarang.text());
            //Add jumlah cell.
            cell = $(row.insertCell(-1));
            cell.html(namagudang.text());
            //Add jumlah cell.
            cell = $(row.insertCell(-1));
            cell.html(namaklinik.text());
            //Add jumlah cell.
            cell = $(row.insertCell(-1));
            cell.html(namabarang_po.text());
            //Add jumlah cell.
            cell = $(row.insertCell(-1));
            cell.html(qty_po.val());
            //Add jumlah cell.
            cell = $(row.insertCell(-1));
            cell.html(qty_po_final.val());
            //Add jumlah cell.
            cell = $(row.insertCell(-1));
            cell.html(qty_do.val());
            //Add jumlah cell.
            cell = $(row.insertCell(-1));
            cell.html(remark_do.val());
            //Add jumlah cell.
            cell = $(row.insertCell(-1));
            cell.html(qty_adj.val());
            //Add jumlah cell.
            cell = $(row.insertCell(-1));
            cell.html(remark_adj.val());
            //Add jumlah cell.
            cell = $(row.insertCell(-1));
            cell.html('Add');

            cell = $(row.insertCell(-1));
            var btnRemove = $("<input />");
            btnRemove.attr("type", "button");
            btnRemove.attr("onclick", "Remove(this);");
            btnRemove.val("Remove");
            cell.append(btnRemove);

            //Clear the TextBoxes.
            $('#deliveryorderdetail input[type="text"], textarea').val('');
            namabarang.select2('data', null);
            namagudang.select2('data', null);
            namaklinik.select2('data', null);
            namabarang_po.select2('data', null);
            namabarang.text('');
            namagudang.text('');
            namaklinik.text('');
            namabarang_po.text('');
            ProductId.attr('value','');
            ProductId_Po.attr('value', '');
            ClinicId.attr('value', '');
            GudangId.attr('value', '');
            Klinik.autocompleteProductOne();
            Klinik.autocompleteGudangOne();
            Klinik.autocompleteKlinikOne();
            Klinik.autocompleteProductTwo();
            Klinik.addDeliveryOrderDetailItem();
        });
    },

    saveDeliveryOrder: function () {
        $('.saveorderdetail').on('click', function () {
            var _deliveryorder = {};
            _deliveryorder.Id = $('#Id').val();
            _deliveryorder.donumber = $('#donumber').val();
            _deliveryorder.dodate = $('#dodate').val();
            _deliveryorder.dodest = $('#dodest').val();
            _deliveryorder.approve_by = $('#approve_by').val();
            var deliveryOrderDetailModels = new Array();
            $("#tblDeliveryOrder TBODY TR").each(function () {
                var row = $(this);
                var deliveryOrderDetail = {};
                deliveryOrderDetail.Id = row.find("TD").eq(0).html();
                deliveryOrderDetail.ProductId = row.find("TD").eq(1).html();
                deliveryOrderDetail.GudangId = row.find("TD").eq(2).html();
                deliveryOrderDetail.ClinicId = row.find("TD").eq(3).html();
                deliveryOrderDetail.ProductId_Po = row.find("TD").eq(4).html();
                deliveryOrderDetail.namabarang = row.find("TD").eq(5).html();
                deliveryOrderDetail.namabarang_po = row.find("TD").eq(8).html();
                deliveryOrderDetail.qty_po = row.find("TD").eq(9).html();
                deliveryOrderDetail.qty_po_final = row.find("TD").eq(10).html();
                deliveryOrderDetail.qty_do = row.find("TD").eq(11).html();
                deliveryOrderDetail.qty_adj = row.find("TD").eq(13).html();
                deliveryOrderDetail.remark_do = row.find("TD").eq(12).html();
                deliveryOrderDetail.remark_adj = row.find("TD").eq(14).html();
                deliveryOrderDetail.type = row.find("TD").eq(15).html();
                deliveryOrderDetailModels.push(deliveryOrderDetail);
            });
            console.log(deliveryOrderDetailModels);
            $.ajax({
                type: "POST",
                url: "/DeliveryOrder/CreateOrEditDeliveryOrder",
                data: JSON.stringify({ _deliveryorder: _deliveryorder, deliveryOrderDetailModels: deliveryOrderDetailModels }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    alert(r);
                    window.location.reload();
                }
            });
        });
    }
};

$(document).ready(function () {
    Klinik.init();
});