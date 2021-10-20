window.datePicker = {
    init: function (datePickerRef, id, minDate, maxDate) {
        $('#' + id).daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            timePicker: true,
            "timePicker24Hour": true,
            "timePickerSeconds": true,
            "autoUpdateInput": false,
            "minDate": new Date(minDate),
            // "maxDate": new Date(maxDate),
            locale: {
                format: 'M/DD/YYYY H:mm:ss',
                "applyLabel": "Հաստատել",
                "cancelLabel": "Չեղարկել",
                "daysOfWeek": [
                    'Կրկ',
                    'Երկ',
                    'Երք',
                    'Չոր',
                    'Հնգ',
                    'Ուրբ',
                    'Շաբ',
                ],
                "monthNames": [
                    'Հունվար',
                    'Փետրվար',
                    'Մարտ‎',
                    'Ապրիլ‎',
                    'Մայիս‎',
                    'Հունիս‎',
                    'Հուլիս‎',
                    'Օգոստոս‎',
                    'Սեպտեմբեր',
                    'Հոկտեմբեր‎',
                    'Նոյեմբեր‎',
                    'Դեկտեմբեր',
                ],
                "firstDay": 1
            }
        }, function (start, end, label) {
            datePickerRef.invokeMethodAsync("SetDate", start)
        });
    }
};