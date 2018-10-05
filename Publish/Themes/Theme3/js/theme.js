    $('#topbar').each(function () {
            var blk = $(this);
            var img = blk.find("img").first();
            var urlImg = img.attr("src");
            var imageUrl = urlImg;
            if (urlImg) {
                blk.css('background', 'url("' + imageUrl + '")').css({
                    "background-size": "cover",
                    "background-repeat": "no-repeat",
                    "background-position": "center",
                });
                img.hide();
            }
            else {
                blk.removeAttr("style");
            }
        });