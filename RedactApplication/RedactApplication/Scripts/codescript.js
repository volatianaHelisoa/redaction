
$(document).ready(function () {
    /* VARIABLES GLOBALES */
    var $win = $(window);
    var userProfil = $('.profil');
    var popup = userProfil.next('.profil-menu');

    var notifsActionner = $('#notification-bell');
    var notifsContent = $('#notification-bell').next('.notifs-content');

    var mainWrapper = $('main#wrapper');

    $(window).scroll(function (e) {
        // Scroll events
        var winScroll = $(window).scrollTop();
        var topBar = $('#top-bar');
        if (winScroll > 0) {
            $(topBar).addClass('fixedNav');
        } else {
            $(topBar).removeClass('fixedNav');
        }
        var onglets = $('#single-container .onglets');
        if (winScroll > 50) {
            $(onglets).addClass('fixedOnglets');
            $('a.back-link').show();
        } else {
            $(onglets).removeClass('fixedOnglets');
            $('a.back-link').hide();
        }
    });

    $('body').on('keydown', function (e) {
        if (e.keyCode === 27) {
            popup.hide();
            notifsContent.hide();
            mainWrapper.removeClass('fade-bg');
        }
    });

    notifsActionner.click(function (e) {
        $(this).toggleClass("active");
        $(".notifs-content").slideToggle();
        $(this).children('.count-notif').hide();
        e.stopPropagation();
    });

    userProfil.click(function (e) {
        $(this).toggleClass("active");
        popup.slideToggle();
        e.stopPropagation();
    });

    var loadFile = function (event) {
        var reader = new FileReader();
        reader.onload = function () {
            var output = document.getElementById('new-profil-preview');
            output.src = reader.result;
        };
        reader.readAsDataURL(event.target.files[0]);
    };

    $('input[type="file"].fileTriger').change(function (e) {
        var fileName = e.target.files[0].name;
        loadFile(event);
        $('.file-name').text(fileName);
    });

});


//Select all User in ListUser when the checkbox is selected.
function ClickAllUserInListUser() {
    if (document.getElementById('checkAllUser').checked) {
        CheckedClick();
    }
    else {
        DecheckedClick();
    }
}

function CheckedClick() {
    $('input[name="selectedUser"]').each(function () {
        $(this).prop("checked", true);
    });
}

function DecheckedClick() {
    $('checkAllUser').prop("checked", false);
    $('input[name="selectedUser"]:checked').each(function () {
        $(this).prop("checked", false);
    });
}

//Select all Commande in ListCommande when the checkbox is selected.
function ClickAllCommandeInListCommande() {
    if (document.getElementById('checkAllCmde').checked) {
        CheckedCmdeClick();
    }
    else {
        DecheckedCmdeClick();
    }
}

function CheckedCmdeClick() {

    $('input[name="selectedCmde"]').each(function () {
        $(this).prop("checked", true);
    });
}

function DecheckedCmdeClick() {
    $('checkAllCmde').prop("checked", false);
    $('input[name="selectedCmde"]:checked').each(function () {
        $(this).prop("checked", false);
    });
}



//Select all facture in liste facture when the checkbox is selected.
function ClickAllFactureInListFacture() {
    if (document.getElementById('checkAllFacture').checked) {
        CheckedFactureClick();
    }
    else {
        DecheckedFactureClick();
    }
}

function CheckedFactureClick() {
    $('input[name="selectedFacture"]').each(function () {
        $(this).prop("checked", true);
    });
}

function DecheckedFactureClick() {
    $('checkAllFacture').prop("checked", false);
    $('input[name="selectedFacture"]:checked').each(function () {
        $(this).prop("checked", false);
    });
}

//Select all template in list template when the checkbox is selected.
function ClickAllTemplateInListTemplate() {
    if (document.getElementById('checkAllTemplate').checked) {
        CheckedTemplateClick();
    }
    else {
        DecheckedTemplateClick();
    }
}

function CheckedTemplateClick() {
    $('input[name="selectedTemplate"]').each(function () {
        $(this).prop("checked", true);
    });
}

function DecheckedTemplateClick() {
    $('checkAllTemplate').prop("checked", false);
    $('input[name="selectedTemplate"]:checked').each(function () {
        $(this).prop("checked", false);
    });
}

