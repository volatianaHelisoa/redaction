$(document).ready(function(){
    
	$(".menuMobile").click(function() {
		$(this).toggleClass("active");
		$(".menu").fadeToggle();
		$(".sub").css('display','none');
		$(".menu li i").removeClass('active');
	});	
	$(".menu li i").click(function() {
		$(this).toggleClass('active');
		$(".menu").find('.sub' ).slideUp();
		if($(this).hasClass("active")){
			$(".menu li i").removeClass('active')
			$(this).next().slideToggle();
			$(this).toggleClass('active');
		}
	});	
})