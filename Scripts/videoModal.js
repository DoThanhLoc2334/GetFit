

@section Scripts {
    <script>
        $(document).ready(function() {
            $('.video-btn').click(function () {
                var videoUrl = $(this).data('video');
                $('#videoSource').attr('src', videoUrl);
                $('#workoutVideo')[0].load();
                $('#videoModal').modal('show');
            });

        $('#videoModal').on('hidden.bs.modal', function() {
            $('#workoutVideo')[0].pause();
        $('#videoSource').attr('src', '');
            });
        });
    </script>
}