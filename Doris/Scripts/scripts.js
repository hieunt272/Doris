AOS.init({
    once: true,
});

function homeJs() {
    $(".banner").slick({
        dots: false,
        infinite: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        speed: 1000,
        autoplay: true,
        autoplaySpeed: 3000,
        prevArrow: '<button class="chevron-prev"><i class="fa-light fa-angle-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fa-light fa-angle-right"></i></button>',
    });

    $(".banner-center-list").slick({
        dots: true,
        infinite: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        speed: 1000,
        autoplay: false,
        autoplaySpeed: 3000,
        arrows: false
    });

    $(".brands").slick({
        dots: false,
        infinite: true,
        slidesToShow: 7,
        slidesToScroll: 1,
        speed: 1000,
        autoplay: false,
        autoplaySpeed: 3000,
        prevArrow: '<button class="chevron-prev"><i class="fa-light fa-angle-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fa-light fa-angle-right"></i></button>',
        responsive: [
            {
                breakpoint: 992,
                settings: {
                    slidesToShow: 2,
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 1,
                }
            }
        ]
    });

    $(".product-slide").slick({
        dots: false,
        infinite: true,
        slidesToShow: 2,
        slidesToScroll: 1,
        speed: 1000,
        autoplay: false,
        autoplaySpeed: 3000,
        prevArrow: '<button class="chevron-prev"><i class="fa-light fa-angle-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fa-light fa-angle-right"></i></button>'
    });
}

function contact() {
    $("#contactForm").on("submit", function (e) {
        e.preventDefault();
        if ($("#contactForm").valid()) {
            $.post("/Home/ContactForm", $(this).serialize(), function (data) {
                if (data.status) {
                    $.toast({
                        heading: "Gửi liên hệ thành công",
                        text: data.msg,
                        icon: "success",
                        position: "bottom-right"
                    });
                    $("#contactForm").trigger("reset");
                } else {
                    $.toast({
                        heading: "Liên hệ không thành công",
                        text: data.msg,
                        icon: "error",
                        position: "bottom-right"
                    });
                }
            });
        }
    });
}

function productDetail() {
    $('.slider-for').slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        fade: true,
        asNavFor: '.slider-nav',
        prevArrow: '<button class="chevron-prev"><i class="fa-light fa-angle-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fa-light fa-angle-right"></i></button>',
    });
    $('.slider-nav').slick({
        slidesToShow: 4,
        slidesToScroll: 1,
        asNavFor: '.slider-for',
        dots: false,
        centerMode: false,
        focusOnSelect: true,
        arrows: false,
        responsive: [
            {
                breakpoint: 830,
                settings: {
                    slidesToShow: 5,
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 4,
                }
            },
        ]
    });

    $(".input-number").niceNumber();

    $(".toggle-btn").click(function () {
        $(".content-detail").toggleClass('hide');
        var txt = $(this).text();

        if (txt == "Thu gọn") {
            $(this).text("Xem thêm");
        }
        else {
            $(this).text("Thu gọn");
        }
    });

    $(".btn-light").click(function () {
        var quantity = $("#number").val();
        var price = $("#priceHidden").val();
        var total = parseInt(quantity * price);
        $("#totalPrice").text(total.toLocaleString('vi') + "đ");
    });
}

function productDetailMB() {
    $(".product-related").slick({
        dots: false,
        infinite: true,
        speed: 1000,
        slidesToShow: 3,
        slidesToScroll: 1,
        centerMode: false,
        variableWidth: true,
        prevArrow: '<button class="chevron-prev"><i class="fa-light fa-angle-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fa-light fa-angle-right"></i></button>',
    });
}

function promotion() {
    $(".promotion-banner").slick({
        dots: true,
        infinite: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        speed: 1000,
        autoplay: true,
        autoplaySpeed: 3000,
        prevArrow: '<button class="chevron-prev"><i class="fa-light fa-angle-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fa-light fa-angle-right"></i></button>',
    });
};

function brand() {
    $(".char-list-mb").slick({
        dots: false,
        infinite: true,
        slidesToShow: 9,
        slidesToScroll: 4,
        speed: 1000,
        autoplay: false,
        autoplaySpeed: 3000,
        variableWidth: true,
        prevArrow: '<button class="chevron-prev"><i class="fa-light fa-angle-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fa-light fa-angle-right"></i></button>',
    });
}

function profile() {
    var avg1 = "", avg2 = "", splitted, part1, part2, part3, part4;
    var email = $('.mail-secret').text();
    splitted = email.split("@");
    part1 = splitted[0].substring(0, 2);
    for (var i = 0; i < splitted[0].length - 2; i++) {
        avg1 += "*";
    }
    part2 = part1 + avg1 + "@" + splitted[1];
    $('.mail-secret').text(part2);

    var phone = $('.phone-secret').text();
    part3 = phone.slice(-2);
    for (var i = 0; i < phone.length - part3.length; i++) {
        avg2 += "*";
    }
    part4 = avg2 + part3;
    $('.phone-secret').text(part4);

    $('.btn-change').click(function () {
        $(this).parent().hide();
        $(this).parent().siblings('.input-hidden').show();
    });
}

function address() {
    $('.btn-add-address').click(function () {
        $(".add-address, #list-address").hide();
        $(".address-form").show();
    });

    $('.close-form-address').click(function () {
        $(".add-address, #list-address").show();
        $(".address-form").hide();
    });
}

function checkout() {
    $('.btn-list-address').click(function (e) {
        $('.address-default').addClass('active');
        $('.list-address').addClass('active');
    });
    $('.btn-back').click(function (e) {
        $('.address-default').removeClass('active');
        $('.list-address').removeClass('active');
    });

    $('.bill-info').click(function () {
        var check = $('input[name="Bill"]:checked').length > 0;
        var type = $('.nav-link.active').attr('data-bill');
        if (check) {
            $('.box-bill').show();
            if (type == "Business") {
                $('#typeBill').val(1);
            }
            else if (type == "Personal") {
                $('#typeBill').val(2);
            }
        }
        else {
            $('.box-bill').hide();
            $('#typeBill').val(0);
        }
    });

    $('.nav-link').click(function () {
        var type = $(this).attr("data-bill");
        if (type == "Business") {
            $('#typeBill').val(1);
        }
        else if (type == "Personal") {
            $('#typeBill').val(2);
        }
    });
}

function checkoutMb() {
    $("#addressCustomerForm .btn-cart").click(function () {
        var city = $("#CityId option:selected").text();
        var district = $("#DistrictId option:selected").text();
        var ward = $("#WardId option:selected").text();
        var address = $("#SpecialAddress").val();
        $("#Order_CustomerInfo_Address").val(address + ", " + ward + ", " + district + ", " + city);
        $('#addressModal').modal('hide');
    });
};

function alertBox() {
    $("#AlertBox").fadeOut(1000);
}

$(document).ready(function () {
    $(window).scroll(function () {
        if ($(this).scrollTop() > 200) {
            $('.btn-scroll').fadeIn(200);
            $('.btn-scroll-mb').fadeIn(200);
        } else {
            $('.btn-scroll').fadeOut(200);
            $('.btn-scroll-mb').fadeOut(200);
        }
    });

    $('.btn-scroll-mb').click(function (event) {
        event.preventDefault();
        $('html, body').animate({ scrollTop: 0 }, 300);
    });

    $(".btn-search").click(function () {
        $(".body-overlay").addClass('active');
        $(".site-search").addClass('active');
        function delay() {
            $(".site-search .form-control").focus();
        }

        setTimeout(delay, 300);
    });

    $(".body-overlay, .site-search-close").click(function () {
        $(".body-overlay").removeClass('active');
        $(".site-search").removeClass('active');
    });

    $(".char-list li").click(function () {
        var char = $(this).text();
        $(this).addClass("active");
        $(this).siblings().removeClass("active");

        $.get("/Home/GetBrand", { c: char }, function (data) {
            $("#list-brand-sort").empty();
            $("#list-brand-sort").html(data);
        });
    });

    $(".char-list-mb li").click(function () {
        var char = $(this).text();
        $(this).addClass("active");
        var item = $(this).parents(".slick-slide").siblings().find("li").removeClass("active");

        $.get("/Home/GetBrandMobile", { c: char }, function (data) {
            $("#list-brand-sort").empty();
            $("#list-brand-sort").html(data);
        });
    });

    $('#popup').modal('show');

    setTimeout(alertBox, 2000);

    $(".category-slide").slick({
        dots: false,
        infinite: true,
        slidesToShow: 5,
        slidesToScroll: 1,
        speed: 1000,
        autoplay: false,
        autoplaySpeed: 3000,
        prevArrow: '<button class="chevron-prev"><img src="/Content/images/img-main/long-arrow.png" alt="Arrow" /></button>',
        nextArrow: '<button class="chevron-next"><img src="/Content/images/img-main/long-arrow.png" alt="Arrow" /></button>',
    });

    $(".toggle-brand").click(function () {
        $(".category-list").toggleClass('hide');
        var txt = $(this).find("span").text();

        if (txt == "Thu gọn") {
            $(this).find("span").text("Xem thêm");
            $(this).find("i").attr("class", "fa-light fa-angles-down");
        }
        else {
            $(this).find("span").text("Thu gọn");
            $(this).find("i").attr("class", "fa-light fa-angles-up");
        }
    });

    $(".banner-mb").slick({
        dots: true,
        infinite: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        speed: 1000,
        autoplay: false,
        autoplaySpeed: 3000,
        arrows: false
    });

    $("#formBookProduct").on("submit", function (e) {
        e.preventDefault();
        $.post("/gio-hang/them-vao-gio-hang", $(this).serialize(), function (data) {
            if (data.result === 1) {
                $.toast({
                    text: "Thêm vào giỏ hàng thành công",
                    icon: "success",
                    position: "bottom-right"
                });
                $("#itemshop").text(data.count);
            } else {
                $.toast("Quá trình thực hiện không thành công");
            }
        });
    });
});

$(function () {
    $(".subcribe-form").on("submit", function (e) {
        e.preventDefault();
        if ($(".subcribe-form").valid()) {
            $.post("/Home/SubcribeForm", $(this).serialize(), function (data) {
                if (data.status) {
                    $.toast({
                        heading: "Gửi đăng ký thành công",
                        text: data.msg,
                        icon: "success",
                        position: "bottom-right"
                    });
                    $(".subcribe-form").trigger("reset");
                } else {
                    $.toast({
                        heading: "Đăng ký không thành công",
                        text: data.msg,
                        icon: "error",
                        position: "bottom-right"
                    });
                }
            });
        }
    });

    $(".contact-form").on("submit", function (e) {
        e.preventDefault();
        if ($(".contact-form").valid()) {
            $.post("/Home/ContactForm", $(this).serialize(), function (data) {
                if (data.status) {
                    $.toast({
                        heading: "Gửi đăng ký đại lý thành công",
                        text: data.msg,
                        icon: "success",
                        position: "bottom-right"
                    });
                    $(".contact-form").trigger("reset");
                } else {
                    $.toast({
                        heading: "Đăng ký đại lý không thành công",
                        text: data.msg,
                        icon: "error",
                        position: "bottom-right"
                    });
                }
            });
        }
    });

    $("[data-item=city]").on("change", function (data) {
        const id = $(this).val();
        var items = [];
        items.push("<option value>Chọn Quận/Huyện</option>");

        if (id !== "") {
            $.getJSON("/Base/GetDistrict", { cityId: id }, function (data) {
                $.each(data, function (key, val) {
                    items.push("<option value='" + val.Id + "'>" + val.Name + "</option>");
                });
                $("[data-item=district]").html(items.join(""));
            });
        } else {
            $("[data-item=district]").html(items.join(""));
        }
    });

    $("[data-item=district]").on("change", function (data) {
        const id = $(this).val();
        var items = [];
        items.push("<option value>Chọn Phường/Xã</option>");

        if (id !== "") {
            $.getJSON("/Base/GetWard", { districtId: id }, function (data) {
                $.each(data, function (key, val) {
                    items.push("<option value='" + val.Id + "'>" + val.Name + "</option>");
                });
                $("[data-item=ward]").html(items.join(""));
            });
        } else {
            $("[data-item=ward]").html(items.join(""));
        }
    });

    $(".remove-product").click(function () {
        if (confirm("Bạn có chắc chắn xóa sản phẩm này khỏi giỏ hàng?")) {
            const recordToDelete = $(this).attr("data-id");
            if (recordToDelete !== "") {
                $.post("/ShoppingCart/RemoveFromCart", { "id": recordToDelete }, function (data) {
                    if (data.Status === 1) {
                        $("div[data-cart-id='" + recordToDelete + "']").fadeOut();
                        function reload() {
                            location.reload();
                        }
                        setTimeout(reload, 700);
                    } else {
                        alert("Quá trình thực hiện không thành công");
                    }
                });
            }
        }
    });
});

function cart() {
    $(".cart-quantity .btn-light").click(function () {
        var quantity = $(this).siblings(".input-number").val();
        var cartId = $(this).parents(".cart-id").attr("data-cart-id");

        $.post("/ShoppingCart/UpdateCart", { cartId: cartId, quantity: quantity }, function (data) {
            if (data.result === 1) {
                location.reload();
            }
        });
    });
}

function Sort(action) {
    $("[data-filter]").on("change", function () {
        const catId = $("select[name=catId]").val();
        const brandId = $("select[name=brandId]").val();
        const sort = $("select[name=sort]").val();
        let url = $("input[name=currentUrl]").val();
        const keywords = $("input[name=keywords][type=hidden]").val();
        const title = $(".breadcrumb-item.active").text();

        url = url.split("/").at(-1);

        window.history.pushState(title, "", url);

        $("body").append('<div class="loading"><i class="fad fa-spin fa-spinner"></i></div>');
        $.get(action, { keywords: keywords, url, catId: catId, brandId: brandId, sort: sort }, function (data) {
            $("#list-item-sort").empty();
            $("#list-item-sort").html(data);
        }).then(function () {
            $(".loading").remove();
        });
    });
}

function userOrder(action) {
    $(document).on('click', '.nav-link', function () {
        let status = parseInt($('.nav-link.active').val());

        $("body").append('<div class="loading"><i class="fad fa-spin fa-spinner"></i></div>');
        $.get(action, { status: status }, function (data) {
            $('#list-order').empty();
            $('#list-order').html(data);
        }).then(function () {
            $(".loading").remove();
        });
    });
}

function userOrderMobile(action) {
    $(document).on('click', '.nav-link', function () {
        let status = parseInt($('.nav-link.active').val());

        $("body").append('<div class="loading"><i class="fad fa-spin fa-spinner"></i></div>');
        $.get("/Dashboard/GetOrderMobile", { status: status }, function (data) {
            $('#orderList').empty();
            $('#orderList').html(data);
        }).then(function () {
            $(".loading").remove();
        });
    });
}