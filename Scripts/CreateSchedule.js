@section Scripts {
    <script>
        $(document).ready(function () {
            $('#add-day').click(function () {
                var newDay = $('.workout-day:first').clone();
                newDay.find('input').val('');
                newDay.find('.workouts-container').html($('.workout-day:first .workouts-container').html());
                $('#workout-schedule').append(newDay);
            });

        $(document).on('click', '.add-workout', function () {
                var workoutEntry = $(this).siblings('.workouts-container').find('.workout-entry:first').clone();
        workoutEntry.find('input').val('');
        workoutEntry.find('select').val(''); 
        $(this).siblings('.workouts-container').append(workoutEntry);
            });

        $(document).on('click', '.remove-workout', function () {
                if ($(this).closest('.workouts-container').find('.workout-entry').length > 1) {
            $(this).closest('.workout-entry').remove();
                }
            });

        $(document).on('click', '.remove-day', function () {
                if ($('.workout-day').length > 1) {
            $(this).closest('.workout-day').remove();
                }
            });

        $(document).on('change', '.day-select', function () {
                var dayValue = $(this).val();
        $(this).closest('.workout-day').find('input[name="days[]"]').val(dayValue);
            });
        });
    </script>
}