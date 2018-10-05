$(document).ready(function(){
  /* VARIABLES GLOBALES */ 	
  var $win = $(window);
  var userProfil = $('.profil');
  var popup = userProfil.next('.profil-menu');
  var mainWrapper = $('main#wrapper');

  $(window).scroll(function(e){
    // Scroll events
    var winScroll = $(window).scrollTop();   
    var topBar = $('#top-bar'); 
    if(winScroll > 0){
      $(topBar).addClass('fixedNav');
    } else {
      $(topBar).removeClass('fixedNav');
    }
    var onglets = $('#single-container .onglets');
    if(winScroll > 10){
      $(onglets).addClass('fixedOnglets');
    } else {
      $(onglets).removeClass('fixedOnglets');
    }
  }); 
  
  $( 'body' ).on( 'keydown', function ( e ) {
    if ( e.keyCode === 27 ) {
        popup.slideUp();
        popup.css('display','none');
        mainWrapper.removeClass('fade-bg');
      }
  });

  $(function (){   
    $win.on("click", function(event){
      if ( userProfil.has(event.target).length == 0 && !userProfil.is(event.target) ){
        popup.slideUp();
        popup.css('display','none');
        mainWrapper.removeClass('fade-bg');
      } else {
        popup.toggleClass('open').slideDown();
        mainWrapper.addClass('fade-bg')
      }
    });
  });

  $('input[type="file"]').change(function(e){
    var fileName = e.target.files[0].name;
    $('.first .file').append("<div class='filename'>" + fileName + "</div>");
  });


  $('input[type="file"]').change(function(e){
    var fileName = e.target.files[0].name;
    $('.file-name').text(fileName);
});

$('.theme-select .choose-color').click(function () {
    var themeparam = $(this).next('.theme-param'),
        actionBtn = $(this).closest('.create-theme').siblings();
    var controlGroup = themeparam.children('.control-group');

    controlGroup.removeClass('show');
    controlGroup.toggleClass('show');
   // console.log(actionBtn)
    $('.theme-param').not(themeparam).hide();
    themeparam.show();
});

$('.cancel-color-choice').click(function () {
    $('.theme-param').hide();
})
 
});
