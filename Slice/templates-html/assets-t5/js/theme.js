 $(document).ready(function(){
	$('#topbar').each(function () {
		
		$(".menuMobile").click(function() {
			$(this).toggleClass("active");
			$(".menu").fadeToggle();
		});
	});
 });