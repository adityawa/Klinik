var Klinik = {
    init: function () {
        Klinik.addPuschaseOrderDetailItem();
        Klinik.autocompleteProductOne();
        Klinik.savePurchaseOrder();
        Klinik.editPurchaseOrderDetail();
        Klinik.checkednewproduct();
        Klinik.total();
        Klinik.saverowPurchaseorderDetail();
        Klinik.CreateNewProduct();
        Klinik.openallbutton();
    },

    total: function () {
        var el = $('#total');
        if (!el.length) return;

        var qtyadd = $('#qty_add');
        var qtyrequest = $('#qty');
        var qty_by_ho = $('#qty_by_ho');
        qtyadd.keyup(function () {
            el.val(parseInt(qtyadd.val()) + parseInt(qtyrequest.val()));
            qty_by_ho.val(el.val());
        });
    },
    autocompleteProductOne: function () {
        var el = $("#namabarang");
        if (!el.length) return;

        var buttonaddnew = '<button class="add-product">Add New</button>';

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
            $('#nama_by_ho').val($(el).text());
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
            var newproductname = $("#newproductname");
            var tBody = $("#tblPurchaseRequest > TBODY")[0];
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
            cell.html($('#newproduct').prop("checked") == false ? namabarang.text() : newproductname.val());

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
            cell.html($('#newproduct').prop('checked'));
            cell.hide();

            cell = $(row.insertCell(-1));
            var btnRemove = $("<input />");
            btnRemove.attr("type", "button");
            btnRemove.attr("onclick", "Remove(this);");
            btnRemove.val("Remove");
            cell.append(btnRemove);

            //$('#purchaseorderdetail input[type="text"], textarea').val('');
            $('#qty_add').val('');
            $('#reason_add').val('');
            $('#nama_by_ho').val('');
            $('#namabarang').select2('data', null)
            $('#namabarang').text('').show();
            $('#newproductname').val('').hide();
            $('#namabarang').prop('checked', false);
            $('.addnewproduct').hide();
            Klinik.autocompleteProductOne();

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
            $("#tblPurchaseRequest TBODY TR").each(function () {
                var row = $(this);
                var purchaseRequestDetail = {};
                var newproductid = 0;

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
                    //window.location.reload();
                    window.location.href = '/PurchaseRequest/PurchaseRequestList';
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
            $(this).hide();
            getdata.find('.save-purchaseorderdetail').show();
        });
    },

    checkednewproduct: function () {
        var el = $('#newproduct');

        if (!el.length) return;

        el.click(function () {
            if ($(this).prop("checked") == true) {
                $("#namabarang").hide();
                $(".select2").remove();
                $("#newproductname").show();
                $(".addnewproduct").show();
            } else {
                $("#namabarang").show();
                Klinik.autocompleteProductOne();
                $("#newproductname").hide();
                $(".addnewproduct").hide();
            }
        });
    },

    saverowPurchaseorderDetail: function () {
        var el = $(".save-purchaseorderdetail");
        if (!el.length) return;

        $(el).on('click', function () {
            $(this).hide();
            var row = $(this).closest('tr');
            row.find('.delete-purchaseorderdetail').hide();
            row.find('.image-loading').show();
            var purchaseRequestDetail = {};
            purchaseRequestDetail.PurchaseRequestId = $('#Id').val();
            purchaseRequestDetail.Id = row.find("TD").eq(0).html();
            purchaseRequestDetail.ProductId = row.closest('tr').find('td:eq(2) select').val() > 0 ? row.closest('tr').find('td:eq(2) select').val() : row.find("TD").eq(1).html();
            if (row.find("TD").eq(12).html() == "true") {
                purchaseRequestDetail.namabarang = row.find("TD").eq(2).html();
                alert('asdf')
            }
            purchaseRequestDetail.tot_pemakaian = row.closest('tr').find('td:eq(3) input').length > 0 ? row.closest('tr').find('td:eq(3) input').val() : row.find("TD").eq(3).html();
            purchaseRequestDetail.sisa_stok = row.closest('tr').find('td:eq(4) input').length > 0 ? row.closest('tr').find('td:eq(4) input').val() : row.find("TD").eq(4).html();
            purchaseRequestDetail.qty = row.closest('tr').find('td:eq(5) input').length > 0 ? row.closest('tr').find('td:eq(5) input').val() : row.find("TD").eq(5).html();
            purchaseRequestDetail.qty_add = row.closest('tr').find('td:eq(6) input').length > 0 ? row.closest('tr').find('td:eq(6) input').val() : row.find("TD").eq(6).html();
            purchaseRequestDetail.reason_add = row.closest('tr').find('td:eq(7) input').length > 0 ? row.closest('tr').find('td:eq(7) input').val() : row.find("TD").eq(7).html();
            purchaseRequestDetail.total = row.closest('tr').find('td:eq(8) input').length > 0 ? row.closest('tr').find('td:eq(8) input').val() : row.find("TD").eq(8).html();
            purchaseRequestDetail.nama_by_ho = row.closest('tr').find('td:eq(9) input').length > 0 ? row.closest('tr').find('td:eq(9) input').val() : row.find("TD").eq(9).html();
            purchaseRequestDetail.qty_by_ho = row.closest('tr').find('td:eq(10) input').length > 0 ? row.closest('tr').find('td:eq(10) input').val() : row.find("TD").eq(10).html();
            purchaseRequestDetail.remark_by_ho = row.closest('tr').find('td:eq(11) input').length > 0 ? row.closest('tr').find('td:eq(11) input').val() : row.find("TD").eq(11).html();
            console.log(purchaseRequestDetail);
            $.ajax({
                type: "POST",
                url: $(this).data('url'),
                data: JSON.stringify({ purchaseRequestDetail: purchaseRequestDetail }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    $('.image-loading').hide();
                    row.find('input').prop('disabled', true);
                    row.find('select').prop('disabled', false);
                    row.find('.edit-purchaseorderdetail').show();
                    row.find('.delete-purchaseorderdetail').show();
                }
            });
        });
    },

    CreateNewProduct: function () {

        var el = $('.addnewproduct')
        if (!el.length) return;

        el.click(function () {
            var productRequest = {};
            productRequest.Name = $('#newproductname').val();
            productRequest.ProductCategoryID = 1;
            productRequest.ProductUnitID = 1;
            productRequest.RetailPrice = 100000;
            $.ajax({
                type: "POST",
                url: '/PurchaseRequest/CreateOrEditNewProduct',
                data: JSON.stringify({ productRequest: productRequest }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    console.log(r.data.Id);
                    $('#ProductId').val(r.data.Id);
                    $('#nama_by_ho').val($('#newproductname').val());
                }
            });
        });
    },

    openallbutton: function () {
        var el = $('.openallbutton');
        if (!el.length) return;

        el.click(function () {
            $('.saveorderdetail').show();
            $(this).hide();
            $('.edit-purchaseorderdetail').attr('disabled', false);
            $('#sendby').attr('disabled', false);
        });
    }
};
$(document).ready(function () {
    Klinik.init();
});