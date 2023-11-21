﻿//1. Chỉnh sửa thông tin cá nhân
//$('#save-form').click(function () {
//    var _userName = $("#Name").val();
//    var _phoneNumber = $("#Phone").val();
//    if (_userName == "") {
//        const Toast = Swal.mixin({
//            toast: true,
//            position: 'top-end',
//            showConfirmButton: false,
//            timer: 1500,
//            didOpen: (toast) => {
//                toast.addEventListener('mouseenter', Swal.stopTimer)
//                toast.addEventListener('mouseleave', Swal.resumeTimer)
//            }
//        })
//        Toast.fire({
//            icon: 'error',
//            title: 'Nhập họ tên'
//        })
//        return false;
//    }
//    if (_phoneNumber == "") {
//        const Toast = Swal.mixin({
//            toast: true,
//            position: 'top-end',
//            showConfirmButton: false,
//            timer: 1500,
//            didOpen: (toast) => {
//                toast.addEventListener('mouseenter', Swal.stopTimer)
//                toast.addEventListener('mouseleave', Swal.resumeTimer)
//            }
//        })
//        Toast.fire({
//            icon: 'error',
//            title: 'Nhập số điện thoại'
//        })
//        return false;
//    }
    //$.ajax({
    //    type: "Post",
    //    url: "/Account/UpdateProfile",
    //    contentType: "application/json; charset=utf-8",
    //    data: JSON.stringify({ userName: _userName, phoneNumber: _phoneNumber}),
    //    dataType: "json",
    //    success: function (result) {
    //        if (result == true) {
    //            const Toast = Swal.mixin({
    //                toast: true,
    //                position: 'top-end',
    //                showConfirmButton: false,
    //                timer: 2500,
    //                didOpen: (toast) => {
    //                    toast.addEventListener('mouseenter', Swal.stopTimer)
    //                    toast.addEventListener('mouseleave', Swal.resumeTimer)
    //                }
    //            })
    //            Toast.fire({
    //                icon: 'success',
    //                title: 'Cập nhật thông tin thành công'
    //            })
    //        }
    //        else {
    //            const Toast = Swal.mixin({
    //                toast: true,
    //                position: 'top-end',
    //                showConfirmButton: false,
    //                timer: 1500,
    //                didOpen: (toast) => {
    //                    toast.addEventListener('mouseenter', Swal.stopTimer)
    //                    toast.addEventListener('mouseleave', Swal.resumeTimer)
    //                }
    //            })
    //            Toast.fire({
    //                icon: 'error',
    //                title: 'Lỗi!'
    //            })
    //        }
    //    }
    //});
//});
//2. Update avatar
function uploadFile(event) {
    var input = document.getElementById("file-ip-1");
    file = input.files[0];
    if (file != undefined) {
        formData = new FormData();
        if (!!file.type.match(/image.*/)) {
            formData.append("image", file);
            $.ajax({
                url: "Account/UpdateAvatar",
                type: "POST",
                data: formData,
                processData: false,
                contentType: false,
                success: function (result) {
                    var src = URL.createObjectURL(event.target.files[0]);
                    var preview = document.getElementById("file-ip-1-preview");
                    preview.src = src;
                    const Toast = Swal.mixin({
                        toast: true,
                        position: 'top-end',
                        showConfirmButton: false,
                        timer: 2000,
                        didOpen: (toast) => {
                            toast.addEventListener('mouseenter', Swal.stopTimer)
                            toast.addEventListener('mouseleave', Swal.resumeTimer)
                        }
                    })
                    Toast.fire({
                        icon: 'success',
                        title: 'Cập nhật Avatar thành công'
                    })
                }
            });
        } else {
            alert('Not a valid image!');
        }
    } else {
        alert('Input something!');
    }
};

$(".js-cancel").click(function (ev) {
    Swal.fire({
        title: 'Xác nhận hủy đơn hàng?',
        text: 'Bạn có chắc chắn muốn hủy đơn hàng này?',
        icon: 'warning',
        showCancelButton: true,
        cancelButtonColor: '#d33',
        confirmButtonColor: '#3085d6',
        cancelButtonText: 'Hủy',
        confirmButtonText: 'Xác nhận!',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            var orderId = $(ev.currentTarget).data("id");

            // Gọi hàm AJAX để gửi yêu cầu hủy đơn hàng
            $.ajax({
                url: "/Account/CancelOrder",
                type: "POST",
                data: { Oderid: orderId },
                success: function (data) {
                    if (data) {
                        const Toast = Swal.mixin({
                            toast: true,
                            position: 'top-end',
                            showConfirmButton: false,
                            timer: 1500,
                            didOpen: (toast) => {
                                toast.addEventListener('mouseenter', Swal.stopTimer)
                                toast.addEventListener('mouseleave', Swal.resumeTimer)
                            }
                        });
                        Toast.fire({
                            icon: 'success',
                            title: 'Đã hủy đơn hàng thành công'
                        });
                    } else {
                        Swal.fire('Không thể hủy đơn hàng', '', 'error');
                    }
                },
                error: function () {
                    Swal.fire('Có lỗi xảy ra khi gửi yêu cầu', '', 'error');
                }
            });
        }
    });
});
