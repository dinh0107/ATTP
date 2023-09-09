AOS.init();
window.addEventListener("load", function () {
    const loadG = sessionStorage.getItem("loadGT");
    if (loadG === "1") {
        $.getScript("https://translate.google.com/translate_a/element.js?cb=googleTranslateElementInit");
    }

    $('#mySelect').on('change', function (e) {
        var $optionSelected = $("option:selected", this);
        $optionSelected.tab('show');
    });
    $("#google_translate_element2 a").click(function () {
        var name = $(this).data('text');
        var translate = localStorage.setItem("leng", name);
        document.getElementById("mylocation").innerText = localStorage.getItem('leng');
        document.getElementById("mylocationMb").innerText = localStorage.getItem('leng');
        $(".box-collapse").removeClass("show");
    });
})

function ShowSearch() {
    $('.search').addClass("active");
}
$(".close").click(function () {
    $('.search').removeClass("active");
});
function IndexJs() {
    $('.list-banner').slick({
        dots: true,
        infinite: true,
        speed: 500,
        fade: true,
        cssEase: 'linear',
        autoplay: true,
        autoplaySpeed: 2000,
        prevArrow: '<button class="chevron-prev"><i class="far fa-chevron-left "></i></button>',
        nextArrow: '<button class="chevron-next"><i class="far fa-chevron-right"></i></button>'
    });

    $('.list-saler').slick({
        dots: false,
        infinite: true,
        speed: 800,
        slidesToShow: 4,
        slidesToScroll: 4,
        autoplay: false,
        autoplaySpeed: 2000,
        prevArrow: '<button class="chevron-prev"><i class="fal fa-chevron-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fal fa-chevron-right"></i></button>',
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3,
                    infinite: true,
                    dots: false
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }
        ]
    });

    $('.list-product').slick({
        dots: false,
        infinite: true,
        speed: 800,
        slidesToShow: 4,
        slidesToScroll: 4,
        autoplay: false,
        autoplaySpeed: 2000,
        prevArrow: '<button class="chevron-prev"><i class="fal fa-chevron-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fal fa-chevron-right"></i></button>',
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3,
                    infinite: true,
                    dots: false
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }
        ]
    });

    function ProductHome(action) {
        $(document).on('click', '.category-h', function () {
            let id = parseInt($('.category-h.active').data('value'));
            $.get(action, { id: id }, function (data) {
                $('#list-item-sort').empty();
                $('#list-item-sort').html(data);
            });
        });
    }

}


function AboutJs() {
    $('.list-member').slick({
        dots: false,
        infinite: true,
        speed: 800,
        slidesToShow: 3,
        slidesToScroll: 4,
        autoplay: false,
        autoplaySpeed: 2000,
        prevArrow: '<button class="chevron-prev"><i class="fal fa-chevron-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fal fa-chevron-right"></i></button>',
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3,
                    infinite: true,
                    dots: false
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }
        ]
    });
    $('.discover-album').slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: true,
        arrows: true,
        dots: false,
        speed: 800,
        autoplaySpeed: 2000,
        prevArrow: '<button class="chevron-prev"><i class="fal fa-chevron-left text-dark"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fal fa-chevron-right text-dark"></i></button>',
    });

    $('.video-discover').slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        arrows: false,
        fade: true,
        asNavFor: '.video-discover-nav'
    });
    $('.video-discover-nav').slick({
        slidesToShow: 3,
        slidesToScroll: 1,
        asNavFor: '.video-discover',
        dots: true,
        focusOnSelect: true
    });
    $('.parther-slick').slick({
        dots: false,
        autoplay: true,
        infinite: true,
        autoplaySpeed: 2000,
        speed: 800,
        slidesToShow: 6,
        slidesToScroll: 1,
        prevArrow: '<button class="chevron-prev"><i class="far fa-chevron-left text-dark"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="far fa-chevron-right text-dark"></i></button>',
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 4,
                    slidesToScroll: 1,
                    arrows: false,
                    infinite: true,
                    dots: false
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1
                }
            }
        ]
    });
    $('.feedback-slick').slick({
        dots: true,
        autoplay: true,
        infinite: true,
        autoplaySpeed: 2000,
        arrows: false,
        speed: 800,
        slidesToShow: 3,
        slidesToScroll: 1,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: true
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }
        ]
    });
    $('.list-member-box').slick({
        dots: false,
        infinite: true,
        speed: 500,
        fade: true,
        cssEase: 'linear',
        autoplay: true,
        autoplaySpeed: 2000,
        prevArrow: '<button class="chevron-prev"><i class="far fa-chevron-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="far fa-chevron-right"></i></button>'
    });
}
function DetailJs() {
    $('.slider-for').slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        fade: true,
        asNavFor: '.slider-nav',
        dots: true,
        arrows: true,
        prevArrow: '<button class="chevron-prev"><i class="far fa-chevron-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="far fa-chevron-right"></i></button>',
    });
    $('.slider-nav').slick({
        slidesToShow: 5,
        slidesToScroll: 1,
        asNavFor: '.slider-for',
        dots: false,
        centerMode: false,
        focusOnSelect: true,
        vertical: true,
        arrows: false,
        responsive: [
            {
                breakpoint: 830,
                settings: {
                    slidesToShow: 5,
                    slidesToScroll: 1,
                    vertical: false,
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 5,
                    slidesToScroll: 1,
                    vertical: false,
                }
            },
        ]
    });

    $(".nice-number").niceNumber();
    $('.list-product').slick({
        dots: false,
        infinite: true,
        speed: 800,
        slidesToShow: 4,
        slidesToScroll: 4,
        autoplay: false,
        autoplaySpeed: 2000,
        prevArrow: '<button class="chevron-prev"><i class="fal fa-chevron-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fal fa-chevron-right"></i></button>',
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3,
                    infinite: true,
                    dots: false
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }
        ]
    });



}
function Sort(action) {
    $(document).on("click", "[data-filter]", function () {
        let sort = "";
        let url = $("input[name=currentUrl]").val();
        let catId = $("input[name=catId]").val();
        let keywords = $("input[name=keywords]").val();
        const title = $(".breadcrumb-item.active").text();
        $("[name=catId]:checked").map(function () {
            catId += $(this).val();
        });
        $("[name=sort]:checked").map(function () {
            sort += $(this).val();
        });
        $("[name=keywords]").on("change", function () {
            keywords += $(this).val();
        });
        console.log(catId)

        url = url.split('/').at(-1);
        window.history.pushState(title, "", url);
        $.get(action, { keywords: keywords, sort: sort, catId: catId, url }, function (data) {
            $("#list-item-sort").empty();
            $("#list-item-sort").html(data);
            var t = $('#list-item-sort').position().top;
            $('html,body').stop().animate({ scrollTop: t }, 400);
        });
    });
}
$(".form-contact").on("submit", function (e) {
    e.preventDefault();
    if ($(this).valid()) {
        $.post("/Home/Contact", $(this).serialize(), function (data) {
            if (data.status) {
                $.toast({
                    heading: 'Liên hệ thành công',
                    text: data.msg,
                    icon: 'success'
                });
                $(".form-contact").trigger("reset");
            } else {
                $.toast({
                    heading: 'Liên hệ không thành công',
                    text: data.msg,
                    icon: 'error'
                });
            }
        });
    }
});
$(".form-subcribe").on("submit", function (e) {
    e.preventDefault();
    $.post("/Home/SubcribeForm", $(this).serialize(), function (data) {
        if (data.status) {
            $.toast({
                heading: "Liên hệ thành công",
                text: data.msg,
                icon: "success"
            });
            $(".form-subcribe").trigger("reset");
        } else {
            $.toast({
                heading: "Liên hệ không thành công",
                text: data.msg,
                icon: "error"
            });
        }
    });
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
function UpdateToCard(id, changeValue) {
    $.ajax({
        type: "Post",
        url: "/ShoppingCart/UpdateCartV2", data: { productId: id, changeValue },
        success: function (res) {
            if (res) {
                $("[data-cart-item=" + id + "]").text(res.total);
                $("#finalTotal").html(res.totalMoneyString);
                $("#finalcart").html(res.totalMoneyString)
                $.toast({
                    heading: res.Msg,
                    position: "bottom-right",
                    icon: "success"
                });
            } else {
                $.toast({
                    heading: res.Msg,
                    position: "bottom-right",
                    icon: "error"
                });
            }
        }
    })
}
$(".remove-product").click(function () {
    const recordToDelete = $(this).attr("data-id");
    if (recordToDelete !== "") {
        Swal.fire({
            title: 'Bạn có muốn xóa sản phẩm?',
            text: "Sản phẩm sẽ bị xóa khỏi giỏ hàng",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Xóa'
        }).then((result) => {
            if (result.isConfirmed) {

                $.post("/ShoppingCart/RemoveFromCart", { "id": recordToDelete }, function (data) {
                    if (data.ItemCount === 1) {
                        alert("Quá trình thực hiện không thành công");

                    } else {
                        $("tr[data-row='" + recordToDelete + "']").fadeOut();
                        function reload() {
                            location.reload();
                        }
                        setTimeout(reload, 500);
                        Swal.fire(
                            'Xóa thành công!',
                            'Sản phẩm đã bị xóa khỏi giỏ hàng',
                            'success'
                        )
                    }
                });


            }
        })

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
    items.push("<option value>Phường / Xã</option>");

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
function userAddress() {
    $('.btn-address').click(function () {
        $('.user-address').addClass('active');
        $('.form-address').addClass('active');
    });

    $('.close-form-address').click(function () {
        $('.user-address').removeClass('active');
        $('.form-address').removeClass('active');
    });
}
function userOrder(action) {
    $(document).on('click', '.nav-link', function () {
        let status = parseInt($('.nav-link.active').val());
        $.get(action, { status: status }, function (data) {
            $('#list-item-sort').empty();
            $('#list-item-sort').html(data);
        });
    });
}
function defaultAddress(id) {
    var divId = $("div[data-id='" + id + "']");
    var addressDefault = divId.find("input.Default").prop("checked");
    var checkDefault = divId.hasClass('default');

    if (checkDefault == true) {
        addressDefault = true;
    }

    $.post("/Dashboard/DefaultAddress", { addressId: id, addressDefault }, function (data) {
        if (data) {
            location.reload();
        }
    });
}
function checkOut() {
    $('.btn-list-address').click(function (e) {
        $('.address-default').addClass('active');
        $('.list-address').addClass('active');
    });
    $('.btn-back').click(function (e) {
        $('.address-default').removeClass('active');
        $('.list-address').removeClass('active');
    });
}
function addToCart(n, m) {
    $.post("/gio-hang/them-vao-gio-hang", { productId: n, userId: m }, function (n) {
        n.result === 1
            ? ($.toast({
                text: "Thêm vào giỏ hàng thành công",
                icon: "success",
                position: "bottom-right",
            }),
                $("#itemshop").text(n.count))
            : $.toast({
                text: "Quá trình thực hiện không thành công",
                icon: "error",
                position: "bottom-right",
            });
    });
}
$(".reply-form").on("submit", function (e) {
    e.preventDefault();
    $.post("/Home/Reply", $(this).serialize(), function (data) {
        if (data.status) {
            $.toast({
                heading: "Đánh giá thành công",
                text: data.msg,
                icon: "success",
                position: "bottom-right"
            });
            $(".reply-form").trigger("reset");
        } else {
            $.toast({
                heading: "Đánh giá không thành công",
                text: data.msg,
                icon: "error",
                position: "bottom-right"
            });
        }
    });
});

function GTranslateFireEvent(a, b) {
    try {
        if (document.createEvent) {
            var c = document.createEvent("HTMLEvents");
            c.initEvent(b, true, true);
            a.dispatchEvent(c);
        } else {
            var c = document.createEventObject();
            a.fireEvent("on" + b, c);
        }
    } catch (e) {
    }
}
function doTranslate(a) {
    const loadG = sessionStorage.getItem("loadGT");
    if (loadG === null) {
        $.getScript("/Scripts/Google_element.js");
        sessionStorage.setItem("loadGT", 1);
    }

    if (a.value) a = a.value;
    if (a === "") return;
    const b = a.split("|")[1];
    var c;
    const d = document.getElementsByTagName("select");
    for (let i = 0; i < d.length; i++) if (d[i].className === "goog-te-combo") c = d[i];
    if (document.getElementById("google_translate_element2") === null || document.getElementById('google_translate_element2').innerHTML.length === 0 || c.length === 0 || c.innerHTML.length === 0) {
        setTimeout(function () {
            doTranslate(a);
        }, 500);
    } else {
        c.value = b;
        GTranslateFireEvent(c, "change");
        GTranslateFireEvent(c, "change");
    }
}
function googleTranslateElementInit() {
    const translateElement = new google.translate.TranslateElement({
        pageLanguage: "vi",
        includedLanguages: "vi,en,zh-CN,de,fr,ja,ko,ru",
        multilanguagePage: false,
        autoDisplay: false
    }, "google_translate_element2");
    $("#goog-gt-tt h1").remove();
}
$('.review-comment__reply.show-sub').click(function () {
    var form = $(this).closest('.row').find('.reply-form');

    if (form.is(':hidden')) {
        form.slideDown(); // Hiển thị form với hiệu ứng slide
    } else {
        form.slideUp(); // Ẩn form với hiệu ứng slide
    }
});
function UpdateToCard(id, changeValue) {
    $.ajax({
        type: "Post",
        url: "/ShoppingCart/UpdateCartV2", data: { productId: id, changeValue },
        success: function (res) {
            if (res) {
                $("[data-cart-item=" + id + "]").text(res.total + "đ");
                $("#finalTotal").html(res.totalMoneyString + "đ");
                $("#finalcart").html(res.totalMoneyString + "đ")
                $.toast({
                    heading: res.Msg,
                    position: "bottom-right",
                    icon: "success"
                });
            } else {
                $.toast({
                    heading: res.Msg,
                    position: "bottom-right",
                    icon: "error"
                });
            }
        }
    })
}
$("#form-subscribe").on("submit", function (e) {
    e.preventDefault();
    $.post("/Home/SubcribeForm", $(this).serialize(), function (data) {
        $('.form-subscribe input').val("");
        if (data.status) {
            $.toast({
                heading: "Gửi liên hệ thành công",
                text: data.msg,
                icon: "success",
                position: "bottom-right"
            });
            $('#form-subscribe').trigger("reset");

        } else {
            $.toast({
                heading: "Liên hệ không thành công",
                text: data.msg,
                icon: "error",
                position: "bottom-right"
            });
        }
    });
});
$('.review-comment__thank').click(function () {
    var reviewId = $(this).data('review-id');
    var spanElement = $(this);
    $.ajax({
        type: 'POST',
        url: '/Home/ToggleLikeForReview?reviewId=' + reviewId,
        success: function (data) {
            if (data.success) {
                if (data.liked) {
                    spanElement.addClass('liked');
                } else {
                    spanElement.removeClass('liked');
                }
                spanElement.text('Hữu ích(' + data.Likes + ')');
            }
        }
    });
});
function showMb() {
    $(".header-mb").addClass("active");
    $(".overlay").addClass(" active");
}
function HidenMb() {
    $(".header-mb").removeClass("active");
    $(".overlay").removeClass(" active");
}
$(".typepay").change(function () {
    var bankValue = $(".typepay").val()
    if (bankValue == 2) {
        $.fancybox.open({
            src: "#dialog-content",
            type: "inline"
        });
    }
})
$(window).scroll(function () {
    if ($(this).scrollTop()) {
        $('.backtotop').fadeIn();
    } else {
        $('.backtotop').fadeOut();
    }
});

$(".backtotop").click(function () {
    $("html, body").animate({ scrollTop: 0 }, 1000);
});
