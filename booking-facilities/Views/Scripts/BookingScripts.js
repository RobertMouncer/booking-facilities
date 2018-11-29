function updateDateTime() {
    document.getElementById("BookingDateTime").value = document.getElementById("BookingDateId").value + "T" + $("#timeDDL :selected").text();
}

function updateSports() {
    document.getElementById("BookingDateId").value = document.getElementById("BookingDateTime").value = "";
    $("#timeDDL").empty();
    urlVenues = '@Url.Content("~/")' + "api/Sports/getSportsByVenue" + '/' + $("#venueDDL").val();


    $.ajax({
        url: urlVenues,
        success: function (json) {

            var items = "";
            $.each(json, function (i, sport) {
                items += "<option value='" + sport.sportId + "'>" + sport.sportName + "</option>";
            })
            $("#sportDDL").empty().html(items);

        }
    });
}

function getTimes() {

    var urlBooking = '@Url.Content("~/")' + "api/booking/" + document.getElementById("BookingDateId").value + "/" + $("#venueDDL").val() + "/" + $("#sportDDL").val();
    $.ajax({
        url: urlBooking,
        success: function (json) {
            var times = "";
            $("#BookingTimeForm").show();

            $.each(json, function (i, time) {
                var dout = new Date(Date.parse(time));
                times += "<option value='" + i + "'>" + ("0" + dout.getHours()).slice(-2) + ":00:00" + "</option>";
            })
            $("#timeDDL").empty().html(times);
            updateDateTime();

        }

    });

}
