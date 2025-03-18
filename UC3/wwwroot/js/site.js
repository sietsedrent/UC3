function HidePassword() {
    var x = document.getElementById("hiddenpw");
    if (x.type == null || x.type === "password") {
        x.type = "text";
    } else {
        x.type = "password";
    }
}

// Calendar functionality
$(document).ready(function () {
    const isProfilePage = window.location.pathname.includes('/Profile');

    if (isProfilePage) {
        const profileUserId = parseInt($('#weekCalendar').data('user-id') || 0);
        const isOwnProfile = $('#weekCalendar').data('is-own-profile') === "true";

        loadWorkoutPlannings(profileUserId, isOwnProfile);
    } else {
        const userId = parseInt($('#weekCalendar').data('user-id') || 0);
        loadWorkoutPlannings(userId, true);

        $("#changeBioBtn").click(function () {
            $("#bioViewContent").hide();
            $("#bioEditContent").show();
        });

        $("#cancelBioBtn").click(function () {
            $("#bioEditContent").hide();
            $("#bioViewContent").show();
        });

        $("#saveBioBtn").click(function () {
            const newBio = $("#bioTextarea").val();

            $.ajax({
                url: '/Home/UpdateBio',
                type: 'POST',
                data: { bio: newBio },
                success: function (response) {
                    if (response.success) {
                        $("#bioViewContent p").text("Bio: " + newBio);
                        $("#bioEditContent").hide();
                        $("#bioViewContent").show();
                    } else {
                        alert("Er is een fout opgetreden bij het opslaan van de bio.");
                    }
                },
                error: function () {
                    alert("Er is een fout opgetreden bij het opslaan van de bio.");
                }
            });
        });
    }
});

function loadWorkoutPlannings(userId, isEditable) {
    $.ajax({
        url: '/Home/GetWorkoutPlannings',
        type: 'GET',
        data: { userId: userId },
        success: function (data) {
            renderSimpleWeekView(userId, isEditable, data.plannings);
        },
        error: function () {
            console.error('Fout bij ophalen workout planningen');
            renderSimpleWeekView(userId, isEditable, []);
        }
    });
}
function renderSimpleWeekView(userId, isEditable, workoutDays) {
    const now = new Date();
    const currentDay = now.getDay();
    const date = new Date(now.getTime());
    date.setHours(0, 0, 0, 0);
    date.setDate(date.getDate() + 3 - (date.getDay() + 6) % 7);
    const week1 = new Date(date.getFullYear(), 0, 4);
    const weekNumber = 1 + Math.round(((date.getTime() - week1.getTime()) / 86400000 - 3 + (week1.getDay() + 6) % 7) / 7);

    let weekView = `
        <div class="week-header">
            <h3>Week: ${weekNumber}</h3>
        </div>
        <div class="weekdays-container">`;

    const dayNames = ['Maandag', 'Dinsdag', 'Woensdag', 'Donderdag', 'Vrijdag', 'Zaterdag', 'Zondag'];

    for (let i = 0; i < 7; i++) {
        const hasWorkout = workoutDays.includes(i);
        const isToday = (i === (currentDay === 0 ? 6 : currentDay - 1));
        const editableClass = isEditable ? 'cursor-pointer' : '';
        const dataAttr = isEditable ? `data-day-index="${i}"` : '';

        weekView += `
            <div class="day-container ${isToday ? 'today' : ''}">
                <div class="day-name">${dayNames[i]}</div>
                <div class="day-circle ${hasWorkout ? 'has-workout' : ''} ${editableClass}" ${dataAttr}></div>
            </div>`;
    }

    weekView += `
        </div>`;

    if (isEditable) {
        weekView += `
        <div class="week-footer">
            <span class="workout-legend">
                <span class="legend-item">
                    <span class="day-circle has-workout"></span> Workout gepland
                </span>
                <span class="legend-item">
                    <span class="day-circle"></span> Geen workout
                </span>
            </span>
            <span class="workout-help">Klik op een cirkel om een workout toe te voegen of te verwijderen</span>
        </div>`;
    }

    // Add to DOM
    $('#weekCalendar').html(weekView);

    if (isEditable) {
        $(document).on('click', '.day-circle.cursor-pointer', function () {
            const dayIndex = $(this).data('day-index');
            const hasWorkout = !$(this).hasClass('has-workout');
            $(this).toggleClass('has-workout');

            $.ajax({
                url: '/Home/UpdateWorkoutPlanning',
                type: 'POST',
                data: { dayIndex: dayIndex, hasWorkout: hasWorkout },
                success: function (response) {
                    if (!response.success) {
                        console.error('Fout bij opslaan van workout planning');
                    }
                },
                error: function () {
                    console.error('Fout bij opslaan van workout planning');
                }
            });
        });
    }
}