$(document).ready(function(){
  /* table odd */
  var trOdd = $("#facture-content tbody tr:odd");
  trOdd.css("background-color", "#F5F5F5");
  /* VARIABLES GLOBALES */ 	
  var $win = $(window);
  var userProfil = $('.profil');
  var popup = userProfil.next('.profil-menu');

  var notifsActionner = $('#notification-bell');
  var notifsContent = notifsActionner.next('.notifs-content');

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
      $('a.back-link').show();
    } else {
      $(onglets).removeClass('fixedOnglets');
      $('a.back-link').hide();
    }
  }); 
  
  $( 'body' ).on( 'keydown', function ( e ) {
    if ( e.keyCode === 27 ) {
        popup.hide();
        notifsContent.hide();
        mainWrapper.removeClass('fade-bg');
      }
  });

  notifsActionner.click(function(e) {
    $(this).toggleClass("active");
    notifsContent.toggle();
    e.preventDefault();    
  });

  userProfil.click(function(e) {
    $(this).toggleClass("active");
    popup.toggle();
    e.preventDefault(); 
  });

  var recentNotifs = $('#notiContent li.recent').length;
  console.log(recentNotifs);
  $('.count-notif').text(recentNotifs);
  
  $(function (){
    $win.on("click", function(event){
      
      if ( userProfil.has(event.target).length == 0 && !userProfil.is(event.target) ){
        popup.slideUp();
        popup.css('display','none');
        mainWrapper.removeClass('fade-bg');
      }
      else {
        popup.toggleClass('open').fadeToggle();
      }

    });
  });

  $('.content-to-show').each(function(){
    var current = null;
    current = $(this).find('.item:first');
    current.addClass('active');

    var currentTab = null;
    currentTab = $(this).find('#tab-navigation li:first a').addClass('tab-nav-active')

    $('#tab-navigation a').click(function(e){
      e.preventDefault();
      var tab = $(this).data('content');    
      $('.item').removeClass('active');
      $('#'+tab).addClass('active');    
      $('#tab-navigation a').removeClass('tab-nav-active');    
      $(this).addClass('tab-nav-active');
    });
  })

  // Copie dans le presse-papier
  /*var toCopy  = document.getElementById( 'toCopy' ),
      btnCopy = document.getElementById( 'copy' );

    btnCopy.addEventListener( 'click', function(){
    toCopy.select();
    console.log(toCopy.length);
    
    if ( document.execCommand( 'copy' ) ) {
        btnCopy.classList.add( 'copied' );
        
        var temp = setInterval( function(){
            btnCopy.classList.remove( 'copied' );
            clearInterval(temp);
        }, 600 );
        
      } else {
          console.info( 'document.execCommand went wrong…' )
      }    
      return false;
    });*/
  
  var loadFile = function(event) {
    var reader = new FileReader();
    reader.onload = function(){
      var output = document.getElementById('new-profil-preview');
      output.src = reader.result;
    };
    reader.readAsDataURL(event.target.files[0]);
  };

  $('input[type="file"]').change(function(e){
	var fileName = e.target.files[0].name;
	var fileSize = (this.files[0].size / 1024 / 1024 ),
		files = e.target.files,
		elem = $(e.target),
		errorMessage = 'Fichier trop volumineux ou non pris en charge';

	if (typeof FileReader == "undefined" || fileSize > 0.5){
		elem.parents().children('p').text(errorMessage).addClass('error');
	}
	else {
		$(this).next('label').children('span').text(fileName);
		$(this).parents().children('i').css("display","inline-block");
		elem.parents().children('p').text("Image au format JPG, PNG ou GIF").removeClass('error');
	}
  });
	
	$(".remove-file").click(function(e){
    $(this).parents().children('input[type="file"]').val("").clone(true);
    $(this).parents().children('label').children('.file-name').text("Choisir un fichier");
    $(this).hide();
  });

  $("a.auto-download").click(     
    function(){
      var $url = $("a.auto-download").attr("href");
      $("a.auto-download").attr("href", $url);
      window.open($url, '_blank');
    }
  );
  $(window).on('load', function() {
    $("a.auto-download").trigger('click');
  });

  /* CLICK AND OPEN THEME SELECTION */
  
	$('.theme-select .item').click(function(){
		
		var themeparam = $(this).children('.theme-param'),
			actionBtn = $(this).children().closest('.create-theme').siblings();
		var controlGroup = themeparam.children('.control-group');
		
		controlGroup.removeClass('show');
		controlGroup.toggleClass('show');
		$('.theme-param').not(themeparam).hide();
		themeparam.show();
	});
	
	$('.cancel-color-choice').click(function(e){
		var x = e.target;
		$('.theme-param').hide(e.target);
	})
});
