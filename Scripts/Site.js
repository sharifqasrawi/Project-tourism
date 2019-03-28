//Page preloader
$(window).on("load", function () {
    $(".loader-wrapper").fadeOut("slow");
});

var amountScrolled = 200;

$(window).scroll(function () {
    if ($(window).scrollTop() > amountScrolled) {
        $('a.backToTop').fadeIn('slow');
    } else {
        $('a.backToTop').fadeOut('slow');
    }
});

$('a.backToTop').click(function () {
    $('html, body').animate({
        scrollTop: 0
    }, 700);
    return false;
});



$(function () {
    $('#Country').change(function () {
        $.getJSON('/Account/Citylist/' + $('#Country').val(), function (data) {
            var items;
            $.each(data, function (i, city) {
                items += "<option value='" + city.Value + "'>" + city.Text + "</option>";
            });
            $('#city').html(items);
        });
    });

    $('#SourceCountry').change(function () {
        $.getJSON('/Offer/Citylist/' + $('#SourceCountry').val(), function (data) {
            var items;
            $.each(data, function (i, city) {
                items += "<option value='" + city.Value + "'>" + city.Text + "</option>";
            });
            $('#SourceCity').html(items);
        });
    });

    $('#DestinationCountry').change(function () {
        $.getJSON('/Offer/Citylist/' + $('#DestinationCountry').val(), function (data) {
            var items;
            $.each(data, function (i, city) {
                items += "<option value='" + city.Value + "'>" + city.Text + "</option>";
            });
            $('#DestinationCity').html(items);
        });
    });


    $("#datepicker").datepicker();



    $('#timepicker').timepicker();

    $("#CheckIndatepicker").datepicker();
    $("#CheckOutdatepicker").datepicker();
    $("#Date").datepicker();

    $("#CheckIndatepicker").val("");
    $("#CheckOutdatepicker").val("");

    //Showing uploading images befor submit
    $("#uploadFile").on('change', function () {

        //Get count of selected files
        var countFiles = $(this)[0].files.length;

        var imgPath = $(this)[0].value;
        var extn = imgPath.substring(imgPath.lastIndexOf('.') + 1).toLowerCase();
        var image_holder = $("#ImageHolder");
        image_holder.empty();

        if (extn == "gif" || extn == "png" || extn == "jpg" || extn == "jpeg") {
            if (typeof (FileReader) != "undefined") {

                //loop for each file selected for uploaded.
                for (var i = 0; i < countFiles; i++) {

                    var reader = new FileReader();
                    reader.onload = function (e) {
                        $("<img />", {
                            "src": e.target.result,
                            "class": "thumb-image"
                        }).appendTo(image_holder);

                        $("#EditCatImg").hide();
                    }

                    image_holder.show();
                    reader.readAsDataURL($(this)[0].files[i]);
                }

            } else {
                alert("This browser does not support FileReader.");
            }
        } else {
            alert("Pls select only images");
        }
    });


    $('#CheckOutdatepicker').on('change', function () {
        var In = new Date($('#CheckIndatepicker').datepicker({ dateFormat: 'dd/mm/yyyy' }).val());
        var Out = new Date($('#CheckOutdatepicker').datepicker({ dateFormat: 'dd/mm/yyyy' }).val());

        var oneDay = 24 * 60 * 60 * 1000;

        var duration = Math.round((Out.getTime() - In.getTime()) / (oneDay));

        var nightCost = parseFloat($('#nightCost').html());

        var cost = duration * nightCost;
        if (cost > 0)
            $('#cost').html("Total Cost: " + cost.toString() + " $");
        else
            $('#cost').html("Total Cost: 0 ");
    });

    $("#HotelImagesBtn").click(function () {
        $("#HotelImagesPanel").slideToggle();
    });


});


/*Image Modal*/
var modal = document.getElementById('ImgModal');

var img = document.getElementById('CityImg');
var modalImg = document.getElementById("img");
var captionText = document.getElementById("caption");

function openImg(source) {
   
    modal.style.display = "block";
    modalImg.src = source;
}

var span = document.getElementsByClassName("close")[0];

span.onclick = function () {
    modal.style.display = "none";
}


function CheckDates(d1, d2){
    if (d1 == d2) {
        $('#errorModal').modal();
        return false;
    }
}