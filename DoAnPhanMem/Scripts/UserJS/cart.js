function reGex() {
    return regex = /(\d)(?=(\d{3})+(?!\d))/g; // Định dạng giá sang dấu chấm
}

// Nhập số lượng vào ô nhập và thay đổi
function Update_quan_mouse_ev() {
    var id = $('.qty-input').data("id");
    $(".btn-inc").removeClass('no-pointer-events');
    var quan = $('.qty-input').val();
    if (quan != "") {
        updateCartOnServer(id, quan); // Gọi hàm API để cập nhật giỏ hàng trên máy chủ
    }
}

$(".qty-input").mouseleave(function () {
    Update_quan_mouse_ev()
});
$(".qty-input").mouseover(function () {
    Update_quan_mouse_ev()
});
$(".qty-input").change(function (ev) {
    Update_quan_mouse_ev()
})
$(".qty-input").mouseout(function (ev) {
    Update_quan_mouse_ev()
})

$(".btn-dec").click(function (ev) {
    $(".btn-inc").removeClass('no-pointer-events');
    var quanInput = $(ev.currentTarget).next();
    var id = quanInput.data("id");
    var price = quanInput.data("price");
    var quan = Number(quanInput.val());
    if (quan > 1) {
        quan = quan - 1;
        quanInput.val(quan);
        updateCartOnServer(id, quan); // Gọi hàm API để cập nhật giỏ hàng trên máy chủ
        updateOrderPrice();
        var newTotal4 = 0;
        newTotal4 = quan * price;
        newTotal4 += "";
        $('#total3-' + id).text(newTotal4.replace(regex, "$1.") + "₫");
    }
});

$(".btn-inc").click(function (ev) {
    var quanInput = $(ev.currentTarget).prev();
    var maxquan = quanInput.data("quan");
    var id = quanInput.data("id");
    var price = quanInput.data("price");
    var quan = Number(quanInput.val());

    if (quan < maxquan) {
        quan = quan + 1;
        quanInput.val(quan); // Đảm bảo cập nhật giá trị trên view
        updateCartOnServer(id, quan); // Gọi hàm API để cập nhật giỏ hàng trên máy chủ
        updateOrderPrice(); // Cập nhật tổng giá trị

        var newTotal4 = 0;
        newTotal4 = quan * price;
        newTotal4 += "";
        $('#total3-' + id).text(newTotal4.replace(regex, "$1.") + "₫");
    }
});

$(".js-delete").click(function (ev) {
    Swal.fire({
        title: 'Xác nhận xóa?',
        text: "Xóa sản phẩm khỏi giỏ hàng",
        icon: 'warning',
        showCancelButton: true,
        cancelButtonColor: '#d33',
        confirmButtonColor: '#3085d6',
        cancelButtonText: 'Hủy',
        confirmButtonText: 'Xác nhận!',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            var id = $(ev.currentTarget).data("id");
            var item = $(ev.currentTarget).closest(".item");

            // Gọi API để xóa sản phẩm khỏi giỏ hàng
            $.post("/Cart/RemoveFromCart", { productId: id }, function (data) {
                if (data.success) {
                    item.remove();
                    const Toast = Swal.mixin({
                        toast: true,
                        position: 'top-end',
                        showConfirmButton: false,
                        timer: 1500,
                        didOpen: (toast) => {
                            toast.addEventListener('mouseenter', Swal.stopTimer)
                            toast.addEventListener('mouseleave', Swal.resumeTimer)
                        }
                    })
                    Toast.fire({
                        icon: 'success',
                        title: 'Xóa thành công'
                    })
                }
            });
        }
    })
});


function updateOrderPrice() {
    $(".lblCartCount").text(Cookie.countProduct());
    var quanInputs = $("input.qty-input");
    var newTotal = 0;
    var totalWithFee;
    quanInputs.each(function (i, e) {
        var price = Number($(e).data('price'));
        var quan = Number($(e).val());
        newTotal += price * quan;
    });
    var eleDiscount = $("#discount");
    var discount = 0;
    if (eleDiscount.attr("data-price") == null) {
        totalWithFee = newTotal + 0;
    } else {
        if (eleDiscount.attr("data-price") < newTotal) {
            discount = Number(eleDiscount.attr("data-price"));
            totalWithFee = newTotal + 0 - discount;
        } else {
            discount = Number(eleDiscount.attr("data-price"));
            totalWithFee = 0;
        }
    }
    totalWithFee += "";
    newTotal += "";
    discount += "";
    reGex();
    $(".totalPrice").text(newTotal.replace(regex, "$1.") + "₫");
    $(".totalPriceWithFee").text(totalWithFee.replace(regex, "$1.") + "₫");
    $("#discount").text(discount.replace(regex, "$1.") + "₫");
};

//$(".js-checkout").click(function (ev) {
//    ev.preventDefault();
//    var count_product = Cookie.countWithPrefix("product");
//    $.get("/Account/UserLogged", {},
//        function (isLogged, textStatus, jqXHR) {
//            if (!isLogged) {
//                Swal.fire({
//                    title: 'Yêu cầu đăng nhập?',
//                    text: "Vui lòng đăng nhập để thực hiện được chức năng này",
//                    icon: 'error',
//                    showCancelButton: true,
//                    cancelButtonColor: '#d33',
//                    confirmButtonColor: '#3085d6',
//                    cancelButtonText: 'Hủy',
//                    confirmButtonText: 'Đăng nhập',
//                    reverseButtons: true
//                }).then((result) => {
//                    if (result.isConfirmed) {
//                        location.href = 'login';
//                    }
//                })
//                return;
//            } else if (count_product == 0) {
//                const Toast = Swal.mixin({
//                    toast: true,
//                    position: 'top',
//                    showConfirmButton: false,
//                    timer: 1500,
//                    didOpen: (toast) => {
//                        toast.addEventListener('mouseenter', Swal.stopTimer)
//                        toast.addEventListener('mouseleave', Swal.resumeTimer)
//                    }
//                })
//                Toast.fire({
//                    icon: 'error',
//                    title: 'Giỏ hàng chưa có sản phẩm!'
//                })
//            } else {
//                location.href = ev.currentTarget.href;
//            }
//        },
//        "json"
//    );
//});

// Hàm gọi API để cập nhật giỏ hàng trên máy chủ
function updateCartOnServer(productId, quantity) {
    $.ajax({
        url: '/Cart/UpdateCart', // Đường dẫn API để cập nhật giỏ hàng trên máy chủ
        type: 'POST',
        data: { productId: productId, quantity: quantity },
        success: function (data) {
            // Xử lý kết quả trả về nếu cần
            updateOrderPrice(); // Cập nhật giá trị trên view
        },
        error: function (error) {
            // Xử lý lỗi nếu có
        }
    });
}
$(".btn-submitcoupon").click(function (ev) {
    var input = $(ev.currentTarget).prev();
    var _code = input.val().trim();
    if (_code.length) {
        $.getJSON("/cart/UseDiscountCode", { code: _code },
            function (data, textStatus, jqXHR) {
                if (data.success) {
                    $("#discount").attr("data-price", data.discountPrice);
                    $("#discount").attr("class", 'text-success');
                    updateOrderPrice();

                    // Lưu mã giảm giá vào cookie
                    Cookie.set("discountCode", _code, 7);

                    const Toast = Swal.mixin({
                        toast: true,
                        position: 'top-end',
                        showConfirmButton: false,
                        timer: 2500,
                        didOpen: (toast) => {
                            toast.addEventListener('mouseenter', Swal.stopTimer)
                            toast.addEventListener('mouseleave', Swal.resumeTimer)
                        }
                    });
                    Toast.fire({
                        icon: 'success',
                        title: 'Mã giảm giá đã được áp dụng thành công!'
                    });
                } else {
                    const Toast = Swal.mixin({
                        toast: true,
                        position: 'top-end',
                        showConfirmButton: false,
                        timer: 2500,
                        didOpen: (toast) => {
                            toast.addEventListener('mouseenter', Swal.stopTimer)
                            toast.addEventListener('mouseleave', Swal.resumeTimer)
                        }
                    });
                    Toast.fire({
                        icon: 'error',
                        title: 'Mã giảm giá không khả dụng!!'
                    });
                }
            }
        );
    }
});
