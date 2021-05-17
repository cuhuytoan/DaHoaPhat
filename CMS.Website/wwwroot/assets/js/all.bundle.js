// Example starter JavaScript for disabling form submissions if there are invalid fields
(function () {
  "use strict";

  window.addEventListener("load", function () {
    // Fetch all the forms we want to apply custom Bootstrap validation styles to
    var forms = document.getElementsByClassName("needs-validation"); // Loop over them and prevent submission

    var validation = Array.prototype.filter.call(forms, function (form) {
      form.addEventListener("submit", function (event) {
        if (form.checkValidity() === false) {
          event.preventDefault();
          event.stopPropagation();
        }

        form.classList.add("was-validated");
      }, false);
    });
  }, false);
  var scrollPage = window.addEventListener("scroll", function () {
    var scrollDistance = $(window).scrollTop();

    if (scrollDistance >= 5) {
      $("#site-header").stop().addClass("header-scrolling bg-white");
    } else {
      $("#site-header").stop().removeClass("header-scrolling bg-white");
    }

    if (scrollDistance >= 100) {
      $("#to_top").stop().addClass("show");
    } else {
      $("#to_top").stop().removeClass("show");
    }
  });

  function getDebounce(func, wait, immediate) {
    var timeout;
    return function () {
      var context = this,
          args = arguments;

      var later = function later() {
        timeout = null;
        if (!immediate) func.apply(context, args);
      };

      var callNow = immediate && !timeout;
      clearTimeout(timeout);
      timeout = setTimeout(later, wait);
      if (callNow) func.apply(context, args);
    };
  }

  var resizeDebounce = getDebounce(function () {
    if ($(window).scrollTop() >= 10) {
      $("#site-header").stop().addClass("header-scrolling bg-white");
    } else {
      $("#site-header").stop().removeClass("header-scrolling bg-white");
    }

    scrollPage;
  }, 250);
  window.addEventListener("resize", resizeDebounce);
  $(".all-menu").on("click", function (e) {
    $('.wrap-main-nav').toggleClass('show-all-menu');
    $('body').toggleClass('show_main_menu');
  });
  $(".close-menu").on("click", function (e) {
    $('.wrap-main-nav').removeClass('show-all-menu');
    $('body').removeClass('show_main_menu');
  });
})();

$(document).ready(function () {
  var _this = this;

  $("#to_top").click(function () {
    $('html, body').animate({
      scrollTop: 0
    }, 800);
    return false;
  });
  $(".view_all_reply").on("click", function (e) {
    $(_this).find(".sub_comment").css({
      display: "block"
    });
    $(".view_all_reply").addClass("hidden");
  });
  var width = $(window).innerWidth();
  console.log(width);
  $(window).on('resize load', function (e) {
    if (width < 768) {
      $('body').addClass('body_mobile personalized-page ');
    }
  });

  window.onscroll = function (ev) {
    var menu = document.querySelector('.section_nav_v2');

    if (menu.offsetTop >= 100) {
      menu.classList.add('sticky-nav');
    } else {
      menu.classList.remove('sticky-nav');
    }
  };
});
//# sourceMappingURL=maps/all.bundle.js.map
