$(function () {
    $("input[ajax-selector='personSelectCheckbox']").change(function () {

        var $input = $(this);
        $input.prop('disabled', true);

        var data = {};
        data['selected'] = $(this).is(':checked');
        data['personId'] = $(this).attr('personId');
        data['districtId'] = $(this).attr('districtId');
              
        $.ajax({
            url: $(this).data('url'),
            type: 'POST',
            data: data,
            success: function (result) {
                $input.attr('checked', result.selected);
                $input.prop('disabled', false);
            },
            error: function (result) {
                // In case of error revert selection done
                $input.attr('checked', !data['selected']);
                $input.prop('disabled', false);
            }
        });
    });
});