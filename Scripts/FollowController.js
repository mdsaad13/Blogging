$(document).on('click', '.Follow', function () {
    var UserName = $('#CurrentUserName').val();
    var formData = new FormData();
    formData.append('userID', $(this).attr('id'));
    $.ajax({
        type: 'post',
        url: '/Account/FollowUser',
        data: formData,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.status) {
                $('.Follow').removeClass('Follow').addClass('Unfollow').attr('title', 'Following');
                $('.Unfollow').text('Unfollow');
                var Before = $('#FollowersCount').text();
                $('#FollowersCount').text(+Before + 1);
                Swal.fire({
                    position: 'top-end',
                    icon: 'success',
                    title: '',
                    text: 'Thank you for following ' + UserName + '!',
                    showConfirmButton: false,
                });
            } else {
                // unknown error
                Swal.fire({
                    position: 'top-end',
                    icon: 'error',
                    title: '',
                    text: 'An unknown error has occured. Please try again later!',
                    showConfirmButton: false,
                });
            }
        }
    });
});

$(document).on('click', '.Unfollow', function () {
    var formData = new FormData();
    formData.append('userID', $(this).attr('id'));
    $.ajax({
        type: 'post',
        url: '/Account/UnFollowUser',
        data: formData,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.status) {
                $('.Unfollow').removeClass('Unfollow').addClass('Follow').attr('title', 'Follow');
                $('.Follow').text('Follow');
                var Before = $('#FollowersCount').text();
                $('#FollowersCount').text(+Before - 1);
            } else {
                // unknown error
                Swal.fire({
                    position: 'top-end',
                    icon: 'error',
                    title: '',
                    text: 'An unknown error has occured. Please try again later!',
                    showConfirmButton: false,
                });
            }
        }
    });
});