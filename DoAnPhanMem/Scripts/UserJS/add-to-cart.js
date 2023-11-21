// Sự kiện click [Thêm vào giỏ] ở trang detail
$(".btnAddToCart").click(function () {
    // Hiển thị thông báo
    Swal.fire({
        position: 'center',
        icon: 'success',
        title: 'Thêm vào giỏ thành công',
        showConfirmButton: false,
        timer: 1000
    });

});
