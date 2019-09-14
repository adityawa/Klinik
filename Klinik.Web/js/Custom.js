var Klinik = {
    init: function () {
        Klinik.addDeliveryOrderDetailItem();
        Klinik.autocompleteProductOne();
        Klinik.autocompleteGudangOne();
        Klinik.autocompleteKlinikOne();
        Klinik.autocompleteProductTwo();
        Klinik.saveDeliveryOrder();
        Klinik.editDeliveryOrderDetail();
        Klinik.searchpurchaceorder();
        Klinik.searchpurchaceorderpusat();
        Klinik.saveOrderdetailPerRow();
        Klinik.checkall();
        Klinik.checkbox();
        Klinik.openallbutton();
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
            $("#GudangId").val($(el).val());
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
            $("#ClinicId").val($(el).val());
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
            $("#ProductId_Po").val($(el).val());
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
            cell.hide();

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
            //Klinik.addDeliveryOrderDetailItem();
        });
    },

    editDeliveryOrderDetail: function () {
        $('.edit-deliveryorderdetail').on('click', function () {
            var getdata = $(this).closest('tr');
            getdata.find('td:eq(5) select').select2({
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

            getdata.find('td:eq(7) input').prop('disabled', false);
            getdata.find('td:eq(8) input').prop('disabled', false);
            $(this).hide();
            getdata.find('.save-deliveryorderdetail').show();
            getdata.find('input[type="checkbox"]').prop('disabled', false);
            Klinik.saveOrderdetailPerRow();
        });
    },

    saveDeliveryOrder: function () {
        $('.saveorderdetail').on('click', function () {
            var _deliveryorder = {};
            _deliveryorder.Id = $('#Id').val();
            _deliveryorder.poid = $('#ponumber').length > 0 ? $('#ponumber').val() : $('#popusatnumber').val();
            _deliveryorder.donumber = $('#donumber').val();
            _deliveryorder.dodate = $('#dodate').val();
            _deliveryorder.dodest = $('#dodest').val();
            _deliveryorder.approve_by = $('#approve_by').val();
            _deliveryorder.sendby = $('#sendby').val();
            var deliveryOrderDetailModels = new Array();
            $("#tblDeliveryOrder TBODY TR").each(function () {
                var row = $(this);
                var deliveryOrderDetail = {};
                deliveryOrderDetail.Id = row.find("TD").eq(0).html();
                deliveryOrderDetail.DeliveryOderId = $('#Id').val();
                deliveryOrderDetail.ProductId = row.find("TD").eq(1).html();
                deliveryOrderDetail.ProductId = row.find("TD").eq(1).html();
                deliveryOrderDetail.qty_adj = row.find('td:eq(7) input').val();
                deliveryOrderDetail.remark_adj = row.find('td:eq(8) input').val();
                deliveryOrderDetail.Recived = row.find('td:eq(9) input[type="checkbox"]').val();
                deliveryOrderDetailModels.push(deliveryOrderDetail);
            });
            console.log(deliveryOrderDetailModels);
            $.ajax({
                type: "POST",
                url: $(this).data('url'),
                data: JSON.stringify({ _deliveryorder: _deliveryorder, deliveryOrderDetailModels: deliveryOrderDetailModels }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    window.location.reload();
                }
            });
        });
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
                        if (item.approve >= 1) {
                            results.push({
                                id: item.Id,
                                text: item.ponumber
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

    searchpurchaceorderpusat: function () {
        var el = $("#popusatnumber");
        if (!el.length) return;

        $('#popusatnumber').select2({
            width: 'resolve',
            placeholder: 'popusat..',
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
                        if (item.approve > 1) {
                            results.push({
                                id: item.Id,
                                text: item.ponumber
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

    saveOrderdetailPerRow: function () {
        var el = $(".save-deliveryorderdetail");
        if (!el.length) return;

        $(el).on('click', function () {
            $(this).hide();
            var row = $(this).closest('tr');
            row.find('.delete-deliveryorderdetail').hide();
            row.find('.image-loading').show();
            var deliveryorderdetail = {};
            deliveryorderdetail.Id = row.find("TD").eq(0).html();
            deliveryorderdetail.ProductId = row.find("TD").eq(1).html();
            deliveryorderdetail.ProductId = row.find("TD").eq(1).html();
            deliveryorderdetail.qty_adj = row.find('td:eq(7) input').val();
            deliveryorderdetail.remark_adj = row.find('td:eq(8) input').val();
            deliveryorderdetail.Recived = row.find('td:eq(9) input[type="checkbox"]').val();

            $.ajax({
                type: "POST",
                url: $(this).data('url'),
                data: JSON.stringify({ deliveryorderdetail: deliveryorderdetail }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    $('.image-loading').hide();
                    row.find('.edit-deliveryorderdetail').show();
                    row.find('.delete-deliveryorderdetail').show();
                    row.find('td:eq(7) input').prop('disabled', true);
                    row.find('td:eq(8) input').prop('disabled', true);
                    row.find('input[type="checkbox"]').prop('disabled', true);
                }
            });
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

    openallbutton: function () {
        var el = $('.openallbutton');
        if (!el.length) return;

        el.click(function () {
            $('.saveorderdetail').show();
            $(this).hide();
            $('.edit-deliveryorderdetail').attr('disabled', false);
            $('#sendby').attr('disabled', false);
        });
    }
};

$(document).ready(function () {
    Klinik.init();
});