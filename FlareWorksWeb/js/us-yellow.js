$(document).ready(
		function() {

			$(".listing").hover(function() {
				$(this).children("ul").show();
			}, function() {
				$(this).children("ul").hide();
			});

			$("#map-title a").toggle(function() {
				$("#map-container").animate({
					height : '50px'
				}, 300, function() {
					$("#map-title span.button-center").text("Expand Map");
				});
			}, function() {

				$("#map-container").animate({
					height : '300px'
				}, 300, function() {
					$("#map-title span.button-center").text("Minimize Map");
				});
			});

			// open links with rel=external in a new window
			$("a[rel^=external]").attr("target", "_blank");

			// disable submit button after form is submitted to avoid multiple
			// submissions.
			$('.single-submit').submit(

					function() {
						$(this).find("input[type='submit']").attr("disabled",
								"disabled").css('opacity', '0.5');

					});

		}); // end document.ready
