$(document).ready(function () {

    InitializeCalendar();

});

var calendar;

function InitializeCalendar() {

    try {

        var calendarEl = document.getElementById('calendar');

        if (!calendarEl)
            return;

        calendar = new FullCalendar.Calendar(calendarEl, {

            initialView: 'dayGridMonth',

            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay'
            },

            selectable: true,
            editable: true,
            timeZone: 'local',

            select: function (selectionInfo) {

                onShowModalAddEvent(selectionInfo);

            },

            events: function (fetchInfo, successCallback, failureCallback) {

                var hiddenDoctorId = $('#hiddenDoctorId').val();
                var doctorId = $('#DoctorId').val() || hiddenDoctorId;
                $.ajax({

                    url: '/api/AppointmentApi/GetCalendarData?doctorId=' + doctorId,

                    type: 'GET',

                    dataType: 'json',

                    success: function (response) {

                        var events = [];

                        if (response.status === 1) {

                            response.data.forEach(data => {

                                events.push({

                                    id: data.id,

                                    title: data.title,

                                    start: data.startDate,

                                    end: data.endDate,

                                    description: data.description,

                                    duration: data.duration,

                                    doctorId: data.doctorId,

                                    patientId: data.patientId,

                                    backgroundColor:
                                        data.isDoctorApproved === 1
                                            ? "#28a745"
                                            : "#dc3545",

                                    borderColor: "#162466",

                                    textColor: "#fff"
                                });

                            });

                            successCallback(events);

                        }
                        else {

                            failureCallback(response.message);

                        }

                    },

                    error: function (xhr, status, error) {

                        failureCallback(error);

                    }

                });

            },

            eventClick: function (info) {

                getEventDetailsByEventId(info);

            }

        });

        calendar.render();

    }
    catch (e) {

        console.log(e);

    }

}

function onShowModalAddEvent(obj, isEventDetail = false) {
    $('#deleteButton').hide();
        // $('#status').hide();


    if (isEventDetail) {
        $('#deleteButton').show();
        $('#status').show();

        $('#Title').val(obj.title);

        $('#Description').val(obj.description);

        $('#StartDate').val(
            moment(obj.startDate).format('YYYY-MM-DDTHH:mm')
        );

        $('#DoctorId').val(obj.doctorId);

        $('#PatientId').val(obj.patientId);

        $('#id').val(obj.id);

        $('#Duration').val(obj.duration);


        $('#status').addClass('btn-info');


        $('#status').html(
            obj.appointmentStatus 
        );

    }
    else {

        let startDate = obj.start || obj.date;

        $('#StartDate').val(
            moment(startDate).format('YYYY-MM-DDTHH:mm')
        );

    }

    $('#appointmentModal').modal('show');

}

function onSubmitForm(e) {

    e.preventDefault();

    const startDate = moment($('#StartDate').val());

if (startDate.isBefore(moment())) {

    $.notify("Appointment date cannot be in the past", "error");
    return;
} 
    var requestData = {

        Id: $('#id').val(),

        Title: $('#Title').val(),

        Description: $('#Description').val(),

        StartDate: $('#StartDate').val(),

        Duration: $('#Duration').val(),

        DoctorId: $('#DoctorId').val() || $('#hiddenDoctorId').val(),

        PatientId: $('#PatientId').val()

    };

    $.ajax({

        url: '/api/AppointmentApi/SaveCalendarData',

        type: 'POST',

        data: JSON.stringify(requestData),

        contentType: 'application/json; charset=utf-8',

        success: function (response) {

            $.notify(
                response.message,
                response.status === 1 || response.status === 2
                    ? "success"
                    : "error"
            );

            onModelClose();

            calendar.refetchEvents();

        },

        error: function (xhr, status, error) {

            $.notify(error, "error");

        }

    });

}

function onModelClose() {

    $('#appointmentModal').modal('hide');

    $('#Title').val('');

    $('#Description').val('');

    $('#StartDate').val('');

    $('#PatientId').val('');

    $('#id').val('');

    $('#Duration').val('');

}

function getEventDetailsByEventId(info) {

    $.ajax({

        url: '/api/AppointmentApi/GetCalendarDataById/' + info.event.id,

        type: 'GET',

        dataType: 'json',

        success: function (response) {

            if (response.status === 1 && response.data) {

                onShowModalAddEvent(response.data, true);

            }

        },

        error: function (xhr, status, error) {

            $.notify(error, "error");

        }

    });

}

function DeleteAppointment() {
    $.ajax({
        method: 'DELETE',
        url: '/api/AppointmentApi/DeleteCalendarData/' + $('#id').val(),
        success: function (response) {
            $.notify(
                response.message,
                response.status === 1 || response.status === 2
                    ? "success"
                    : "error"
            );
            onModelClose();
            calendar.refetchEvents();
        },
        error: function (xhr, status, error) {
            $.notify(error, "error");
            onModelClose();

        }
    })
}


function CancelAppointment() {

    const appointmentId = $('#id').val();

    if (!appointmentId) {
        $.notify("Appointment id not found", "error");
        return;
    }

    $.ajax({
        method: 'GET',
        url: '/api/AppointmentApi/CancelAppointment/' + appointmentId,

        success: function (response) {
            $.notify(response.message, response.status === 1 ? "success" : "error");
            onModelClose();
            calendar.refetchEvents();
        },

        error: function (xhr, status, error) {
            $.notify(error, "error");
        }
    });
}

function AcceptAppointment() {
    $.ajax({
        method: 'GET',
        url: '/api/AppointmentApi/AcceptAppointment/' + $('#id').val(),
        success: function (response) {
            $.notify(
                response.message,
                response.status === 1 || response.status === 2
                    ? "success"
                    : "error"
            );
            onModelClose();
            calendar.refetchEvents();
        },
        error: function (xhr, status, error) {
            $.notify(error, "error");
            onModelClose();

        }
    })
}


$('#DoctorId').change(function () {

    if (calendar) {

        calendar.refetchEvents();

    }

});